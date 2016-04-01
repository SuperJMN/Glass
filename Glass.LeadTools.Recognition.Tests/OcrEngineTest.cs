namespace Glass.Imaging.Recognition.Tests
{
    using System.Windows.Media.Imaging;
    using DataProviders.Text;
    using Imaging.ZoneConfigurations;
    using Imaging.ZoneConfigurations.Alphanumeric;
    using Imaging.ZoneConfigurations.Numeric;
    using Xunit;
    using Xunit.Abstractions;

    public abstract class OcrEngineTest : EngineTestBase
    {
        [Theory]
        [ClassData(typeof(NumericTestDataProvider))]
        public void Numeric(BitmapSource image, string expected)
        {
            Assert.Equal(expected, ExtractFirstFiltered(image, new NumericStringFilter { MinLength = 6, MaxLength = 6, }, Symbology.Text));
        }

        [Theory]
        [ClassData(typeof(TextTestDataProvider))]
        public void AlphaNumeric(BitmapSource image, string expected)
        {
            Assert.Equal(expected, ExtractFirstFiltered(image, new AlphanumericStringFilter(), Symbology.Text));
        }

        protected OcrEngineTest(ITestOutputHelper output) : base(output)
        {
        }
    }
}