namespace Glass.Imaging.Recognition.Tests
{
    using DataProviders.Barcode;
    using DataProviders.Text;
    using Xunit;
    using Xunit.Abstractions;
    using ZoneConfigurations;
    using ZoneConfigurations.Alphanumeric;
    using ZoneConfigurations.Numeric;

    public class OcrTests : MultiEngineTest
    {
        public OcrTests(ITestOutputHelper output) : base(output)
        {
        }

        private double AlphanumericSuccessRate => 0;

        private double NumericSuccessRate => 0.944;

        [Fact]
        public void Alphanumeric()
        {
            AssertSuccessRate(new AlphanumericTestCases(), new AlphanumericStringFilter { MinLength = 6, MaxLength = 6 }, AlphanumericSuccessRate, Symbology.Text);
        }

        [Fact]
        public void Numeric()
        {
            AssertSuccessRate(new NumericTestCases(), new NumericStringFilter { MinLength = 6, MaxLength = 6 }, NumericSuccessRate, Symbology.Text);
        }
    }
}