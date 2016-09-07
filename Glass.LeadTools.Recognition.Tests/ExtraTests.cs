namespace Glass.Imaging.Recognition.Tests
{
    using System;
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

        [Theory]
        [InlineData("61703", @"Images\Texts\61703.jpg", 6, 6, Symbology.Text, "Numeric")]
        public void SpecificTest(string result, string pathToImage, int min, int max, Symbology symbology, string filterType)
        {
            StringFilter filter = GetFilter(filterType, min, max);
            var sut = GetSut();
            var bmp = LoadImage(pathToImage);
            var recognizedPage = sut.Recognize(bmp, RecognitionConfiguration.FromSingleImage(bmp, filter, symbology));
            var recognitionResult = recognizedPage.RecognizedZones.First().RecognitionResult;
            Assert.Equal(result, recognitionResult.Text);
        }

        private StringFilter GetFilter(string filterType, int min, int max)
        {
            switch (filterType)
            {
                case "Numeric":
                    return new NumericStringFilter() {MaxLength = max, MinLength = min,};
                default:
                    throw new ArgumentException();
            }
        }
    }
}