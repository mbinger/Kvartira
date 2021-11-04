using Common;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Providers
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

        private async Task<IProvider> GetProviderByNameAsync(string name)
        {
            var provider = providers.FirstOrDefault(p => string.Compare(p.Name, name, true) == 0);
            if (provider == null)
            {
                await log.LogAsync($"Provider '{name}' not founnd");
            }

            return provider;
        }

        /// <summary>
        /// Load overview
        /// </summary>
        /// <returns>Count of the new items</returns>
        public async Task<int> LoadAsync()
        {
            var result = 0;
            var searchList = config.SearchConfig.Where(p => p.Active).ToList();
            foreach (var search in searchList)
            {
                try
                {
                    var provider = await GetProviderByNameAsync(search.ProviderName);
                    if (provider == null)
                    {
                        continue;
                    }

                    var headers = await provider.LoadIndexAsync(search);
                    if (headers == null)
                    {
                        continue;
                    }

                    if (headers.WohnungIds != null)
                    {
                        headers.WohnungIds = headers.WohnungIds.Distinct().ToList();
                    }

                    List<string> newIds;
                    using (var db = new WohnungDb())
                    {
                        var existingIds = db.WohnungHeaders.Where(p => p.Provider == provider.Name).Select(p => p.WohnungId).ToList();
                        newIds = headers.WohnungIds.Where(p => !existingIds.Contains(p)).ToList();

                        //insert
                        var now = DateTime.Now;
                        foreach (var newHeader in newIds)
                        {
                            var header = new WohnungHeaderEntity
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
                            db.WohnungHeaders.Add(header);
                        }

                        await db.SaveChangesAsync();
                    }

                    result += newIds.Count();
                }
                catch (Exception ex)
                {
                    await log.LogAsync("ERROR Director.LoadAsync\n" + ex);
                }
            }

            await LoadAllDetailsAllAsync();

            return result;
        }

        private class ProviderWohnungId
        {
            public int HeaderId { get; set; }
            public string Provider { get; set; }
            public string WohnungId { get; set; }
        }

        private async Task LoadAllDetailsAllAsync()
        {
            //load details
            List<ProviderWohnungId> ids;
            using (var db = new WohnungDb())
            {
                ids = db.WohnungHeaders
                    .Where(p => p.LoadDetailsTries < 3 && !p.Details.Any())
                    .Select(p => new ProviderWohnungId
                    {
                        HeaderId = p.Id,
                        Provider = p.Provider,
                        WohnungId = p.WohnungId
                    }).ToList();
            }

            foreach (var id in ids)
            {
                var provider = await GetProviderByNameAsync(id.Provider);
                if (provider == null)
                {
                    continue;
                }

                var card = await provider.LoadDetailsAsync(id.WohnungId);
                using (var db = new WohnungDb())
                {
                    if (card != null)
                    {
                        var detailsEntity = new WohnungDetailsEntity
                        {
                            WohnungHeaderId = id.HeaderId,
                            Ueberschrift = card.Header,
                            Anschrift = card.Anschrift,
                            Balkon = card.Balkon,
                            Beschreibung = card.Beschreibung,
                            Bezirk = card.Bezirk,
                            Etage = card.Etage,
                            Etagen = card.Etagen,
                            Flaeche = card.Flaeche,
                            FreiAb = card.FreiAb,
                            Keller = card.Keller,
                            MieteKalt = card.MieteKalt,
                            MieteWarm = card.MieteWarm,
                            Wbs = card.Wbs,
                            Zimmer = card.Zimmer
                        };
                        db.WohnungDetails.Add(detailsEntity);
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        var header = db.WohnungHeaders.Single(p => p.Id == id.HeaderId);
                        header.LoadDetailsTries++;
                        await db.SaveChangesAsync();
                    }
                }
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
    }
}
