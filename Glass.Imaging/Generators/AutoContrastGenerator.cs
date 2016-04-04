namespace Glass.Imaging.Generators
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media.Imaging;
    using AForge.Imaging.Filters;
    using Core;
    using Imaging;

    public class ContrastCorrectionGeneractor : IBitmapBatchGenerator
    {
        public IEnumerable<BitmapSource> Generate(BitmapSource image)
        {
            var factors = EnumerableExtensions.Range(100, 200, i => i + 30);
            var original = image.ToBitmap();
            var median = new Median().Apply(original);
            var generatedBmps = factors.Select(f => new ContrastCorrection(f).Apply(median).ToBitmapImage()).ToList();
            return generatedBmps;
        }
    }
}