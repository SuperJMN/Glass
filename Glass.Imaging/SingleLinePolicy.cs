namespace Glass.Imaging
{
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using PostProcessing;

    public class SingleLinePolicy : ISingleLinePolicy
    {
        private const double MistakenBarcodeFactor = -2;
        private const double ValidCharFactor = 1;
        private const double InvalidCharFactor = -2;

        public double GetScore(string s, ZoneConfiguration field)
        {
            int validCharsCount;
            switch (field.SmartZoneType)
            {
                case SmartZoneType.Digits:
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
            var mistakenBarcodeScore = GetMistakenBarcodeScore(s, field) * MistakenBarcodeFactor;
            var lengthScore = GetLengthScore(s, field);

            var score = validScore + invalidScore + lengthScore + mistakenBarcodeScore;
            return score;
        }

        private static double GetMistakenBarcodeScore(string str, ZoneConfiguration field)
        {
            // THIS SHOULD NOT HAPPEN!
            if (field.SmartZoneType == SmartZoneType.Barcode)
            {
                return 0;
            }

            var numeric = new[] { ' ', '1' };
            var alpha = new[] { 'l', 'I', 'L', '|' };
            var alphaNumeric = new HashSet<char>(numeric.Concat(alpha)).ToArray();

            if (field.SmartZoneType == SmartZoneType.Digits)
            {                
                return GetBarcodeScoreFormula(str, numeric);
            }
            
            if (field.SmartZoneType == SmartZoneType.AlphaOnly)
            {
                return GetBarcodeScoreFormula(str, alpha);
            }

            if (field.SmartZoneType == SmartZoneType.Alpha)
            {
                return GetBarcodeScoreFormula(str, alphaNumeric);
            }

            return 0;
        }

        private static double GetBarcodeScoreFormula(string str, char[] possibleBarcodeChars)
        {
            var barcodeChars = str.Count(possibleBarcodeChars.Contains);
            var total = (double) str.Length;

            return barcodeChars/total;
        }

        private static double GetLengthScore(string str, ZoneConfiguration field)
        {
            var maskLen = GetMaskLength(field);
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

        private static MaskLength GetMaskLength(ZoneConfiguration field)
        {
            var mask = field.Mask;
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