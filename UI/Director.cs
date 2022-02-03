using Common;
using Data;
using Providers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace UI
{
    public class Director
    {
        private readonly AppConfig config;
        private readonly IProvider[] providers;
        private readonly Log log;

        public Director(AppConfig config, IProvider[] providers, Log log)
        {
            this.config = config;
            this.providers = providers;
            this.log = log;
        }

        private void BindCardToEntity(WohnungCard card, WohnungEntity entity)
        {
            entity.Ueberschrift = card.Header;
            entity.Anschrift = card.Anschrift;
            entity.Balkon = card.Balkon;
            entity.Beschreibung = card.Beschreibung;
            entity.Bezirk = card.Bezirk;
            entity.Etage = card.Etage;
            entity.Etagen = card.Etagen;
            entity.Flaeche = card.Flaeche;
            entity.FreiAb = card.FreiAb;
            entity.Keller = card.Keller;
            entity.MieteKalt = card.MieteKalt;
            entity.MieteWarm = card.MieteWarm;
            entity.Wbs = card.Wbs;
            entity.Zimmer = card.Zimmer;
        }

        private IProvider GetProviderByName(string name)
        {
            var provider = providers.FirstOrDefault(p => string.Compare(p.Name, name, true) == 0);
            if (provider == null)
            {
                log.Write($"Provider '{name}' not founnd");
            }

            return provider;
        }

        /// <summary>
        /// Load overview
        /// </summary>
        /// <returns>Count of the new items</returns>
        public async Task<int> LoadAsync()
        {
            var result = new ConcurrentBag<int>();
            var searchList = config.SearchConfig.Where(p => p.Active).ToList();

            var searchListGroupByProvider = searchList.GroupBy(p => p.ProviderName).ToList();

            var providerTasks = new List<Task>();
            foreach (var providerItems in searchListGroupByProvider)
            {
                try
                {
                    var provider = GetProviderByName(providerItems.Key);
                    if (provider == null)
                    {
                        continue;
                    }

                    var providerTask = Task.Run(async () =>
                    {
                        var res = await ProcessProvider(provider, providerItems.ToList());
                        result.Add(res);
                    });
                    providerTasks.Add(providerTask);
                }
                catch (Exception ex)
                {
                    log.Write("ERROR Director.LoadAsync\n" + ex);
                }
            }

            await Task.WhenAll(providerTasks);

            await LoadAllDetailsAllAsync();

            var summ = result.Sum();
            return summ;
        }


        private async Task<int> ProcessProvider(IProvider provider, List<Search> items)
        {
            try
            {
                var newIdsGlobal = new List<string>();
                foreach (var search in items)
                {
                    var headers = await provider.LoadIndexAsync(search);
                    if (headers == null)
                    {
                        continue;
                    }

                    if (headers.WohnungIds != null)
                    {
                        headers.WohnungIds = headers.WohnungIds.Distinct().ToList();
                    }

                    using (var db = new WohnungDb())
                    {
                        var existingIds = db.Wohnungen.Where(p => p.Provider == provider.Name).Select(p => p.WohnungId).ToList();
                        var newIds = headers.WohnungIds.Where(p => !existingIds.Contains(p)).ToList();
                        newIdsGlobal.AddRange(newIds);

                        //insert
                        var now = DateTime.Now;
                        foreach (var newHeader in newIds)
                        {
                            var header = new WohnungEntity
                            {
                                WohnungId = newHeader,
                                Provider = search.ProviderName,
                                Geladen = now,
                                Wichtigkeit = search.Importance,
                                SucheShort = search.DescriptionShort,
                                SucheDetails = search.DescriptionLong,
                                Gemeldet = null,
                                Gesehen = null,
                                LoadDetailsTries = 0
                            };
                            db.Wohnungen.Add(header);
                        }

                        var providerHealthEntry = db.ProviderHealthLogs.FirstOrDefault(p => p.ProviderName == provider.Name);
                        if (providerHealthEntry == null)
                        {
                            providerHealthEntry = new ProviderHealthEntity
                            {
                                ProviderName = provider.Name
                            };
                            db.ProviderHealthLogs.Add(providerHealthEntry);
                        }

                        providerHealthEntry.LastUpdate = now;

                        if (headers.WohnungIds.Any())
                        {
                            providerHealthEntry.IdsLoaded = now;
                        }
                        if (newIds.Any())
                        {
                            providerHealthEntry.NewIdsLoaded = now;
                        }

                        await db.SaveChangesAsync();
                    }
                }

                return newIdsGlobal.Distinct().Count();
            }
            catch (Exception ex)
            {
                log.Write($"ERROR Director.ProcessProvider({provider.Name}).\n" + ex);
            }

            return -1;
        }

        public async Task<bool> TryReloadDetails(string providerName, string wohnungId)
        {
            var provider = GetProviderByName(providerName);
            if (provider == null)
            {
                log.Write("TryReloadDetails: Unable to find provider");
                return false;
            }

            using (var db = new WohnungDb())
            {
                var header = db.Wohnungen.FirstOrDefault(p => p.Provider == providerName && p.WohnungId == wohnungId);
                if (header == null)
                {
                    log.Write($"TryReloadDetails: WohnungHeader '{providerName}' '{wohnungId}' not found");
                    return false;
                }
            }

            var card = await provider.LoadDetailsAsync(wohnungId, true);
            if (card == null)
            {
                log.Write("TryReloadDetails: LoadDetailsAsync returns null");
                return false;
            }

            using (var db = new WohnungDb())
            {
                var header = db.Wohnungen.FirstOrDefault(p => p.Provider == providerName && p.WohnungId == wohnungId);
                if (header == null)
                {
                    log.Write($"TryReloadDetails: WohnungHeader '{providerName}' '{wohnungId}' not found after load");
                    return false;
                }
                header.LoadDetailsTries++;

                BindCardToEntity(card, header);

                db.SaveChanges();
            }

            return true;
        }

        private class ProviderWohnungId
        {
            public Guid HeaderId { get; set; }
            public string Provider { get; set; }
            public string WohnungId { get; set; }
        }

        public async Task LoadAllDetailsAllAsync()
        {
            //load details
            List<ProviderWohnungId> ids;
            using (var db = new WohnungDb())
            {
                ids = db.Wohnungen
                    .Where(p => p.LoadDetailsTries < 3 && p.SucheDetails == null)
                    .Select(p => new ProviderWohnungId
                    {
                        HeaderId = p.Id,
                        Provider = p.Provider,
                        WohnungId = p.WohnungId
                    }).ToList();
            }

            var idsGroupedByProvider = ids.GroupBy(p => p.Provider);

            var tasks = new List<Task>();

            foreach (var providerIds in idsGroupedByProvider)
            {
                var provider = GetProviderByName(providerIds.Key);
                if (provider == null)
                {
                    continue;
                }

                var task = ProcessProviderCards(provider, providerIds.ToList());
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
        }

        private async Task ProcessProviderCards(IProvider provider, List<ProviderWohnungId> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    try
                    {
                        var card = await provider.LoadDetailsAsync(id.WohnungId, false);

                        var now = DateTime.Now;

                        using (var db = new WohnungDb())
                        {
                            if (card != null)
                            {
                                var header = db.Wohnungen.Single(p => p.Id == id.HeaderId);
                                BindCardToEntity(card, header);

                                await db.SaveChangesAsync();

                                var providerHealthEntry = db.ProviderHealthLogs.FirstOrDefault(p => p.ProviderName == provider.Name);
                                if (providerHealthEntry == null)
                                {
                                    providerHealthEntry = new ProviderHealthEntity
                                    {
                                        ProviderName = provider.Name
                                    };
                                    db.ProviderHealthLogs.Add(providerHealthEntry);
                                }

                                providerHealthEntry.DetailsLoaded = now;
                                if (card.Complete)
                                {
                                    providerHealthEntry.AllDetailsComlete = now;
                                }

                                await db.SaveChangesAsync();
                            }
                            else
                            {
                                var header = db.Wohnungen.Single(p => p.Id == id.HeaderId);
                                header.LoadDetailsTries++;
                                await db.SaveChangesAsync();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Write($"ERROR Director.ProcessProviderCards1({provider.Name}).\n" + ex);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Write($"ERROR Director.ProcessProviderCards2({provider.Name}).\n" + ex);
            }
        }

        public string GetOpenDetailsUrl(string providerName, string wohnungId)
        {
            foreach (var provider in providers)
            {
                if (string.Compare(provider.Name, providerName, true) == 0)
                {
                    return provider.GetOpenDetailsUrl(wohnungId);
                }
            }

            return null;
        }

        public class ProviderHealthRating
        {
            public ProviderHealthEntity Entry { get; set; }
            public string Reason { get; set; }
            public ProviderHealthColor Color { get; set; }
        }

        private ProviderHealthRating GetRatingForEntry(ProviderHealthEntity entry)
        {
            var result = new ProviderHealthRating
            {
                Entry = entry,
                Color = ProviderHealthColor.Green,
                Reason = "OK"
            };

            var dateTimePattern = "dd.MM.yyyy HH:mm";

            //not called yet
            if (entry?.LastUpdate == null)
            {
                result.Reason = "Еще не загружался";
                result.Color = ProviderHealthColor.Gray;
                return result;
            }

            if (entry.IdsLoaded == null)
            {
                result.Reason = "Индекс никогда не загружался";
                result.Color = ProviderHealthColor.Red;
                return result;
            }

            if (entry.NewIdsLoaded == null)
            {
                result.Reason = "Новые квартиры никогда не были найдены";
                result.Color = ProviderHealthColor.Red;
                return result;
            }

            if (entry.DetailsLoaded == null)
            {
                result.Reason = "Детали квартир никогда не запрашивались";
                result.Color = ProviderHealthColor.Red;
                return result;
            }

            if (entry.AllDetailsComlete == null)
            {
                result.Reason = "Детали квартир никогда не загружались полностью";
                result.Color = ProviderHealthColor.Red;
                return result;
            }

            var now = DateTime.Now;

            if ((now - entry.LastUpdate.Value).TotalDays >= 1)
            {
                result.Reason = $"Данные не обновлялись с {entry.LastUpdate.Value.ToString(dateTimePattern)}";
                result.Color = ProviderHealthColor.Red;
                return result;
            }

            if ((now - entry.NewIdsLoaded.Value).TotalDays >= 7)
            {
                result.Reason = $"Не найдено новых квартир с {entry.NewIdsLoaded.Value.ToString(dateTimePattern)}";
                result.Color = ProviderHealthColor.Red;
                return result;
            }

            if (Math.Abs((entry.NewIdsLoaded.Value - entry.DetailsLoaded.Value).TotalMinutes) >= 30)
            {
                result.Reason = $"Детали квартир не загружаются с {entry.DetailsLoaded.Value.ToString(dateTimePattern)}";
                result.Color = ProviderHealthColor.Red;
                return result;
            }

            if (Math.Abs((entry.DetailsLoaded.Value - entry.AllDetailsComlete.Value).TotalMinutes) >= 30)
            {
                result.Reason = $"Детали квартир загружаются неполностью с {entry.DetailsLoaded.Value.ToString(dateTimePattern)}";
                result.Color = ProviderHealthColor.Yellow;
                return result;
            }

            if ((entry.LastUpdate.Value - entry.IdsLoaded.Value).TotalHours >= 2)
            {
                result.Reason = $"С последнего обновления не найдено никаких квартир";
                result.Color = ProviderHealthColor.Yellow;
                return result;
            }

            if ((now - entry.NewIdsLoaded.Value).TotalDays >= 3)
            {
                result.Reason = $"Не найдено новых квартир с {entry.NewIdsLoaded.Value.ToString(dateTimePattern)}";
                result.Color = ProviderHealthColor.Yellow;
                return result;
            }

            return result;
        }

        public List<ProviderHealthRating> GetRating()
        {
            List<ProviderHealthEntity> providers;
            using (var db = new WohnungDb())
            {
                providers = db.ProviderHealthLogs.ToList();
            }

            if (!providers.Any())
            {
                return new List<ProviderHealthRating>
                {
                    GetRatingForEntry(null)
                };
            }

            var ratings = providers.Select(GetRatingForEntry)
                .OrderBy(p => p.Entry.ProviderName)
                .ToList();

            return ratings;
        }

        public ProviderHealthRating GetRatingAll()
        {
            var rating = GetRating();

            var ratings = rating
                .OrderByDescending(p => (int)p.Color)
                .ToList();

            return ratings.First();
        }
    }
}
