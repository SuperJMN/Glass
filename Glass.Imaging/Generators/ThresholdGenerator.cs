namespace Glass.Imaging.Generators
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media.Imaging;
    using Filters;
    using Imaging;

    public class ThresholdGenerator : IBitmapBatchGenerator
    {
        public IEnumerable<BitmapSource> Generate(BitmapSource image)
        {
            var factors = EnumerableExtensions.Range(100, 200, i => i + 30);
            return factors.Select(f => new ThresholdFilter(f).Apply(image));
        }
    }
}