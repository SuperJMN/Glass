namespace Glass.Imaging.PostProcessing
{
    using System.Windows;

    public class ZoneConfiguration
    {
        public bool IsSingleLine { get; set; }

        public SmartZoneType SmartZoneType { get; set; } 

        public bool ReplaceLinesBySpaces { get; set; }

        public string Mask { get; set; }

        public Rect Bounds { get; set; }
        public int? Max { get; set; }
        public int? Min { get; set; }
        public bool IsRequired { get; set; }
        public string Id { get; set; }
    }
}