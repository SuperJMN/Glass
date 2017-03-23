namespace Glass.Imaging.Recognition.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Core;
    using DataProviders;
    using DotImaging;
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

            var recognitionResult = recognizedPage.RecognizedZones.First().RecognitionResult;
            Assert.Null(recognitionResult.Text);
        }

        private static IImage GetEmptyBitmap()
        {
            var writeableBitmap = new WriteableBitmap(10, 10, 96, 96, PixelFormats.Bgr24, new BitmapPalette(new List<Color> { Color.FromRgb(0, 0, 0) }));
            var bmpSrc = Extensions.ConvertWriteableBitmapToBitmapImage(writeableBitmap);
            Bgra<byte>[,] colorImg = bmpSrc.ToArray<Bgra<byte>>();
            return colorImg.Lock();
        }
    }
}