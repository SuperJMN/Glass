namespace Glass.Imaging.ZoneConfigurations
{
    using PostProcessing;

    public interface ITextualDataFilter
    {
        bool IsSingleLine { get; set; }
        string Regex { get; set; }
        string GetBestMatchFromRaw(string input);
        FilterType FilterType { get; }
        IEvaluator Evaluator { get; }
    }
}