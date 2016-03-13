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
    using JetBrains.dotMemoryUnit;
    using Xunit.Abstractions;

    public class LeadToolsOpticalRecognizerTests
    {
        private readonly LeadToolsLicenseApplier licenseApplier;


        public LeadToolsOpticalRecognizerTests()
        {
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
        public void TestUniqueBarcode(string fileName, string expected)
        {
            var sut = new LeadToolsOpticalRecognizer(licenseApplier);
            var bitmapSource = LoadImage($"Barcodes\\{fileName}.jpg");
            var numericStringFilter = new NumericStringFilter { MinLength = 6, MaxLength = 6 };
            var recognizedPage = sut.Recognize(
                bitmapSource,
                RecognitionConfiguration.FromSingleImage(bitmapSource, numericStringFilter, Symbology.Barcode));
            var uniqueZone = recognizedPage.RecognizedZones.First();
            Assert.Equal(expected, uniqueZone.RecognizedText);
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
        public void TestNumericField(string fileName, string expected)
        {
            var sut = new LeadToolsOpticalRecognizer(licenseApplier);
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