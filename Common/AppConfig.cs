using System.Collections.Generic;

namespace Common
{
    public class AppConfig
    {
        public string DatabaseFile { get; set; }
        public string LogFile { get; set; }
        public string DumpFolder { get; set; }
        public string [] UserAgents { get; set; }
        public List<Search> SearchConfig { get; set; }
    }

    public class Search
    {
        public string ProviderName { get; set; }
        public string Description { get; set; }
        public string SearchUrl { get; set; }
        public bool DreamApartment{ get; set; }
        public bool Active { get; set; }
    }
}
