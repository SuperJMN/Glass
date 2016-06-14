namespace Glass.Imaging.Recognition.MessagingToolkit.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Windows.Media.Imaging;
    using Barcodes.MessagingToolkit;
    using Xunit;
    using Xunit.Abstractions;
    using ZoneConfigurations;
    using ZoneConfigurations.Alphanumeric;

    public class SpecificTests
    {
        private readonly ITestOutputHelper output;

        public SpecificTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Batch()
        {
            foreach (var bitmap in Directory.EnumerateFiles("Batch").Select(GetBitmap))
            {
                var sut = new MessagingToolkitZoneBasedBarcodeReader();
                var recognition = sut.Recognize(bitmap, ZoneConfiguration.FromSingleImage(bitmap, new AlphanumericStringFilter(), Symbology.Barcode));
                foreach (var recognitionResult in recognition)
                {
                    output.WriteLine(recognitionResult.Text ?? "{Empty result}");
                }
            }
            
        }

        private BitmapSource GetBitmap(string path)
        {
            return new BitmapImage(new Uri(path, UriKind.Relative));
        }
    }
}