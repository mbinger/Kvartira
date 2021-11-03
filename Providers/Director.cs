using Common;
using Data;
using System;
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
                    var provider = providers.FirstOrDefault(p => string.Compare(p.Name, search.ProviderName, true) == 0);
                    if (provider == null)
                    {
                        await log.LogAsync($"Provider '{search.ProviderName}' not founnd");
                        continue;
                    }

                    var headers = await provider.LoadIdsAsync(search);
                    if (headers.WohnungIds != null)
                    {
                        headers.WohnungIds = headers.WohnungIds.Distinct().ToList();
                    }

                    using (var db = new WohnungDb())
                    {
                        var existingIds = db.WohnungHeaders.Where(p => p.Provider == provider.Name).Select(p => p.WohnungId).ToList();
                        var newIds = headers.WohnungIds.Where(p => !existingIds.Contains(p)).ToList();

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
                                Gesehen = null
                            };
                            db.WohnungHeaders.Add(header);
                        }

                        await db.SaveChangesAsync();

                        result += newIds.Count();
                    }
                }
                catch (Exception ex)
                {
                    await log.LogAsync("ERROR Director.LoadAsync\n" + ex);
                }
            }

            return result;
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
