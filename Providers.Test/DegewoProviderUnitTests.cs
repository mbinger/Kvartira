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
        public async Task DegewoProvider_WithDetails1_ShouldLoadDetails()
        {
            var downloader = new ResDownloader(new[] { Resource._2021_11_10_07_08_42_DEGEWO_details_W1400_40274_0940_0404 });
            var logMock = new Mock<ILog>(MockBehavior.Loose);
            var provider = new DegewoProvider(downloader, logMock.Object);

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
            var provider = new DegewoProvider(downloader, logMock.Object);

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
            var provider = new DegewoProvider(downloader, logMock.Object);

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
            var provider = new DegewoProvider(downloader, logMock.Object);

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
