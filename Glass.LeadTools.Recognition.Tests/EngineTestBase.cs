namespace Glass.Imaging.Recognition.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using Core;
    using FullFx;
    using Xunit;
    using Xunit.Abstractions;
    using ZoneConfigurations;

    public abstract class EngineTestBase
    {
        private readonly ITestOutputHelper output;

        protected EngineTestBase(ITestOutputHelper output)
        {
            this.output = output;
            ImagingContext.BitmapOperations = new BitmapOperations();
        }

        protected abstract IImageToTextConverter Engine { get; }

        protected string ExtractBestTextCandidate(BitmapSource bitmap, ITextualDataFilter filter, Symbology symbology)
        {
            var bounds = new Rect(0, 0, bitmap.Width, bitmap.Height);
            var zoneConfiguration = new ZoneConfiguration {Bounds = bounds, TextualDataFilter = filter, Id = "", Symbology = symbology};

            var recognitions = Engine.Recognize(bitmap, zoneConfiguration);

            var selector = OpticalResultSelector.ChooseBest(recognitions, zoneConfiguration);

            return selector?.Text;
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

    public class TestCase
    {
        public BitmapSource Bitmap { get; set; }
        public string Expected { get; set; }
    }
}