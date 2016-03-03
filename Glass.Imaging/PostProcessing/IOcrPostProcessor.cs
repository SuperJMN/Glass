namespace Glass.Imaging.PostProcessing
{
    public interface IOcrPostProcessor
    {
        string Process(string input, ZoneConfiguration config);
    }
}