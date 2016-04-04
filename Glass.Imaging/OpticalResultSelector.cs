namespace Glass.Imaging
{
    using System.Collections.Generic;
    using System.Linq;

    public static class OpticalResultSelector
    {
        public static RecognitionResult ChooseBest(IEnumerable<RecognitionResult> recognitions, ZoneConfiguration zoneConfiguration)
        {
            var scores = from r in recognitions
                let score = zoneConfiguration.TextualDataFilter.Evaluator.GetScore(r.Text)
                let globalScore = score
                select new { Score = globalScore, Result = r };


            return scores.OrderByDescending(result => result.Score)
                .Select(arg => arg.Result)
                .FirstOrDefault();
        }
    }
}