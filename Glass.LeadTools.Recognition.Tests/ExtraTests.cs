namespace Glass.Imaging.Recognition.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using DataProviders;
    using Imaging.PostProcessing;
    using Imaging.ZoneConfigurations;
    using Imaging.ZoneConfigurations.Numeric;
    using Xunit;
    using Xunit.Abstractions;

    public class ExtraTests : MultiEngineTestBase
    {
        private readonly ITestOutputHelper output;
        
        public ExtraTests(ITestOutputHelper output)
        {
            this.output = output;                       
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
    }
}