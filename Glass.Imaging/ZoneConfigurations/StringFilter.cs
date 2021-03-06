namespace Glass.Imaging.ZoneConfigurations
{
    using System.Linq;
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

        public string GetBestMatchFromRaw(string input)
        {
            if (input == null)
            {
                return null;
            }

            return ProcessFromRaw(input);
        }

        private string ProcessFromRaw(string input)
        {
            string str = input;
            str = Chunkify(str);
            var bestMatch = GetBestMatchFromChunkifiedString(str);
            return bestMatch.Contains("<NOISE>") ? null : bestMatch;
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
            var split = str.Split(' ');

            var scoreList = from s in split
                            let score = GetScore(s)
                            select new { Text = s, Score = score };

            return string.Join(" ", scoreList.Select(arg => arg.Score > 0.6 ? "<NOISE>" : arg.Text));            
        }

        private double GetScore(string s)
        {
            var lenght = s.Length;
            var noSpaces = !s.Any(char.IsWhiteSpace) ? 1 : 0;
            var lotsOfSymbols = (double) s.Count(char.IsSymbol)/lenght;
            var lotsOfSuspiciousChars = (double) s.Count(IsSuspicious)/lenght;
            var score = noSpaces * 0.2 + lotsOfSymbols * 0.2 + lotsOfSuspiciousChars * 0.6;
            return score;
        }

        private bool IsSuspicious(char c)
        {
            var suspicious = "1|!NLlIWN|�";
            return suspicious.Contains(c);
        }
    }
}