using System;

namespace Glass.Design {
    public interface ISizable {
        double Width { get; set; }
        double Height { get; set; }
        double MinWidth { get; set; }
        double MinHeight { get; set; }
        event EventHandler SizeChanged;
    }
}