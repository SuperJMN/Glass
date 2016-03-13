namespace Glass.Imaging.ZoneConfigurations
{
    public class ZeroEvaluator : IEvaluator
    {
        public double GetScore(string str)
        {
            return 0;
        }
    }
}