namespace Glass.LeadTools.Recognition.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
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
        private readonly LeadToolsLicenseApplier licenseApplier;
        private LeadToolsOpticalRecognizer opticalRecognizer;

        public LeadToolsOpticalRecognizerTests(ITestOutputHelper output)
        {
            licenseApplier = new LeadToolsLicenseApplier();
        }

        [Theory]
        [ClassData(typeof(BarcodeTestDataProvider))]
        public void TestUniqueBarcode(BitmapSource image, string expected)
        {
            var sut = GetSut();
            var numericStringFilter = new NumericStringFilter {MinLength = 6, MaxLength = 6};
            var recognizedPage = sut.Recognize(
                image,
                RecognitionConfiguration.FromSingleImage(image, numericStringFilter, Symbology.Barcode));
            var uniqueZone = recognizedPage.RecognizedZones.First();

            Assert.Equal(expected, uniqueZone.RecognizedText);
        }

        private LeadToolsOpticalRecognizer GetSut()
        {
            return opticalRecognizer ?? (opticalRecognizer = new LeadToolsOpticalRecognizer(licenseApplier));
        }

        [Theory]
        [ClassData(typeof(NumericTestDataProvider))]
        public void TestNumericField(BitmapSource image, string expected)
        {
            var sut = GetSut();
            var numericStringFilter = new NumericStringFilter {MinLength = 6, MaxLength = 6};
            var recognizedPage = sut.Recognize(
                image,
                RecognitionConfiguration.FromSingleImage(image, numericStringFilter, Symbology.Text));
            var uniqueZone = recognizedPage.RecognizedZones.First();
            Assert.Equal(expected, uniqueZone.RecognizedText);
        }

        private static BitmapSource LoadImage(string s)
        {
            return new BitmapImage(new Uri(s, UriKind.Relative));
        }
    }

    public abstract class TestFilesProvider : IEnumerable<object[]>
    {
        private IEnumerable<object[]> data {
            get
            {
                return from path in Directory.GetFiles("Barcodes")
                       let filename = Path.GetFileNameWithoutExtension(path)
                       let expected = filename.Replace("!", "").Replace("-", "")
                       where !filename.Contains(IgnoreChar)
                       select new object[] {LoadImage(path), expected };
            }
        }

        protected abstract char IgnoreChar { get; }

        private static BitmapSource LoadImage(string s)
        {
            return new BitmapImage(new Uri(s, UriKind.Relative));
        }

        public IEnumerator<object[]> GetEnumerator()
        {
            return data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal class BarcodeTestDataProvider : TestFilesProvider
    {
        protected override char IgnoreChar
        {
            get
            {
                return '-';
            }
        }  
    }

    internal class NumericTestDataProvider : TestFilesProvider
    {
        protected override char IgnoreChar
        {
            get
            {
                return '!';
            }
        }
    }
}