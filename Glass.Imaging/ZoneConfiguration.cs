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
    }
}