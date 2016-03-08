namespace Glass.Imaging
{
    using System.Linq;
    using Core;
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
            switch (fieldConfiguration.SmartZoneType)
            {
                case SmartZoneType.Number:
                    validCharsCount = s.Count(char.IsNumber);
                    break;

                case SmartZoneType.AlphaOnly:
                    validCharsCount = s.Count(char.IsLetter);
                    break;

                case SmartZoneType.Alpha:
                case SmartZoneType.Barcode:
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
            var maskLen = GetMaskLength(fieldConfiguration);
            if (maskLen == null)
            {
                return 0;
            }

            double average;

            if (maskLen.FixedLength != 0)
            {
                average = maskLen.FixedLength;
            }
            else
            {
                average = new[] { maskLen.Min, maskLen.Max }.Average();
            }

            var total = str.Length;

            return ScorePolicy.GetScore(total, average);
        }

        private static MaskLength GetMaskLength(ZoneConfiguration fieldConfiguration)
        {
            var mask = fieldConfiguration.Mask;
            if (mask == null)
            {
                return null;
            }

            var match = Regexes.MaskLength.Match(mask);

            if (!match.Success)
            {
                return null;
            }

            if (match.Groups[1].Success || match.Groups[1].Success)
            {
                var min = match.Groups[1].Value;
                var max = match.Groups[2].Value;

                return new MaskLength(min.ToInt32(), max.ToInt32());
            }

            if (match.Groups[2].Success)
            {
                return new MaskLength(match.Captures[2].Value.ToInt32());
            }

            return null;
        }
    }
}