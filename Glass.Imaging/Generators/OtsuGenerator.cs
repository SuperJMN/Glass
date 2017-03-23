namespace Glass.Imaging.Generators
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media.Imaging;
    using DotImaging;
    using Filters;

    public class OtsuGenerator : IBitmapBatchGenerator
    {
        public IEnumerable<IImage> Generate(IImage image)
        {
            yield return new OtsuThresholdFilterFree().Apply(image);
        }
    }
}