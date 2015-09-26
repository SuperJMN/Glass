using System.Windows;

namespace Glass.Design {
    public interface IDesignable : ISizable, IMovable {
        Point AnchorPoint { get; set; }
    }
}