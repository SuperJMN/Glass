namespace Glass.Imaging.Recognition.LeadTools.Tests
{
    using Glass.LeadTools.Recognition;
    using Recognition.Tests;
    using Xunit.Abstractions;

    public class LeadToolsEngineTests : OcrEngineTest
    {
        protected override IImageToTextConverter GetSut()
        {
            return new LeadToolsZoneBasedOcrService(new LeadToolsLicenseApplier());
        }

        public LeadToolsEngineTests(ITestOutputHelper output) : base(output)
        {
        }
    }
}
