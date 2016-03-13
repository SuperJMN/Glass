namespace Glass.LeadTools.Recognition.Tests
{
    using System;
    using System.Linq;
    using System.Windows.Media.Imaging;
    using Imaging.PostProcessing;
    using Imaging.ZoneConfigurations;
    using Imaging.ZoneConfigurations.Numeric;
    using JetBrains.dotMemoryUnit;
    using Leadtools;
    using Leadtools.Forms.Ocr.Advantage;
    using Xunit;
    using Xunit.Abstractions;

    public class LeadToolsOpticalRecognizerMemoryTests
    {
        private readonly LeadToolsLicenseApplier licenseApplier;


        public LeadToolsOpticalRecognizerMemoryTests(ITestOutputHelper output)
        {
            DotMemoryUnitTestOutput.SetOutputMethod(output.WriteLine);
            licenseApplier = new LeadToolsLicenseApplier();          
        }

        [Fact]
        [DotMemoryUnit(FailIfRunWithoutSupport = false)]
        public void MemoryTest()
        {
            var sut = new LeadToolsOpticalRecognizer(licenseApplier);
            var bitmapSource = LoadImage($"Barcodes\\{"1"}.jpg");
            var numericStringFilter = new NumericStringFilter { MinLength = 6, MaxLength = 6 };
            var recognizedPage = sut.Recognize(
                bitmapSource,
                RecognitionConfiguration.FromSingleImage(bitmapSource, numericStringFilter, Symbology.Text));
            var uniqueZone = recognizedPage.RecognizedZones.First();
            Assert.Equal("825075", uniqueZone.RecognizedText);

            dotMemory.Check(
                memory =>
                {
                    Assert.Equal(0, memory.GetObjects(where => where.Type.Is<OcrPage>()).ObjectsCount);
                    Assert.Equal(0, memory.GetObjects(where => where.Type.Is<RasterImage>()).ObjectsCount);                    
                });

        }

        private static BitmapSource LoadImage(string s)
        {
            return new BitmapImage(new Uri(s, UriKind.Relative));
        }
    }
}