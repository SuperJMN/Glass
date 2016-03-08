namespace Glass.Imaging.PostProcessing
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Core;

    public class OcrPostProcessor : IOcrPostProcessor
    {
        private readonly ISingleLinePolicy singleLinePolicy;

        public OcrPostProcessor(ISingleLinePolicy singleLinePolicy)
        {
            this.singleLinePolicy = singleLinePolicy;
        }

        public string Process(string input, ZoneConfiguration fieldConfiguration)
        {
            var str = input.TrimEnd();
            
            if (fieldConfiguration.ZoneType == ZoneType.Number)
            {
                return ProcessNumeric(fieldConfiguration, str);
            }

            if (fieldConfiguration.ZoneType == ZoneType.AlphaOnly || fieldConfiguration.ZoneType == ZoneType.Alpha)
            {
                str = ProcessAlpha(fieldConfiguration, str);
            }
            
            return str;
        }

        private static string ProcessAlpha(ZoneConfiguration fieldConfiguration, string str)
        {
            str = ApplyWhitespacePolicy(str);
            return str;
        }

        private string ProcessNumeric(ZoneConfiguration fieldConfiguration, string str)
        {
            if (fieldConfiguration.IsSingleLine)
            {
                if (fieldConfiguration.ZoneType == ZoneType.Number)
                {
                    str = Chunkify(str);
                }

                return GetBestMatchFromChunkifiedString(str, fieldConfiguration);
            }

            return str;
        }

        private string Chunkify(string str)
        {
            str = str.CollapseSpaces();
            str = ReplaceBarcodeString(str);
            return str.Replace(" ", ";");
        }

        private string ReplaceBarcodeString(string str)
        {
            var regex = new Regex(@"\s[\s1|Ll]+\s");
            return regex.Replace(str, " <BARCODE> ");
        }

        private static string ApplyWhitespacePolicy(string str)
        {
            str = str.CollapseSpaces();
            return str;
        }

        private string GetBestMatchFromChunkifiedString(string str, ZoneConfiguration fieldConfiguration)
        {
            var recognitionUnits = (from line in str.GetChunks()
                                    select new { Line = line, Score = singleLinePolicy.GetScore(line, fieldConfiguration) }).ToList();

            var sortedByScore = recognitionUnits.OrderByDescending(g => g.Score).Select(g => g.Line);
            return !recognitionUnits.Any() ? str : sortedByScore.FirstOrDefault();
        }

        private static string WipeCarriageReturns(string input)
        {
            var str = string.Copy(input);
            str = str.Replace(Environment.NewLine, "");
            return str;
        }
    }
}