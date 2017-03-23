namespace Glass.Barcodes.Inlite.Tests
{
    using Imaging;
    using Imaging.Recognition.Tests;
    using Xunit.Abstractions;

    public class MessagingToolkitBarcodeEngineTests : BarcodeEngineTest
    {
        protected override IImageToTextConverter Engine { get; } = new InliteBarcodeEngine();

        public MessagingToolkitBarcodeEngineTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override double AlphanumericSuccessRate => 0.1;
        protected override double NumericSuccessRate => 1;
    }
}
