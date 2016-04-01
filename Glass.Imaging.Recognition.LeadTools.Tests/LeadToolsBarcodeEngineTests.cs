namespace Glass.Imaging.Recognition.LeadTools.Tests
{
    using Glass.LeadTools.Recognition;
    using Imaging;
    using Recognition.Tests;
    using Xunit.Abstractions;

    public class LeadToolsBarcodeEngineTests : BarcodeEngineTest
    {
        protected override IImageToTextConverter GetSut()
        {
            return new LeadToolsZoneBasedBarcodeReader(new LeadToolsLicenseApplier());
        }

        public LeadToolsBarcodeEngineTests(ITestOutputHelper output) : base(output)
        {
        }
    }
}