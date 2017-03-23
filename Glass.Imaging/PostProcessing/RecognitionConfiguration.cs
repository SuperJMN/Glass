namespace Glass.Imaging.PostProcessing
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using DotImaging;
    using ZoneConfigurations;

    public class RecognitionConfiguration
    {
        public IEnumerable<ZoneConfiguration> Zones { get; set; }

        public static RecognitionConfiguration FromSingleImage(IImage bitmapSource, ITextualDataFilter dataFilter, Symbology symbology)
        {
            var bounds = new Rect(0, 0, bitmapSource.Width, bitmapSource.Height);
            var zoneConfiguration = new ZoneConfiguration() { Bounds = bounds, TextualDataFilter = dataFilter, Id = "", Symbology = symbology};
            return new RecognitionConfiguration() { Zones = new List<ZoneConfiguration>() { zoneConfiguration } };
        }
    }
}