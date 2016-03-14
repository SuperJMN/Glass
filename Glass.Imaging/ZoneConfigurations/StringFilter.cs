namespace Glass.Imaging.ZoneConfigurations
{
    using System.Linq;
    using System.Text.RegularExpressions;
    using Core;
    using PostProcessing;

    public abstract class StringFilter : ITextualDataFilter
    {
        public int MinLength { get; set; }
        public int MaxLength { get; set; } = int.MaxValue;
        public bool IsSingleLine { get; set; }
        public string Regex { get; set; }
        public abstract FilterType FilterType { get; }
        public abstract IEvaluator Evaluator { get; }

        public string Filter(string input)
        {
            if (input == null)
            {
                return null;
            }

            string str = input;
            str = Chunkify(str);
            return GetBestMatchFromChunkifiedString(str);
        }

        private string GetBestMatchFromChunkifiedString(string str)
        {
            var recognitionUnits = (from line in str.GetChunks()
                                    select new { Line = line, Score = Evaluator.GetScore(line) }).ToList();

            var sortedByScore = recognitionUnits.OrderByDescending(g => g.Score).Select(g => g.Line);
            return !recognitionUnits.Any() ? str : sortedByScore.FirstOrDefault();
        }

        private string Chunkify(string str)
        {
            str = str.CollapseSpaces();
            str = ReplaceBarcodeString(str);
            return str.Replace(" ", ";");
        }

        private string ReplaceBarcodeString(string str)
        {
            var regex = new Regex(@"[\s1|ILl]{3,}\s|\s[\s1|ILl]{3,}");
            return regex.Replace(str, " <BARCODE> ");
        }
    }
}