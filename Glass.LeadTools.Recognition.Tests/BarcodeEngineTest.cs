namespace Glass.Imaging.Recognition.Tests
{
    using System.Windows.Media.Imaging;
    using DataProviders.Barcode;
    using ZoneConfigurations;
    using ZoneConfigurations.Alphanumeric;
    using ZoneConfigurations.Numeric;
    using Xunit;
    using Xunit.Abstractions;

    public abstract class BarcodeEngineTest : EngineTestBase
    {
        [Theory]
        [ClassData(typeof(NumericBarcodeTestDataProvider))]
        public void NumericBarcode(BitmapSource image, string expected)
        {
            Assert.Equal(expected, ExtractFirstFiltered(image, new NumericStringFilter { MinLength = 6, MaxLength = 6 }, Symbology.Barcode));
        }

        [Theory]
        [ClassData(typeof(AlphanumericBarcodeTestDataProvider))]
        public void AlphanumericBarcode(BitmapSource image, string expected)
        {
            Assert.Equal(expected, ExtractFirstFiltered(image, new AlphanumericStringFilter { MinLength = 12, MaxLength = 13 }, Symbology.Barcode));
        }

        public BarcodeEngineTest(ITestOutputHelper output) : base(output)
        {
        }
    }
}