namespace Glass.Imaging.Generators
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media.Imaging;
    using Filters;

    public class OtsuGenerator : IBitmapBatchGenerator
    {
        public IEnumerable<BitmapSource> Generate(BitmapSource image)
        {
            yield return new OtsuThresholdFilterFree().Apply(image);
        }
    }
}