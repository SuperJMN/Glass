using System.Windows;
using System.Windows.Controls;

namespace Glass.Design.Panels.RelativeCanvas {
    public class ResizerCanvas2 : Canvas {

        private readonly DesignableGroup group;

        public ResizerCanvas2() {            

            group = new DesignableGroup();

            this.SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs) {
            if (sizeChangedEventArgs.WidthChanged) {
                group.Width = sizeChangedEventArgs.NewSize.Width;
            }
            if (sizeChangedEventArgs.HeightChanged) {
                group.Height = sizeChangedEventArgs.NewSize.Height;
            }
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved) {

            var elementAdded = (FrameworkElement)visualAdded;

            if (elementAdded.IsLoaded) {
                var designableAdapter = new FrameworkElementDesignableAdapter(elementAdded);
                group.Items.Add(designableAdapter);

            } else {
                elementAdded.Loaded += ElementAddedOnLoaded;
            }

            //children.Remove(new FrameworkElementDesignableAdapter((FrameworkElement)visualRemoved));
        }

        private void ElementAddedOnLoaded(object sender, RoutedEventArgs routedEventArgs) {
            var elementAdded = (FrameworkElement)sender;
            var designableAdapter = new FrameworkElementDesignableAdapter(elementAdded);
            group.Items.Add(designableAdapter);
            
        }
    }
}
