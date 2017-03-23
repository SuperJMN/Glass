namespace Glass.Imaging.Recognition.MessagingToolkit.Tests
{
    using Barcodes.Inlite;
    using Imaging;
    using Recognition.Tests;
    using Xunit.Abstractions;

    public class Inlite : BarcodeEngineTest
    {
        protected override IImageToTextConverter Engine { get; } = new ClearImageBarcodeEngine();

        public Inlite(ITestOutputHelper output) : base(output)
        {
        }

        protected override double AlphanumericSuccessRate => 0.1;
        protected override double NumericSuccessRate => 1;
    }
}
