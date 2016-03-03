using System.Windows;

namespace Glass.Design.Panels.RelativeCanvas {
    internal struct SizeInfo {
        public SizeInfo(Size size)
            : this() {
            Size = size;
            WidthChanged = false;
            HeightChanged = true;
        }

        public Size Size { get; set; }
        public bool WidthChanged { get; set; }
        public bool HeightChanged { get; set; }
    }
}