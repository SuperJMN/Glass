namespace Glass.Imaging.Recognition.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using PostProcessing;
    using ZoneConfigurations;
    using ZoneConfigurations.Numeric;
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

            var recognitionResult = recognizedPage.RecognizedZones.First().RecognitionResult;
            Assert.Null(recognitionResult.Text);
        }

        private static WriteableBitmap GetEmptyBitmap()
        {
            return new WriteableBitmap(100, 100, 96, 96, PixelFormats.Bgr32, new BitmapPalette(new List<Color> { Color.FromRgb(0, 0, 0) }));
        }      
    }
}