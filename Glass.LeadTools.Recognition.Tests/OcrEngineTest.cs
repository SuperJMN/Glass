namespace Glass.Imaging.Recognition.Tests
{
    using System.Linq;
    using DataProviders.Text;
    using Xunit;
    using Xunit.Abstractions;
    using ZoneConfigurations;
    using ZoneConfigurations.Alphanumeric;
    using ZoneConfigurations.Numeric;

    public abstract class OcrEngineTest : EngineTestBase
    {
        protected OcrEngineTest(ITestOutputHelper output) : base(output)
        {
        }

        protected abstract double AlphaNumericSuccessRate { get; }

        protected abstract double NumericSuccessRate { get; }

        [Fact]
        public void Alphanumeric()
        {
            AssertSuccessRate(new AlphanumericTestCases().Skip(2).Take(1), new AlphanumericStringFilter {MinLength = 6, MaxLength = 6}, AlphaNumericSuccessRate, Symbology.Text);
        }

        [Fact]
        public void Numeric()
        {
            AssertSuccessRate(new NumericTestCases().Where((c, i) => i % 2 == 0), new NumericStringFilter {MinLength = 6, MaxLength = 6}, NumericSuccessRate, Symbology.Text);
        }
    }
}