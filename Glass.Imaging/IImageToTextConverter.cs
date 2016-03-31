namespace Glass.Imaging
{
    using System.Collections.Generic;
    using System.Windows.Media.Imaging;

    public interface IImageToTextConverter
    {
        IEnumerable<string> Recognize(BitmapSource bitmap, ZoneConfiguration barcodeConfig);
        IEnumerable<ImageTarget> ImageTargets { get; }
    }
}