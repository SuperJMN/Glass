namespace Glass.Imaging.Core
{
    using DotImaging;

    public class RecognizedZone
    {
        public RecognizedZone(IImage bitmap, ZoneConfiguration zoneConfiguration, RecognitionResult recognitionResult)
        {
            ZoneConfig = zoneConfiguration;
            Bitmap = bitmap;
            RecognitionResult = recognitionResult;
        }

        public RecognitionResult RecognitionResult { get; set; }

        public IImage Bitmap { get; set; }

        public ZoneConfiguration ZoneConfig { get; set; }
        public string Id => ZoneConfig.Id;
    }
}