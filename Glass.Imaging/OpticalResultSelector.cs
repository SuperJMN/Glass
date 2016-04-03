namespace Glass.Imaging
{
    using System.Collections.Generic;
    using System.Linq;

    public class OpticalResultSelector
    {
        public RecognitionResult Select(IEnumerable<RecognitionResult> recognitions, ZoneConfiguration zoneConfiguration)
        {
            var scores = from r in recognitions
                let score = zoneConfiguration.TextualDataFilter.Filter(r.Text)
                select new { Score = score, Result = r };


            return scores.OrderByDescending(result => result.Score)
                .Select(arg => arg.Result)
                .FirstOrDefault();
        }
    }
}