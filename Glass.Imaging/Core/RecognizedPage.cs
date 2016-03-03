namespace Glass.Imaging.Core
{
    using System.Collections.Generic;
    using System.Windows.Media;

    public class RecognizedPage
    {
        public RecognizedPage(ImageSource image, IEnumerable<RecognizedZone> recognizedZones)
        {
            Image = image;
            RecognizedZones = recognizedZones;
        }

        public ImageSource Image { get; set; }
        public IEnumerable<RecognizedZone> RecognizedZones { get; private set; }
    }
}