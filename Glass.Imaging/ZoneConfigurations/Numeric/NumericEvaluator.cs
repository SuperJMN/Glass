namespace Glass.Imaging.ZoneConfigurations.Numeric
{
    using System.Linq;

    public class NumericEvaluator : Evaluator, IEvaluator
    {
        public NumericEvaluator(StringFilter stringFilter) : base(stringFilter)
        {
        }

        protected override int GetValidChars(string s)
        {
            return s.Count(char.IsNumber);
        }
    }
}