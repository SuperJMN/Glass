namespace Glass.Imaging.Recognition.LeadTools.Tests
{
    using Glass.LeadTools.Recognition;
    using Recognition.Tests;
    using Xunit.Abstractions;

    public class LeadToolsOcrEngineTests : OcrEngineTest
    {
        protected override IImageToTextConverter Engine { get; } = new LeadToolsZoneBasedOcrService(new LeadToolsLicenseApplier());

        public LeadToolsOcrEngineTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override double AlphaNumericSuccessRate => 0;
        protected override double NumericSuccessRate => 1;
    }
}
