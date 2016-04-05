using System.Windows.Media.Imaging;

namespace Glass.Imaging
{
    using System.Windows;
    using ZoneConfigurations;

    public class ZoneConfiguration
    {        
        public Rect Bounds { get; set; }
        public string Id { get; set; }
        public ITextualDataFilter TextualDataFilter { get; set; }
        public Symbology Symbology { get; set; }

        public static ZoneConfiguration FromSingleImage(BitmapSource bitmapSource, ITextualDataFilter dataFilter, Symbology symbology)
        {

            var bounds = new Rect(0, 0, bitmapSource.Width, bitmapSource.Height);
            return new ZoneConfiguration() { Bounds = bounds, TextualDataFilter = dataFilter, Id = "", Symbology = symbology };
        }
    }
}