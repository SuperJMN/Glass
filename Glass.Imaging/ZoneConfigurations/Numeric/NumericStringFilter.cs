namespace Glass.Imaging.ZoneConfigurations.Numeric
{
    using PostProcessing;

    public class NumericStringFilter : StringFilter
    {
        public NumericStringFilter()
        {
            Evaluator = new NumericEvaluator(this);
        }     

        public override FilterType FilterType => FilterType.Digits;
        public override IEvaluator Evaluator { get; }
        public decimal Maximum { get; set; } = decimal.MaxValue;
        public decimal Minimum { get; set; } = decimal.MinValue;
    }
}