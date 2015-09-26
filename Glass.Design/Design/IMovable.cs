using System;

namespace Glass.Design {
    public interface IMovable {
        double Left { get; set; }
        double Top { get; set; }
        event EventHandler LocationChanged;
    }
}