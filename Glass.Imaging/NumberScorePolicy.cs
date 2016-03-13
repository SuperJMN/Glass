namespace Glass.Imaging
{
    using System.Linq;

    class NumberScorePolicy : IScorePolicy
    {
        private const double ValidCharFactor = 5;
        private const double InvalidCharFactor = 3;

        public double GetScore(string s, int minLegth, int maxLegth)
        {
            if (s.Equals("<BARCODE>"))
            {
                return double.MinValue;
            }


            var validCharsCount = s.Count(char.IsNumber);
            var invalidChars = s.Length - validCharsCount;

            var validScore = validCharsCount * ValidCharFactor;
            var invalidScore = invalidChars * InvalidCharFactor;

            var lengthScore = GetLengthScore(s, minLegth, maxLegth);

            var score = validScore + invalidScore + lengthScore;
            return score;
        }

        private static double GetLengthScore(string str, int minLegth, int maxLegth)
        {
            if (maxLegth == int.MaxValue)
            {
                return 0;
            }

            var average = new[] { minLegth, maxLegth }.Average();
            var total = str.Length;

            return ScorePolicy.GetScore(total, average);
        }
    }
}