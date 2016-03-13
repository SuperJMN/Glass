namespace Glass.Imaging.ZoneConfigurations.Alphanumeric
{
    using PostProcessing;

    public class AlphanumericStringFilter : StringFilter
    {
        public AlphanumericStringFilter()
        {
            Evaluator = new AlphanumericEvaluator(this);
        }

        public override FilterType FilterType => FilterType.Alpha;
        public override IEvaluator Evaluator { get; } 
    }
}