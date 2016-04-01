namespace Glass.Imaging.Recognition.Tests
{
    using Imaging.PostProcessing;
    using Imaging.ZoneConfigurations;
    using Imaging.ZoneConfigurations.Numeric;
    using JetBrains.dotMemoryUnit;
    using Leadtools;
    using Leadtools.Forms.Ocr.Advantage;
    using Xunit;
    using Xunit.Abstractions;

    public class LeadToolsOpticalRecognizerMemoryTests : MultiEngineTestBase
    {
        public LeadToolsOpticalRecognizerMemoryTests(ITestOutputHelper output)
        {
            DotMemoryUnitTestOutput.SetOutputMethod(output.WriteLine);
        }

        [Fact]
        [DotMemoryUnit(FailIfRunWithoutSupport = false)]
        public void MemoryTest()
        {
            var sut = GetSut();
            var bitmapSource = LoadImage($"Images\\Barcodes\\Numeric\\802492.jpg");
            var numericStringFilter = new NumericStringFilter { MinLength = 6, MaxLength = 6 };
            var recognizedPage = sut.Recognize(
                bitmapSource,
                RecognitionConfiguration.FromSingleImage(bitmapSource, numericStringFilter, Symbology.Text));


            dotMemory.Check(
                memory =>
                {
                    Assert.Equal(0, memory.GetObjects(where => where.Type.Is<OcrPage>()).ObjectsCount);
                    Assert.Equal(0, memory.GetObjects(where => where.Type.Is<RasterImage>()).ObjectsCount);
                });

        }


    }
}