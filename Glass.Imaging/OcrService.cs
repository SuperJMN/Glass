namespace Glass.Imaging
{
    using System.Collections.Generic;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public abstract class OcrService : IImageToTextConverter
    {
        public abstract double SourceScaleForOcr { get; }
        public abstract bool IsSourceScalingEnabledForOcr { get; }
        public abstract IEnumerable<IBitmapBatchGenerator> BitmapGenerators { get; }
        public abstract IEnumerable<RecognitionResult> Recognize(BitmapSource bitmap, ZoneConfiguration config);
        public abstract IEnumerable<ImageTarget> ImageTargets { get; }

        protected BitmapSource ScaleIfEnabled(BitmapSource bmp)
        {
            var scale = SourceScaleForOcr;
            var isScalingEnabled = IsSourceScalingEnabledForOcr;

            return isScalingEnabled ? new TransformedBitmap(bmp, new ScaleTransform(scale, scale)) : bmp;
        }
    }
}