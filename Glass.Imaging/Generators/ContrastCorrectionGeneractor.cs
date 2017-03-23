namespace Glass.Imaging.Generators
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Media.Imaging;
    using Accord.Extensions.Imaging;
    using AForge.Imaging.Filters;
    using Core;
    using DotImaging;
    using Imaging;

    public class ContrastCorrectionGeneractor : IBitmapBatchGenerator
    {
        public IEnumerable<IImage> Generate(IImage image)
        {
            var factors = EnumerableExtensions.Range(10, 120, i => i + 15);
            //var median = new Median().Apply(original);
            var generatedBmps = factors.Select(f => Correction(f, image)).ToList();
            return generatedBmps;
        }

        private static IImage Correction(int f, IImage original)
        {
            var bitmap = new ContrastCorrection(f).Apply(new BrightnessCorrection(f).Apply(original.ToBgr().Lock().AsAForgeImage()));
            return bitmap.AsImage();
        }
    }
}