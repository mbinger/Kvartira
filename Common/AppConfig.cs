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
        public string DescriptionShort { get; set; }
        public string DescriptionLong { get; set; }
        public string SearchUrl { get; set; }
        public int Importance { get; set; }
        public bool Active { get; set; }
        public bool Test { get; set; }
    }
}
