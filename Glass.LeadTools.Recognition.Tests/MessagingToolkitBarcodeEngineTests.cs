namespace Glass.Imaging.Recognition.Tests
{
    using Barcodes.MessagingToolkit;
    using Imaging;
    using Xunit.Abstractions;

    public class MessagingToolkitBarcodeEngineTests : BarcodeEngineTest
    {
        protected override IImageToTextConverter GetSut()
        {
            return new MessagingToolkitZoneBasedBarcodeReader();
        }

        public MessagingToolkitBarcodeEngineTests(ITestOutputHelper output) : base(output)
        {
        }
    }
}