using Common;
using Moq;
using NUnit.Framework;
using Providers.ImmobilienScout24;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Providers.Test
{
    [TestFixture]
    public class ImmobilienScout24ProviderUnitTests
    {
        [Test]
        public async Task ImmobilienScout_WithIndex_ShouldLoadIds()
        {
            var appConfig = new AppConfig
            {
                DumpFolder = @"c:\a"
            };
            var log = new Mock<Log>(MockBehavior.Loose);
            
            var downloader = new HttpDownloader(log.Object, appConfig);
            var response = await downloader.GetAsync("https://www.immobilienscout24.de/Suche/de/berlin/berlin/treptow-koepenick/wohnung-mit-balkon-mieten?haspromotion=false&numberofrooms=3.0-&enteredFrom=result_list", "ImmoScout24 Treptow-Köpenick", true);


//            var provider = new ImmobilienScout24Provider(downloader, log.Object);

        }
    }
}
