namespace Glass.Imaging
{
    using System.Linq;
    using PostProcessing;

    public class SingleLinePolicy : ISingleLinePolicy
    {
        private const double ValidCharFactor = 5;
        private const double InvalidCharFactor = 3;

        public double GetScore(string s, ZoneConfiguration fieldConfiguration)
        {
            if (s.Equals("<BARCODE>"))
            {
                return double.MinValue;
            }

            int validCharsCount;
            switch (fieldConfiguration.ZoneType)
            {
                case ZoneType.Number:
                    validCharsCount = s.Count(char.IsNumber);
                    break;

                case ZoneType.AlphaOnly:
                    validCharsCount = s.Count(char.IsLetter);
                    break;

                case ZoneType.Alpha:
                case ZoneType.Barcode:
                    validCharsCount = s.Count(c => char.IsLetter(c) || char.IsNumber(c) || char.IsPunctuation(c));
                    break;

                default:
                    validCharsCount = s.Length;
                    break;
            }

            var invalidChars = s.Length - validCharsCount;

            var validScore = validCharsCount * ValidCharFactor;
            var invalidScore = invalidChars * InvalidCharFactor;
            var lengthScore = GetLengthScore(s, fieldConfiguration);

            var score = validScore + invalidScore + lengthScore;
            return score;
        }

        private static double GetLengthScore(string str, ZoneConfiguration fieldConfiguration)
        {
            if (fieldConfiguration.MaxLenght == int.MaxValue)
            {
                return 0;
            }
            
            var  average = new[] { fieldConfiguration.MinLength, fieldConfiguration.MaxLenght }.Average();
            var total = str.Length;

            return ScorePolicy.GetScore(total, average);
        }
    }
}