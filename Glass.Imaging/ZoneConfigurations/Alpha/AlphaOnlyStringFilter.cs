namespace Glass.Imaging.ZoneConfigurations.Alpha
{
    using PostProcessing;

    public class AlphaOnlyStringFilter : StringFilter
    {
        public AlphaOnlyStringFilter()
        {
             Evaluator = new AlphaOnlyEvaluator(this);
        }

        public override FilterType FilterType => FilterType.AlphaOnly;
        public override IEvaluator Evaluator { get; }
    }
}