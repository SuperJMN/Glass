namespace Glass.Barcodes.ZXing.Tests
{
    using Imaging;
    using Imaging.Recognition.Tests;
    using Xunit.Abstractions;

    public class ZXingBarcodeReaderTests : BarcodeEngineTest
    {
        protected override IImageToTextConverter Engine { get; } = new ZXingBarcodeReader();

        public ZXingBarcodeReaderTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override double AlphanumericSuccessRate => 0.1;
        protected override double NumericSuccessRate => 1;
    }
}
