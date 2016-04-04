namespace Glass.Imaging
{
    using System.Collections.Generic;
    using System.Windows.Media.Imaging;

    public abstract class OcrService : IImageToTextConverter
    {
        public double SourceScaleForOcr { get; set; } = 0.3;
        public bool IsSourceScalingEnabledForOcr { get; set; } = false;
        public abstract IEnumerable<IBitmapBatchGenerator> BitmapGenerators { get; }
        public abstract IEnumerable<RecognitionResult> Recognize(BitmapSource bitmap, ZoneConfiguration config);
        public abstract IEnumerable<ImageTarget> ImageTargets { get; }
    }
}