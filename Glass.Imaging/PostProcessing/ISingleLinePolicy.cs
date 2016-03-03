namespace Glass.Imaging.PostProcessing
{
    public interface ISingleLinePolicy
    {
        double GetScore(string text, ZoneConfiguration configuration);
    }
}