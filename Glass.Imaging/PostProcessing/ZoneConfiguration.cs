namespace Glass.Imaging.PostProcessing
{
    using System.Windows;

    public class ZoneConfiguration
    {
        public bool IsSingleLine { get; set; }
        public ZoneType ZoneType { get; set; }
        public string Regex { get; set; }
        public Rect Bounds { get; set; }
        public int MaxLenght { get; set; } = int.MaxValue;
        public int MinLength { get; set; } = 0;
        public string Id { get; set; }
    }
}