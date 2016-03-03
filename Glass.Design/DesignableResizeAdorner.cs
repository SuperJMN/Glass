using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Glass.Design {
    public class DesignableResizeAdorner : Adorner {
        private readonly IDesignable designable;
        private UIElement chrome;

        public DesignableResizeAdorner(UIElement adornedElement, IDesignable designable, UIElement chrome) : base(adornedElement) {
            this.designable = designable;

            this.designable.LocationChanged += DesignableLayoutChanged;
            this.designable.SizeChanged += DesignableLayoutChanged;            

            Chrome = chrome;
        }

        private void DesignableLayoutChanged(object sender, EventArgs e) {
            InvalidateArrange();
            //UpdateLayout();
        }

        protected override int VisualChildrenCount {
            get {
                return 1;
            }
        }

        protected override Visual GetVisualChild(int index) {
            if (index != 0)
                throw new ArgumentOutOfRangeException();

            return chrome;
        }

        public UIElement Chrome {
            get { return chrome; }
            set {
                if (chrome != null) {
                    RemoveVisualChild(chrome);
                }
                chrome = value;
                if (chrome != null) {
                    AddVisualChild(chrome);
                }
            }
        }

        //protected override Size MeasureOverride(Size constraint) {
        //    chrome.Measure(constraint);
        //    return constraint;
        //}

        protected override Size ArrangeOverride(Size finalSize) {
            chrome.Arrange(new Rect(new Point(designable.Left, designable.Top), new Size(designable.Width, designable.Height)));
            return finalSize;
        }
    }
}