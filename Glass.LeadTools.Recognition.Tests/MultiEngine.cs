namespace Glass.Imaging.Recognition.Tests
{
    using System.Windows.Media.Imaging;
    using DataProviders;
    using DataProviders.Barcode;
    using DataProviders.Text;
    using Imaging.ZoneConfigurations;
    using Imaging.ZoneConfigurations.Alphanumeric;
    using Imaging.ZoneConfigurations.Numeric;
    using Xunit;
    using Xunit.Abstractions;

    public class MultiEngine : MultiEngineTestBase
    {
        private readonly ITestOutputHelper output;
        
        public MultiEngine(ITestOutputHelper output)
        {
            this.output = output;                       
        }

        [Theory]
        [ClassData(typeof(NumericBarcodeTestDataProvider))]
        public void NumericBarcode(BitmapSource image, string expected)
        {
            Assert.Equal(expected, Extract(image, new NumericStringFilter { MinLength = 6, MaxLength = 6 }, Symbology.Barcode));
        }

        [Theory(Skip = "Not working")]
        [ClassData(typeof(AlphanumericBarcodeTestDataProvider))]
        public void AlphanumericBarcode(BitmapSource image, string expected)
        {
            Assert.Equal(expected, Extract(image, new AlphanumericStringFilter {MinLength = 12, MaxLength = 13}, Symbology.Barcode));
        }

        [Theory]
        [ClassData(typeof(NumericTestDataProvider))]
        public void Numeric(BitmapSource image, string expected)
        {
            Assert.Equal(expected, Extract(image, new NumericStringFilter { MinLength = 6, MaxLength = 6, }, Symbology.Text));
        }

        [Theory]
        [ClassData(typeof(TextTestDataProvider))]
        public void AlphaNumeric(BitmapSource image, string expected)
        {
            Assert.Equal(expected, Extract(image, new AlphanumericStringFilter(), Symbology.Text));
        }

        [Theory(Skip = "Sólo para lote")]
        [ClassData(typeof(BulkBarcodeTestFilesProvider))]
        public void BulkUniqueBarcode(BitmapSource image)
        {            
            var extract = Extract(image, new NumericStringFilter { MinLength = 6, MaxLength = 6 }, Symbology.Barcode);
            Assert.Equal(extract, extract);
            output.WriteLine(extract);
            Assert.Equal(6, extract.Length);
        }
    }
}