namespace Glass.LeadTools.Recognition
{
    using System.Collections.Generic;
    using System.Windows.Media.Imaging;
    using DotImaging;
    using Imaging;
    using ImagingExtensions.ImageFilters;

    public class AutoColorLevelFilterGenerator : IBitmapBatchGenerator
    {
        public IEnumerable<IImage> Generate(IImage image)
        {
            yield return new AutoColorLevelFilter().Apply(image);
        }
    }
}