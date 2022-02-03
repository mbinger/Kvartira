using Common;
using Providers.Degewo;
using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;

namespace Providers.Test
{
    [TestFixture]
    public class DegewoProviderUnitTests
    {
        [Test]
        public async Task DegewoProvider_WithIndex_ShouldLoadIds()
        {
            var expectedIds = "W1100-11902-0010-0110,W1100-11902-0086-0217,W1100-11902-0108-0208,W1100-11902-0186-0404,W1140-00526-0226-0504,W1150-00427-0176-0303,W1150-00427-0202-0302,W1150-00427-0207-0503,W1150-00427-0236-0102,W1150-00427-0239-0202,W1300-20219-0105-0202,W1300-53005-0010-8121,W1300-60710-0054-0510,W1400-06080-0080-0106,W1400-06080-0162-0202,W1400-40109-0810-1107,W1400-40109-1210-1607,W1400-40132-0170-0602,W1400-40274-0940-0404,W1400-40602-0170-0603,W1400-51813-0012-0304,W1400-51813-0015-0309,W3110-02304-0126-0201,W3110-02304-0197-0201".Split(',');
            var downloader = new ResDownloader(new[] 
            { 
                Resource._2021_11_10_07_03_22_DEGEWO_Degewo_alles_page_1,
                Resource._2021_11_10_07_03_26_DEGEWO_Degewo_alles_page_2,
                Resource._2021_11_10_07_03_31_DEGEWO_Degewo_alles_page_3
            });
            var logMock = new Mock<ILog>(MockBehavior.Loose);
            var provider = new DegewoProvider(downloader, logMock.Object, logMock.Object);
            
            var result = await provider.LoadIndexAsync(new Search
            {
                SearchUrl = "https://immosuche.degewo.de/de/search?size=10&page=1&property_type_id=1&categories%5B%5D=1&lat=&lon=&area=&address%5Bstreet%5D=&address%5Bcity%5D=&address%5Bzipcode%5D=&address%5Bdistrict%5D=&district=33%2C+46%2C+3%2C+2%2C+28%2C+29%2C+71%2C+64%2C+4-8%2C+58%2C+60%2C+7%2C+40-67&property_number=&price_switch=true&price_radio=null&price_from=&price_to=&qm_radio=null&qm_from=&qm_to=&rooms_radio=null&rooms_from=&rooms_to=&wbs_required=&order=rent_total_without_vat_asc"
            });

            Assert.AreEqual(3, result.PagesCount);
            CollectionAssert.AreEquivalent(expectedIds, result.WohnungIds);
        }

        [Test]
        public async Task DegewoProvider_WithDetails1_ShouldLoadDetails()
        {
            var downloader = new ResDownloader(new[] { Resource._2021_11_10_07_08_42_DEGEWO_details_W1400_40274_0940_0404 });
            var logMock = new Mock<ILog>(MockBehavior.Loose);
            var provider = new DegewoProvider(downloader, logMock.Object, logMock.Object);

            var details = await provider.LoadDetailsAsync("W1400_40274_0940_0404");

            Assert.NotNull(details);
            Assert.AreEqual("Ruhige Wohnlage am Erholungspark Marzahn - WBS erforderlich!", details.Header);
            Assert.IsNull(details.Bezirk);
            Assert.AreEqual("Wuhlestraße 19 12683 Berlin", details.Anschrift);
            Assert.AreEqual(408.30d, details.MieteKalt);
            Assert.AreEqual(559.95, details.MieteWarm);
            Assert.IsNull(details.Kaution);
            Assert.AreEqual(4, details.Etage);
            Assert.AreEqual(10, details.Etagen);
            Assert.AreEqual(2, details.Zimmer);
            Assert.AreEqual(57.67d, details.Flaeche);
            Assert.AreEqual(DateTime.Today, details.FreiAb);
            Assert.AreEqual("Kaution: drei Nettokaltmieten, Bad mit Fenster, Balkon/Loggia, Aufzug, Fern-/Zentralheizung, Fernwarmwasserversorgung", details.Beschreibung);
            Assert.IsTrue(details.Wbs);
            Assert.IsTrue(details.Balkon);
            Assert.IsNull(details.Keller);
            
            Assert.IsTrue(details.Complete);
        }

        [Test]
        public async Task DegewoProvider_WithDetails2_ShouldLoadDetails()
        {
            var downloader = new ResDownloader(new[] { Resource._2021_11_10_07_09_57_DEGEWO_details_W3110_02304_0197_0201 });
            var logMock = new Mock<ILog>(MockBehavior.Loose);
            var provider = new DegewoProvider(downloader, logMock.Object, logMock.Object);

            var details = await provider.LoadDetailsAsync("W3110_02304_0197_0201");

            Assert.NotNull(details);
            Assert.AreEqual("Wohnung mit schönem Schnitt sucht neue Mieter", details.Header);
            Assert.IsNull(details.Bezirk);
            Assert.AreEqual("Köpenicker Landstraße 53 12435 Berlin", details.Anschrift);
            Assert.AreEqual(443.46d, details.MieteKalt);
            Assert.AreEqual(627.61d, details.MieteWarm);
            Assert.IsNull(details.Kaution);
            Assert.AreEqual(2, details.Etage);
            Assert.AreEqual(4, details.Etagen);
            Assert.AreEqual(3, details.Zimmer);
            Assert.AreEqual(69.29d, details.Flaeche);
            Assert.AreEqual(DateTime.Today, details.FreiAb);
            Assert.AreEqual("Kaution: drei Nettokaltmieten, Bad mit Fenster, Balkon/Loggia, Abstellraum, Kabelfernsehen, Fern-/Zentralheizung", details.Beschreibung);
            Assert.IsFalse(details.Wbs);
            Assert.IsTrue(details.Balkon);
            Assert.IsNull(details.Keller);

            Assert.IsTrue(details.Complete);
        }

        [Test]
        public async Task DegewoProvider_WithDetails3_ShouldLoadDetails()
        {
            var downloader = new ResDownloader(new[] { Resource._2021_11_10_11_13_56_DEGEWO_details_W1400_51813_0012_0304 });
            var logMock = new Mock<ILog>(MockBehavior.Loose);
            var provider = new DegewoProvider(downloader, logMock.Object, logMock.Object);

            var details = await provider.LoadDetailsAsync("W1400_51813_0012_0304");

            Assert.NotNull(details);
            Assert.AreEqual("Erstbezug Neubau - Wohnung ausschließlich für zwei oder drei Studierende und/oder Auszubildende", details.Header);
            Assert.IsNull(details.Bezirk);
            Assert.AreEqual("Ludwig-Renn-Straße 56 12679 Berlin", details.Anschrift);
            Assert.AreEqual(475.10d, details.MieteKalt);
            Assert.AreEqual(750.00d, details.MieteWarm);
            Assert.IsNull(details.Kaution);
            Assert.AreEqual(3, details.Etage);
            Assert.AreEqual(8, details.Etagen);
            Assert.AreEqual(3, details.Zimmer);
            Assert.AreEqual(76.07d, details.Flaeche);
            Assert.AreEqual(DateTime.Today, details.FreiAb);
            Assert.AreEqual("Kaution: drei Nettokaltmieten, barrierefrei, Dusche, Balkon/Loggia, Fußbodenheizung, Aufzug, Aufzug ebenerdig, Kabelfernsehen, Fern-/Zentralheizung, Fernwarmwasserversorgung", details.Beschreibung);
            Assert.IsFalse(details.Wbs);
            Assert.IsTrue(details.Balkon);
            Assert.IsNull(details.Keller);

            Assert.IsTrue(details.Complete);
        }

        [Test]
        public async Task DegewoProvider_WithDetails4_ShouldLoadDetails()
        {
            var downloader = new ResDownloader(new[] { Resource._2021_11_10_11_14_12_DEGEWO_details_W1150_00427_0207_0503 });
            var logMock = new Mock<ILog>(MockBehavior.Loose);
            var provider = new DegewoProvider(downloader, logMock.Object, logMock.Object);

            var details = await provider.LoadDetailsAsync("W1150_00427_0207_0503");

            Assert.NotNull(details);
            Assert.AreEqual("Großzügige Familienwohnung in Lankwitz - Erstbezug", details.Header);
            Assert.IsNull(details.Bezirk);
            Assert.AreEqual("Retzowstraße 54A 12249 Berlin", details.Anschrift);
            Assert.AreEqual(1484.85d, details.MieteKalt);
            Assert.AreEqual(1863.96d, details.MieteWarm);
            Assert.IsNull(details.Kaution);
            Assert.AreEqual(5, details.Etage);
            Assert.AreEqual(6, details.Etagen);
            Assert.AreEqual(5, details.Zimmer);
            Assert.AreEqual(126.4d, details.Flaeche);
            Assert.AreEqual(new DateTime(2021, 12, 31), details.FreiAb);
            Assert.AreEqual("Kaution: drei Nettokaltmieten, 2. WC / Gäste WC, Dusche, Balkon/Loggia, Abstellraum, Aufzug, Aufzug ebenerdig, Fern-/Zentralheizung, Fernwarmwasserversorgung", details.Beschreibung);
            Assert.IsFalse(details.Wbs);
            Assert.IsTrue(details.Balkon);
            Assert.IsNull(details.Keller);

            Assert.IsTrue(details.Complete);
        }
    }
}
