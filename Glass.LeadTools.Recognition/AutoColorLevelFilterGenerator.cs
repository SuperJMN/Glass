namespace Glass.LeadTools.Recognition
{
    using System.Collections.Generic;
    using System.Windows.Media.Imaging;
    using Imaging;
    using ImagingExtensions.ImageFilters;

    public class AutoColorLevelFilterGenerator : IBitmapBatchGenerator
    {
        public IEnumerable<BitmapSource> Generate(BitmapSource image)
        {
            yield return new AutoColorLevelFilter().Apply(image);
        }
    }
}