using Common;
using Moq;
using NUnit.Framework;
using Providers.ImmobilienScout24;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Providers.Test
{
    [TestFixture]
    public class ImmobilienScout24ProviderUnitTests
    {
        [Test]
        public async Task ImmobilienScout_WithIndex_ShouldLoadIds()
        {
            var expectedIds = "103275506,116452644,125725071,126465449,126468258,126492738,128974184,128974185,129369167,129369862,129370780,129371084,129588578,129727666,129783352,130169369,130209885,130293982,130300178,130300259,130390101,130395489,130410164,130448842,130468093,130471531,130476304,130480018,130516497,130554696,130563639,130568812,130591438,130593071,130622304,130632786,130639719,130643556,130678171,130683603,130683620,130686007,130691448,130700428,130702380,130703433,130704111,130707202,130731220,130733632,130739038,130739430,130755792,130756170,130758188,130760222,130765316,130765318,130765320,130766133,130770219,130770231,57832773".Split(',');
            var downloader = new ResDownloader(new[]
            {
                Resource._2021_11_30_12_47_17_ImmoScout24_Captcha,
                Resource._2021_11_30_12_20_45_ImmoScout24_Treptow_Köpenick_page_1,
                Resource._2021_11_30_12_47_17_ImmoScout24_Captcha,
                Resource._2021_11_30_12_20_52_ImmoScout24_Treptow_Köpenick_page_2,
                Resource._2021_11_30_12_47_17_ImmoScout24_Captcha,
                Resource._2021_11_30_12_20_56_ImmoScout24_Treptow_Köpenick_page_3
            });
            
            var logMock = new Mock<ILog>(MockBehavior.Loose);
            var provider = new ImmobilienScout24Provider(downloader, logMock.Object);

            var result = await provider.LoadIndexAsync(new Search
            {
                SearchUrl = "https://www.immobilienscout24.de/Suche/de/berlin/berlin/treptow-koepenick/wohnung-mit-balkon-mieten?haspromotion=false&numberofrooms=3.0-&enteredFrom=result_list"
            });

            Assert.AreEqual(3, result.PagesCount);
            CollectionAssert.AreEquivalent(expectedIds, result.WohnungIds);
        }

        [Test]
        public async Task ImmobilienScout_WithDetails1_ShouldLoadDetails()
        {
            var downloader = new ResDownloader(new[] { Resource._2021_11_30_12_48_27_ImmoScout24_details_130410164 });

            var logMock = new Mock<ILog>(MockBehavior.Loose);
            var provider = new ImmobilienScout24Provider(downloader, logMock.Object);

            var details = await provider.LoadDetailsAsync("130410164");

            Assert.NotNull(details);
            Assert.AreEqual("THE VIEW, Erstbezug,vollklimatisiertes exkl. Penthouse mit unverbaubarer Wasserlage - provisionsfrei", details.Header);
            Assert.AreEqual("Schmöckwitz", details.Bezirk);
            Assert.AreEqual("An der Dahme 5,12527 Berlin, Schmöckwitz", details.Anschrift);
            Assert.AreEqual(3300d, details.MieteKalt);
            Assert.AreEqual(3800d, details.MieteWarm);
            Assert.AreEqual(9000d, details.Kaution);
            Assert.AreEqual(4, details.Etage);
            Assert.AreEqual(1, details.Etagen);
            Assert.AreEqual(4, details.Zimmer);
            Assert.AreEqual(127.02d, details.Flaeche);
            Assert.AreEqual(DateTime.Today, details.FreiAb);

            var expectedBeschreibung = @"52° Nord Nachhaltigkeit spielte bei der Entwicklung des Quartiers 52° Nord eine große Rolle. Neben einem 6.000 m² großen Wasserbecken in dem Regenwasser gesammelt wird und durch Verdunstung zurück in den natürlichen Wasserkreislauf gelangt, wurde auch bei den verschiedenen Gebäuden, neben der architektonischen Qualität, ein Augenmerk auf die ökologische, ökonomische und soziale Nachhaltigkeit gelegt.Die Wohneinheit liegt im 4 OG (Penthouse) eines 18 Parteien Neubau direkt an der Uferpromenade, mit unverbaubaren Wasserblick4 Zimmer (3 SZ), 2 Bäder, 2 Sozialräume, 2 TerrassenMiete: 3.300 kalt/Monat VHB inkl. TG Stellplatz
Keller, Dachterrasse, Fahrstuhl, Vollbad, Duschbad, Einbauküche, Gäste-WC, Barrierefrei, Parkett, FliesenBemerkungen:Sonderausstattung:-Vollklimatisiert-Parkett/Fliesen-Fußbodenheizung-Elekt. Außensonnenschutz-Einbauküche mit  Markengeräten-9 hochwertige Einbauschränke-1TG Stellplatz mit E-Lademöglichkeit im Mietpreis inkludiert *inkl. Möbelierung möglich,     nach Absprache
";
            Assert.AreEqual(expectedBeschreibung, details.Beschreibung);

            Assert.IsFalse(details.Wbs);
            Assert.IsTrue(details.Balkon);
            Assert.IsTrue(details.Keller);
            Assert.IsTrue(details.Complete);
        }
    }
}
