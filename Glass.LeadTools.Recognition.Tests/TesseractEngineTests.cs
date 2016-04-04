namespace Glass.Imaging.Recognition.Tests
{
    using Imaging;
    using Ocr.Tesseract;
    using Xunit.Abstractions;

    public class TesseractEngineTests : OcrEngineTest
    {
        protected override IImageToTextConverter Engine { get; } = new TesseractOcrOcrService();

        public TesseractEngineTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override double AlphaNumericSuccessRate => 0.3;
        protected override double NumericSuccessRate => 0.6;
    }
}