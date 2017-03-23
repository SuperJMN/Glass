namespace Glass.Imaging
{
    using System.Collections.Generic;
    using System.Windows.Media.Imaging;
    using DotImaging;

    public interface IBitmapBatchGenerator
    {
        IEnumerable<IImage> Generate(IImage image);
    }
}