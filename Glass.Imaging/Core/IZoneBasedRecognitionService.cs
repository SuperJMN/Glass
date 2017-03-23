namespace Glass.Imaging.Core
{
    using DotImaging;
    using PostProcessing;

    public interface IZoneBasedRecognitionService
    {
        RecognizedPage Recognize(IImage image, RecognitionConfiguration configuration);        
    }    
}