using System;
using System.Windows;
using System.Windows.Controls;

namespace Glass.Design {

    public class FrameworkElementDesignableAdapter : IDesignable {

        private readonly FrameworkElement element;
        private double minWidth;
        private double minHeight;

        public FrameworkElementDesignableAdapter(FrameworkElement element) {
            this.element = element;
        }

        public double Width {
            get { return element.ActualWidth; }
            set { Element.Width = value; }
        }

        public double Height {
            get { return element.ActualHeight; }
            set { Element.Height = value; }
        }

        public double MinWidth
        {
            get { return element.MinWidth; }
            set { element.MinWidth = value; }
        }

        public double MinHeight
        {
            get { return element.MinHeight; }
            set { element.MinHeight = value; }
        }

        public double Left {
            get {
                double left = Canvas.GetLeft(Element);
                return double.IsNaN(left) ? 0 : left;
            }
            set { Canvas.SetLeft(Element, value); }
        }
        public double Top {
            get {
                var top = Canvas.GetTop(Element);
                return double.IsNaN(top) ? 0 : top;
            }
            set { Canvas.SetTop(Element, value); }
        }

        public Point AnchorPoint {
            get { return Element.RenderTransformOrigin; }
            set { Element.RenderTransformOrigin = value; }
        }

        public FrameworkElement Element
        {
            get { return element; }
        }

        public event EventHandler SizeChanged;
        public event EventHandler LocationChanged;
    }
}