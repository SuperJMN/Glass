namespace Glass.Imaging
{
    using System.Collections.Generic;
    using System.Windows.Media.Imaging;

    public interface IBitmapBatchGenerator
    {
        IEnumerable<BitmapSource> Generate(BitmapSource image);
    }
}