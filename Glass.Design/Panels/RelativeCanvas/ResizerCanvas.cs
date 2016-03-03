using System.Windows;
using System.Windows.Controls;

namespace Glass.Design.Panels.RelativeCanvas {

    public class ResizerCanvas : Canvas {


        protected override Size MeasureOverride(Size constraint) {

            foreach (FrameworkElement child in InternalChildren) {
                if (child == null) { continue; }

                var size = GetMeasureSize(constraint, child);

                var relativeSize = size;
                child.Measure(relativeSize);
            }

            return new Size();
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved) {

            if (visualAdded != null) {
                var element = (FrameworkElement) visualAdded;
                element.SizeChanged += ElementOnSizeChanged;
            }
            if (visualRemoved != null) {
                var element = (FrameworkElement)visualRemoved;
                element.SizeChanged -= ElementOnSizeChanged;
            }

            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }

        private void ElementOnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs) {
            InvalidateArrange();
        }

        private static Size GetMeasureSize(Size constraint, FrameworkElement child) {

            double width = double.PositiveInfinity;
            double height = double.PositiveInfinity;

            var relativeSize = GetRelativeSize(child);

            if (!double.IsNaN(child.Width)) {
                width = child.Width;
            } else if (!double.IsInfinity(relativeSize.Width)) {
                width = relativeSize.Width * constraint.Width;
            }

            if (!double.IsNaN(child.Height)) {
                height = child.Height;
            } else if (!double.IsInfinity(relativeSize.Width)) {
                width = relativeSize.Width * constraint.Height;
            }

            return new Size(width, height);
        }

        private static Size GetArrangeSize(Size constraint, FrameworkElement child) {

            double width = child.DesiredSize.Width;
            double height = child.DesiredSize.Height;

            var relativeSize = GetRelativeSize(child);

            if (!double.IsNaN(child.Width)) {
                width = child.Width;
            } else if (!double.IsInfinity(relativeSize.Width)) {
                width = relativeSize.Width * constraint.Width;
            }

            if (!double.IsNaN(child.Height)) {
                height = child.Height;
            } else if (!double.IsInfinity(relativeSize.Width)) {
                width = relativeSize.Width * constraint.Height;
            }

            return new Size(width, height);
        }

        protected override Size ArrangeOverride(Size arrangeSize) {
            //Canvas arranges children at their DesiredSize. 
            //This means that Margin on children is actually respected and added 
            //to the size of layout partition for a child.
            //Therefore, is Margin is 10 and Left is 20, the child's ink will start at 30. 

            foreach (FrameworkElement child in InternalChildren) {
                if (child == null) { continue; }

                double x = 0;
                double y = 0;

                var size = GetArrangeSize(arrangeSize, child);

                //Compute offset of the child:
                //If Left is specified, then Right is ignored
                //If Left is not specified, then Right is used
                //If both are not there, then 0 
                double left = GetRelativeLocation(child).X * arrangeSize.Width - GetRelativeHook(child).X * size.Width;

                if (!double.IsNaN(left)) {
                    x = left;
                } else {
                    double right = GetRight(child);

                    if (!double.IsNaN(right)) {
                        x = arrangeSize.Width - size.Width - right;
                    }
                }

                double top = GetRelativeLocation(child).Y * arrangeSize.Height - GetRelativeHook(child).Y * size.Height;

                if (!double.IsNaN(top)) {
                    y = top;
                } else {
                    double bottom = GetBottom(child);

                    if (!double.IsNaN(bottom)) {
                        y = arrangeSize.Height - size.Height - bottom;
                    }
                }

                child.Arrange(new Rect(new Point(x, y), size));
            }
            return arrangeSize;

        }

        #region RelativeLocation

        public static readonly DependencyProperty RelativeLocationProperty =
            DependencyProperty.RegisterAttached("RelativeLocation", typeof(Point), typeof(ResizerCanvas),
                new FrameworkPropertyMetadata(new Point(double.NaN, double.NaN),
                    OnRelativeLocationChanged));

        public static Point GetRelativeLocation(DependencyObject d) {
            return (Point)d.GetValue(RelativeLocationProperty);
        }

        public static void SetRelativeLocation(DependencyObject d, Point value) {
            d.SetValue(RelativeLocationProperty, value);
        }

        private static void OnRelativeLocationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var oldRelativeLocation = (Point)e.OldValue;
            var newRelativeLocation = (Point)d.GetValue(RelativeLocationProperty);
        }

        #endregion

        #region RelativeHook

        public static readonly DependencyProperty RelativeHookProperty =
            DependencyProperty.RegisterAttached("RelativeHook", typeof(Point), typeof(ResizerCanvas),
                new FrameworkPropertyMetadata(new Point(0.5, y: 0.5),
                    OnRelativeHookChanged));

        public static Point GetRelativeHook(DependencyObject d) {
            return (Point)d.GetValue(RelativeHookProperty);
        }

        public static void SetRelativeHook(DependencyObject d, Point value) {
            d.SetValue(RelativeHookProperty, value);
        }

        private static void OnRelativeHookChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var oldRelativeHook = (Point)e.OldValue;
            var newRelativeHook = (Point)d.GetValue(RelativeHookProperty);
        }

        #endregion

        #region RelativeSize

        public static readonly DependencyProperty RelativeSizeProperty =
            DependencyProperty.RegisterAttached("RelativeSize", typeof(Size), typeof(ResizerCanvas),
                new FrameworkPropertyMetadata(Size.Empty,
                    OnRelativeSizeChanged));

        public static Size GetRelativeSize(DependencyObject d) {
            return (Size)d.GetValue(RelativeSizeProperty);
        }

        public static void SetRelativeSize(DependencyObject d, Size value) {
            d.SetValue(RelativeSizeProperty, value);
        }

        private static void OnRelativeSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var oldRelativeSize = (Size)e.OldValue;
            var newRelativeSize = (Size)d.GetValue(RelativeSizeProperty);
        }

        #endregion
    }

}
