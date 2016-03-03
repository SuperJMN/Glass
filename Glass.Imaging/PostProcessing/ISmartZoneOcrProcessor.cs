namespace Glass.Imaging.PostProcessing
{
    using System.Windows.Media;
    using Core;

    public interface ISmartZoneOcrProcessor
    {
        RecognizedPage Recognize(ImageSource image, RecognitionConfiguration configuration);
    }
}