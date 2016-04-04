namespace Glass.Imaging.Core
{
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public class RecognizedZone
    {
        public RecognizedZone(BitmapSource bitmap, ZoneConfiguration zoneConfiguration, RecognitionResult recognitionResult)
        {
            ZoneConfig = zoneConfiguration;
            var bitmapSource = ImagingContext.BitmapOperations.Crop(bitmap, zoneConfiguration.Bounds);
            bitmapSource.Freeze();
            Bitmap = bitmapSource;
            RecognitionResult = recognitionResult;
        }

        public RecognitionResult RecognitionResult { get; set; }

        public BitmapSource Bitmap { get; set; }

        public ZoneConfiguration ZoneConfig { get; set; }
        public string Id => ZoneConfig.Id;
    }
}