namespace Glass.LeadTools.Recognition.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Imaging.Core;
    using Imaging.FullFx;
    using Imaging.PostProcessing;
    using Imaging.ZoneConfigurations;
    using Imaging.ZoneConfigurations.Alphanumeric;
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
            ImagingContext.BitmapOperations = new BitmapOperations();
        }

        [Fact]
        public void EmptyBitmapGivesEmptyResult()
        {
            var sut = GetSut();
            var numericStringFilter = new NumericStringFilter { MinLength = 6, MaxLength = 6 };
            var bmp = GetEmptyBitmap();
            var recognizedPage = sut.Recognize(bmp, RecognitionConfiguration.FromSingleImage(bmp, numericStringFilter, Symbology.Barcode));

            Assert.Null(recognizedPage.RecognizedZones.First().RecognizedText);
        }

        private static WriteableBitmap GetEmptyBitmap()
        {
            return new WriteableBitmap(10, 10, 96, 96, PixelFormats.Bgr24, new BitmapPalette(new List<Color> { Color.FromRgb(0, 0, 0) }));
        }

        [Theory]
        [ClassData(typeof(BarcodeTestDataProvider))]
        public void CroppedBarcode(BitmapSource image, string expected)
        {
            var sut = GetSut();
            var numericStringFilter = new NumericStringFilter { MinLength = 6, MaxLength = 6 };
            var recognizedPage = sut.Recognize(
                image,
                RecognitionConfiguration.FromSingleImage(image, numericStringFilter, Symbology.Barcode));

            var uniqueZone = recognizedPage.RecognizedZones.FirstOrDefault();

            Assert.NotNull(uniqueZone);
            Assert.Equal(expected, uniqueZone.RecognizedText);
        }

        [Theory(Skip = "Sólo para lote")]
        [ClassData(typeof(BulkBarcodeTestFilesProvider))]
        public void BulkUniqueBarcode(BitmapSource image)
        {
            var sut = GetSut();
            var numericStringFilter = new NumericStringFilter { MinLength = 6, MaxLength = 6 };
            var recognizedPage = sut.Recognize(
                image,
                RecognitionConfiguration.FromSingleImage(image, numericStringFilter, Symbology.Barcode));

            var uniqueZone = recognizedPage.RecognizedZones.First();
            output.WriteLine(uniqueZone.RecognizedText);
            Assert.Equal(6, uniqueZone.RecognizedText.Length);
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
            var numericStringFilter = new NumericStringFilter { MinLength = 6, MaxLength = 6 };
            var recognizedPage = sut.Recognize(
                image,
                RecognitionConfiguration.FromSingleImage(image, numericStringFilter, Symbology.Text));
            var uniqueZone = recognizedPage.RecognizedZones.First();
            Assert.Equal(expected, uniqueZone.RecognizedText);
        }

        [Theory(Skip = "Porque ni uno funciona, copón")]
        [ClassData(typeof(TextTestDataProvider))]
        public void AlphaNumeric(BitmapSource image, string expected)
        {
            var sut = GetSut();
            var stringFilter = new AlphanumericStringFilter();
            var recognizedPage = sut.Recognize(image, RecognitionConfiguration.FromSingleImage(image, stringFilter, Symbology.Text));
            var uniqueZone = recognizedPage.RecognizedZones.First();
            Assert.Equal(expected, uniqueZone.RecognizedText);
        }
    }

    public abstract class TestFilesProvider : IEnumerable<object[]>
    {
        protected TestFilesProvider(string path)
        {
            pathToFiles = path;
        }

        private readonly string pathToFiles;

        private IEnumerable<object[]> data
        {
            get
            {
                return from path in Directory.GetFiles(pathToFiles)
                       let filename = Path.GetFileNameWithoutExtension(path)
                       let expected = filename.Replace("!", "").Replace("-", "")
                       where !filename.Contains(IgnoreChar)
                       select new object[] { LoadImage(path), expected };
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

        public BarcodeTestDataProvider() : base("Barcodes")
        {
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

        public NumericTestDataProvider() : base("Barcodes")
        {
        }
    }

    internal class TextTestDataProvider : TestFilesProvider
    {
        protected override char IgnoreChar
        {
            get
            {
                return '!';
            }
        }

        public TextTestDataProvider() : base("Text")
        {
        }
    }
}