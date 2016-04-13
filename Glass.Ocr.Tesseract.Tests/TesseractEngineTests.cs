namespace Glass.Imaging.Recognition.Tesseract.Tests
{
    using Recognition.Tests;
    using Xunit.Abstractions;

    // ReSharper disable once UnusedMember.Global
    public class TesseractEngineTests : OcrEngineTest
    {
        protected override IImageToTextConverter Engine { get; } = new TesseractOcrOcrService();

        public TesseractEngineTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override double AlphaNumericSuccessRate => 0.33;
        protected override double NumericSuccessRate => 0.6;
    }
}