namespace Glass.Imaging.Core
{
    using System.Collections.Generic;
    using System.Windows.Media;
    using DotImaging;

    public class RecognizedPage
    {
        public RecognizedPage(IImage image, IEnumerable<RecognizedZone> recognizedZones)
        {
            Image = image;
            RecognizedZones = recognizedZones;
        }

        public IImage Image { get; set; }
        public IEnumerable<RecognizedZone> RecognizedZones { get; private set; }
    }
}