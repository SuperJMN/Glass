namespace Glass.Imaging
{
    using PostProcessing;

    public interface IScorePolicy
    {
        double GetScore(string s, int minLegth, int maxLegth);
    }
}