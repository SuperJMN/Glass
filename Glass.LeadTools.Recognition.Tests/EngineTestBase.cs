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

        private string ExtractBestTextCandidate(BitmapSource bitmap, ITextualDataFilter filter, Symbology symbology)
        {
            var bounds = new Rect(0, 0, bitmap.Width, bitmap.Height);
            var zoneConfiguration = new ZoneConfiguration {Bounds = bounds, TextualDataFilter = filter, Id = "", Symbology = symbology};


            var scores = from text in Engine.Recognize(bitmap, zoneConfiguration)
                let filteredText = filter.Filter(text)
                let score = filter.Evaluator.GetScore(text)
                select new {FilteredText = filteredText, Score = score};

            var selected = scores.OrderByDescending(arg => arg.Score).First();

            return selected.FilteredText;
        }

        protected void AssertSuccessRate(IEnumerable<TestCase> testCases, ITextualDataFilter stringFilter, double minimumAcceptable, Symbology symbology)
        {
            var cases = testCases as IList<TestCase> ?? testCases.ToList();

            var testExecutions = (from c in cases
                let result = ExtractBestTextCandidate(c.Bitmap, stringFilter, symbology)
                select new {Result = result, Expected = c.Expected, Success = result == c.Expected}).ToList();

            foreach (var testExecution in testExecutions)
            {
                var isSuccess = testExecution.Success ? "OK" : "FAILED";
                output.WriteLine($"{isSuccess}: Expected: {testExecution.Expected} Result: {testExecution.Result}");
            }

            var success = testExecutions.Count(testCase => testCase.Expected == testCase.Result);
            var total = testExecutions.Count();

            var d = (double) success/total;
            output.WriteLine($"Success Ratio: {d}");
            Assert.True(d >= minimumAcceptable);
        }
    }

    public class TestCase
    {
        public BitmapSource Bitmap { get; set; }
        public string Expected { get; set; }
    }
}