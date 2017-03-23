namespace Glass.Imaging
{
    using System.Collections.Generic;
    using Accord.Extensions.Imaging;
    using DotImaging;
    using DotImaging.Primitives2D;

    public abstract class OcrService : IImageToTextConverter
    {
        public abstract double SourceScaleForOcr { get; }
        public abstract bool IsSourceScalingEnabledForOcr { get; }
        public abstract IEnumerable<IBitmapBatchGenerator> BitmapGenerators { get; }
        public abstract IEnumerable<RecognitionResult> Recognize(IImage bitmap, ZoneConfiguration config);
        public abstract IEnumerable<ImageTarget> ImageTargets { get; }

        protected IImage ScaleIfEnabled(IImage bmp)
        {
            var scale = SourceScaleForOcr;
            var isScalingEnabled = IsSourceScalingEnabledForOcr;
            var scaledSize = new Size((int) (bmp.Width * scale), (int) (bmp.Height * scale));

            return isScalingEnabled ? bmp.ToBgr().Resize(scaledSize, InterpolationMode.Bicubic).Lock() : bmp;
        }
    }
}