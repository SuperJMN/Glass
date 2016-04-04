namespace Glass.Imaging.Recognition.LeadTools.Tests
{
    using Glass.LeadTools.Recognition;
    using Imaging;
    using Recognition.Tests;
    using Xunit.Abstractions;

    public class LeadToolsBarcodeEngineTests : BarcodeEngineTest
    {
        protected override IImageToTextConverter Engine { get; } = new LeadToolsZoneBasedBarcodeReader(new LeadToolsLicenseApplier());

        public LeadToolsBarcodeEngineTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override double AlphanumericSuccessRate => 0.1;
        protected override double NumericSuccessRate => 0.8;
    }
}