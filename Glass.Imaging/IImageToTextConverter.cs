namespace Glass.Imaging
{
    using System.Collections.Generic;
    using System.Windows.Media.Imaging;

    public interface IImageToTextConverter
    {
        IEnumerable<RecognitionResult> Recognize(BitmapSource bitmap, ZoneConfiguration config);
        IEnumerable<ImageTarget> ImageTargets { get; }
    }
}