namespace Glass.Imaging.Generators
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Media.Imaging;
    using AForge.Imaging.Filters;
    using Core;
    using Imaging;

    public class ContrastCorrectionGeneractor : IBitmapBatchGenerator
    {
        public IEnumerable<BitmapSource> Generate(BitmapSource image)
        {
            var factors = EnumerableExtensions.Range(10, 120, i => i + 15);
            var original = image.ToBitmap();
            //var median = new Median().Apply(original);
            var generatedBmps = factors.Select(f => Correction(f, original)).ToList();
            return generatedBmps;
        }

        private static BitmapSource Correction(int f, Bitmap original)
        {
            var bitmap = new ContrastCorrection(f).Apply(new BrightnessCorrection(f).Apply(original));
            return bitmap.ToBitmapImage();
        }
    }
}