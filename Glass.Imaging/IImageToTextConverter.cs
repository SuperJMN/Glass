namespace Glass.Imaging
{
    using System.Collections.Generic;
    using System.Windows.Media.Imaging;

    public interface IImageToTextConverter
    {
        IEnumerable<RecognitionResult> Recognize(BitmapSource bitmap, ZoneConfiguration config);
        IEnumerable<ImageTarget> ImageTargets { get; }
    }

    public class RecognitionResult
    {
        public RecognitionResult(string text, double confidence)
        {
            Text = text;
            Confidence = confidence;
        }

        public string Text { get; set; }
        public double Confidence { get; set; }
    }
}