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
    }
}
