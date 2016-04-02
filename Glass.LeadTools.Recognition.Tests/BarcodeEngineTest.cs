namespace Glass.Imaging.Recognition.Tests
{
    using DataProviders.Barcode;
    using Xunit;
    using Xunit.Abstractions;
    using ZoneConfigurations;
    using ZoneConfigurations.Alphanumeric;
    using ZoneConfigurations.Numeric;

    public abstract class BarcodeEngineTest : EngineTestBase
    {
        protected BarcodeEngineTest(ITestOutputHelper output) : base(output)
        {
        }

        protected abstract double AlphanumericSuccessRate { get; }

        protected abstract double NumericSuccessRate { get; }

        [Fact]
        public void Alphanumeric()
        {
            AssertSuccessRate(
                new AlphanumericBarcodeTestCases(),
                new AlphanumericStringFilter {MinLength = 12, MaxLength = 13},
                AlphanumericSuccessRate,
                Symbology.Barcode);
        }

        [Fact]
        public void Numeric()
        {
            AssertSuccessRate(new NumericBarcodeTestCases(), new NumericStringFilter {MinLength = 6, MaxLength = 6}, NumericSuccessRate, Symbology.Barcode);
        }
    }
}