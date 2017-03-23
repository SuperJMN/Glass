namespace Glass.Imaging
{
    using System.Collections.Generic;
    using System.Windows.Media.Imaging;
    using DotImaging;

    public interface IImageToTextConverter
    {
        IEnumerable<RecognitionResult> Recognize(IImage bitmap, ZoneConfiguration config);
        IEnumerable<ImageTarget> ImageTargets { get; }
    }
}