namespace Glass.LeadTools.Recognition.Tests
{
    using System;
    using System.Linq;
    using System.Windows.Media.Imaging;
    using Imaging.PostProcessing;
    using Imaging.ZoneConfigurations;
    using Imaging.ZoneConfigurations.Numeric;
    using Recognition;
    using Xunit;
    using Xunit.Abstractions;

    public class LeadToolsOpticalRecognizerTests
    {
        private readonly ITestOutputHelper output;
        private readonly LeadToolsLicenseApplier licenseApplier;
        private LeadToolsOpticalRecognizer opticalRecognizer;

        public LeadToolsOpticalRecognizerTests(ITestOutputHelper output)
        {
            this.output = output;
            licenseApplier = new LeadToolsLicenseApplier();
        }

        [Theory]
        [InlineData("1", "825075")]
        [InlineData("2", "826705")]
        [InlineData("3", "825205")]
        [InlineData("4", "825165")]
        [InlineData("5", "825676")]
        [InlineData("6", "825672")]
        [InlineData("7", "825545")]
        [InlineData("8", "825475")]
        [InlineData("9", "826744")]
        [InlineData("10", "826274")]
        [InlineData("11", "825941")]
        //[InlineData("12", "857797")]
        [InlineData("13", "828645")]
        [InlineData("14", "828964")]
        [InlineData("15", "828744")]
        [InlineData("16", "828745")]
        [InlineData("17", "829270")]
        [InlineData("18", "830560")]
        [InlineData("19", "832148")]
        [InlineData("20", "830539")]
        [InlineData("21", "830429")]
        [InlineData("22", "830644")]
        [InlineData("23", "830754")]
        [InlineData("24", "830289")]
        [InlineData("25", "827264")]
        [InlineData("26", "830292")]
        [InlineData("27", "827496")]
        [InlineData("28", "830326")]
        [InlineData("29", "830329")]
        [InlineData("30", "832144")]
        [InlineData("31", "832145")]
        [InlineData("32", "832146")]
        //[InlineData("33", "831474 ")]
        //[InlineData("34", "827496")]
        public void TestUniqueBarcode(string fileName, string expected)
        {
            var sut = GetSut();
            var bitmapSource = LoadImage($"Barcodes\\{fileName}.jpg");
            var numericStringFilter = new NumericStringFilter { MinLength = 6, MaxLength = 6 };
            var recognizedPage = sut.Recognize(
                bitmapSource,
                RecognitionConfiguration.FromSingleImage(bitmapSource, numericStringFilter, Symbology.Barcode));
            var uniqueZone = recognizedPage.RecognizedZones.First();

            Assert.Equal(expected, uniqueZone.RecognizedText);
        }

        private LeadToolsOpticalRecognizer GetSut()
        {
            return opticalRecognizer ?? (opticalRecognizer = new LeadToolsOpticalRecognizer(licenseApplier));
        }

        [Theory]
        [InlineData("1", "825075")]
        [InlineData("2", "826705")]
        [InlineData("3", "825205")]
        [InlineData("4", "825165")]
        [InlineData("5", "825676")]
        [InlineData("6", "825672")]
        [InlineData("7", "825545")]
        [InlineData("8", "825475")]
        [InlineData("9", "826744")]
        [InlineData("10", "826274")]
        [InlineData("11", "825941")]
        //[InlineData("12", "857797")]
        [InlineData("13", "828645")]
        [InlineData("14", "828964")]
        [InlineData("15", "828744")]
        [InlineData("16", "828745")]
        [InlineData("17", "829270")]
        [InlineData("18", "830560")]
        //[InlineData("19", "832148")]
        [InlineData("20", "830539")]
        [InlineData("21", "830429")]
        [InlineData("22", "830644")]
        [InlineData("23", "830754")]
        [InlineData("24", "830289")]
        [InlineData("25", "827264")]
        //[InlineData("26", "830292")]
        [InlineData("27", "827496")]
        [InlineData("28", "830326")]
        [InlineData("29", "830329")]
        [InlineData("30", "832144")]
        [InlineData("31", "832145")]
        [InlineData("32", "832146")]
        public void TestNumericField(string fileName, string expected)
        {
            var sut = GetSut();
            var bitmapSource = LoadImage($"Barcodes\\{fileName}.jpg");
            var numericStringFilter = new NumericStringFilter { MinLength = 6, MaxLength = 6 };
            var recognizedPage = sut.Recognize(
                bitmapSource,
                RecognitionConfiguration.FromSingleImage(bitmapSource, numericStringFilter, Symbology.Text));
            var uniqueZone = recognizedPage.RecognizedZones.First();
            Assert.Equal(expected, uniqueZone.RecognizedText);
        }

        private static BitmapSource LoadImage(string s)
        {
            return new BitmapImage(new Uri(s, UriKind.Relative));
        }
    }
}