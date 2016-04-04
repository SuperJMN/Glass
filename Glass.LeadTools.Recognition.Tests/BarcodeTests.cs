namespace Glass.Imaging.Recognition.Tests
{
    using DataProviders.Barcode;
    using Xunit;
    using Xunit.Abstractions;
    using ZoneConfigurations;
    using ZoneConfigurations.Alphanumeric;
    using ZoneConfigurations.Numeric;

    public class BarcodeTests : MultiEngineTest
    {
        public BarcodeTests(ITestOutputHelper output) : base(output)
        {
        }

        protected double AlphanumericSuccessRate => 0.1;

        protected double NumericSuccessRate => 1;

        [Fact]
        public void Alphanumeric()
        {
            AssertSuccessRate(
                new AlphanumericBarcodeTestCases(),
                new AlphanumericStringFilter { MinLength = 12, MaxLength = 13 },
                AlphanumericSuccessRate,
                Symbology.Barcode);
        }

        [Fact]
        public void Numeric()
        {
            AssertSuccessRate(new NumericBarcodeTestCases(), new NumericStringFilter { MinLength = 6, MaxLength = 6 }, NumericSuccessRate, Symbology.Barcode);
        }
    }
}