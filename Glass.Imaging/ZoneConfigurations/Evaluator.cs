namespace Glass.Imaging.ZoneConfigurations
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    public abstract class Evaluator : IEvaluator
    {
        protected StringFilter Filter { get; }

        private const double ValidCharFactor = 4;
        private const double InvalidCharFactor = 3;
        private const double LengthScore = 20;
        private const double RegexScore = 50;

        protected Evaluator(StringFilter stringFilter)
        {
            Filter = stringFilter;
        }

        public virtual double GetScore(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return double.MinValue;
            }

            if (s.Equals("<NOISE>"))
            {
                return double.MinValue;
            }

            var validCharsCount = GetValidChars(s);
            var invalidChars = s.Length - validCharsCount;

            var validScore = validCharsCount * ValidCharFactor;
            var invalidScore = invalidChars * InvalidCharFactor;

            var lengthScore = GetLengthScore(s, Filter.MinLength, Filter.MaxLength);

            double regexScore;
            if (Filter.Regex != null)
            {
                var regex = new Regex(Filter.Regex);
                var match = regex.Match(s);
                regexScore = Filter.Regex == null ? 0 : match.Length == s.Length ? RegexScore : 0;
            }
            else
            {
                regexScore = 0;
            }

            var score = validScore + invalidScore + lengthScore + regexScore;
            return score;
        }

        protected abstract int GetValidChars(string str);

        private static double GetLengthScore(string str, int minLength, int maxLength)
        {
            if (maxLength == int.MaxValue)
            {
                return 0;
            }

            var average = new[] { minLength, maxLength }.Average();
            var total = str.Length;

            return GetScore(total, average);
        }

        private static double GetScore(int n, double reference)
        {
            var diff = n - reference;
            var squaredDiff = Math.Pow(diff, 2.0);
            var proportion = squaredDiff / reference;
            var score = LengthScore * (1 - proportion);
            return score;
        }
    }
}