namespace Glass.Imaging.ZoneConfigurations
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    public abstract class Evaluator : IEvaluator
    {
        private readonly StringFilter stringFilter;

        private const double ValidCharFactor = 5;
        private const double InvalidCharFactor = 3;
        private const double LengthScore = 17;
        private const double RegexScore = 200;

        protected Evaluator(StringFilter stringFilter)
        {
            this.stringFilter = stringFilter;
        }

        public double GetScore(string s)
        {
            if (s == null)
            {
                return double.MinValue;
            }

            if (s.Equals("<BARCODE>"))
            {
                return double.MinValue;
            }

            var validCharsCount = GetValidChars(s);
            var invalidChars = s.Length - validCharsCount;

            var validScore = validCharsCount * ValidCharFactor;
            var invalidScore = invalidChars * InvalidCharFactor;

            var lengthScore = GetLengthScore(s, stringFilter.MinLength, stringFilter.MaxLength);

            double regexScore;
            if (stringFilter.Regex != null)
            {
                var regex = new Regex(stringFilter.Regex);
                var match = regex.Match(s);
                regexScore = stringFilter.Regex == null ? 0 : match.Length == s.Length ? RegexScore : 0;
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