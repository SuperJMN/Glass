namespace Glass.Imaging.Core
{
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public class RecognizedZone
    {
        public RecognizedZone(BitmapSource image, ZoneConfiguration zoneConfiguration, string text)
        {
            ZoneConfig = zoneConfiguration;
            var bitmapSource = image.Crop(zoneConfiguration.Bounds);
            bitmapSource.Freeze();
            Image = bitmapSource;
            RecognizedText = text;
        }

        public string RecognizedText { get; set; }

        public ImageSource Image { get; set; }

        public ZoneConfiguration ZoneConfig { get; set; }
        public string Id => ZoneConfig.Id;
    }
}