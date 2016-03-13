namespace Glass.Imaging.ZoneConfigurations.Alpha
{
    using System.Linq;

    public class AlphaOnlyEvaluator : Evaluator
    {
        public AlphaOnlyEvaluator(AlphaOnlyStringFilter alphaOnlyStringFilter) : base(alphaOnlyStringFilter)
        {
            
        }

        protected override int GetValidChars(string str)
        {
            return str.Count(char.IsLetter);
        }
    }
}