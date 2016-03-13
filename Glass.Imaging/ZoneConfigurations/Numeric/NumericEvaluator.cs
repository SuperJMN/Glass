namespace Glass.Imaging.ZoneConfigurations.Numeric
{
    using System.Linq;

    public class NumericEvaluator : Evaluator
    {
        private const int InRangeScore = 50;
        private readonly NumericStringFilter numericStringFilter;

        public NumericEvaluator(NumericStringFilter stringFilter) : base(stringFilter)
        {
            numericStringFilter = stringFilter;
        }

        public override double GetScore(string s)
        {
            var baseScore = base.GetScore(s);
            decimal value;
            var success = decimal.TryParse(s, out value);
            if (success)
            {
                var stringFilter = numericStringFilter;

                if (value >= stringFilter.Minimum && value <= stringFilter.Maximum)
                {
                    return baseScore + InRangeScore;
                }
            }

            return baseScore;
        }

        protected override int GetValidChars(string s)
        {
            return s.Count(char.IsNumber);
        }
    }
}