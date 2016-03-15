namespace Glass.Imaging.Core
{
    using System.Windows.Media.Imaging;
    using PostProcessing;

    public interface IZoneBasedRecognitionService
    {
        RecognizedPage Recognize(BitmapSource image, RecognitionConfiguration configuration);        
    }    
}