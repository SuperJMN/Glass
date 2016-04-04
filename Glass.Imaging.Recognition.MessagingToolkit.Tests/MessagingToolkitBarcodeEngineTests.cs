namespace Glass.Imaging.Recognition.MessagingToolkit.Tests
{
    using Barcodes.MessagingToolkit;
    using Imaging;
    using Recognition.Tests;
    using Xunit.Abstractions;

    public class MessagingToolkitBarcodeEngineTests : BarcodeEngineTest
    {
        protected override IImageToTextConverter Engine { get; } = new MessagingToolkitZoneBasedBarcodeReader();

        public MessagingToolkitBarcodeEngineTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override double AlphanumericSuccessRate => 0.1;
        protected override double NumericSuccessRate => 1;
    }
}