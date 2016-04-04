namespace Glass.Imaging.Recognition.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using Barcodes.MessagingToolkit;
    using Core;
    using FullFx;
    using LeadTools.Recognition;
    using PostProcessing;
    using Tesseract;
    using Xunit;
    using Xunit.Abstractions;
    using ZoneConfigurations;

    public class MultiEngineTest
    {
        private readonly ITestOutputHelper output;
        private readonly ILeadToolsLicenseApplier licenseApplier = new LeadToolsLicenseApplier();
        private IZoneBasedRecognitionService opticalRecognizer;

        protected MultiEngineTest(ITestOutputHelper output)
        {
            this.output = output;
            ImagingContext.BitmapOperations = new BitmapOperations();
        }

        private IZoneBasedRecognitionService Engine
        {
            get
            {
                var ocrEngines = new List<IImageToTextConverter> { new TesseractOcrOcrService(), new LeadToolsZoneBasedOcrService(licenseApplier) };
                var barcodeEngines = new List<IImageToTextConverter> { new MessagingToolkitZoneBasedBarcodeReader(), new LeadToolsZoneBasedBarcodeReader(licenseApplier) };
                return opticalRecognizer ?? (opticalRecognizer = new CompositeOpticalRecognizer(ocrEngines.Concat(barcodeEngines)));  }
        }

        private string ExtractBestTextCandidate(BitmapSource bitmap, ITextualDataFilter filter, Symbology symbology)
        {
            var page = Engine.Recognize(bitmap, RecognitionConfiguration.FromSingleImage(bitmap, filter, symbology));
            var zone = page.RecognizedZones.First();

            return zone.RecognitionResult.Text;
        }

        protected void AssertSuccessRate(IEnumerable<TestCase> testCases, ITextualDataFilter stringFilter, double minimum, Symbology symbology)
        {
            var cases = testCases as IList<TestCase> ?? testCases.ToList();

            var testExecutions = (from c in cases
                let result = OutputResult(ExtractBestTextCandidate(c.Bitmap, stringFilter, symbology), c)
                select new {Result = result, Expected = c.Expected, Success = result == c.Expected}).ToList();

            var success = testExecutions.Count(testCase => testCase.Expected == testCase.Result);
            var total = testExecutions.Count;

            var d = (double) success/total;
            output.WriteLine($"Success Ratio: {d}");
            Assert.True(d >= minimum);
        }

        private string OutputResult(string result, TestCase testCase)
        {
            var isSuccess = testCase.Expected==result ? "OK" : "FAILED";
            output.WriteLine($"{isSuccess}: Expected: {testCase.Expected} Result: {result}");
            return result;
        }
    }
}