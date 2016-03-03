namespace Glass.Imaging
{
    using System;

    public static class ScorePolicy
    {
        private const double LengthScore = 4;

        public static double GetScore(int n, double reference)
        {
            var diff = n - reference;
            var squaredDiff = Math.Pow(diff, 2.0);
            var proportion = squaredDiff / reference;
            var score = LengthScore * (1 - proportion);
            return score;
        }
    }
}