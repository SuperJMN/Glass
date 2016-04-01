namespace Glass.Imaging.Recognition.Tests
{
    using Imaging;
    using Ocr.Tesseract;
    using Xunit.Abstractions;

    public class TesseractEngineTests : OcrEngineTest
    {
        protected override IImageToTextConverter GetSut()
        {
            return new TesseractOcrOcrService();
        }

        public TesseractEngineTests(ITestOutputHelper output) : base(output)
        {
        }
    }
}