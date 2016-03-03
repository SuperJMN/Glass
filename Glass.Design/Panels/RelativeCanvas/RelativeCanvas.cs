using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Glass.Design.Panels.RelativeCanvas {
    public class RelativeCanvas : Canvas {

        private bool firstLoad = true;

        public RelativeCanvas() {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs) {
            SizeChanged -= OnSizeChanged;
        }

        void OnLoaded(object sender, RoutedEventArgs e) {


            if (firstLoad) {
                foreach (UIElement child in Children) {
                    UpdateHorizontalProportions(child);
                    UpdateVerticalProportions(child);
                }
                firstLoad = false;
            } else {
                var newSizeInfo = new SizeInfo(new Size(ActualWidth, ActualHeight)) { WidthChanged = true, HeightChanged = true };
                UpdateChildLocationsAfterSizeChange(newSizeInfo);
            }

            SizeChanged += OnSizeChanged;
        }

        private void UpdateChildLocationsAfterSizeChange(SizeInfo newSizeInfo) {

            foreach (UIElement uiElement in Children) {

                if (newSizeInfo.WidthChanged) {
                    var relativeX = (double)uiElement.GetValue(HorizontalProportionProperty);

                    if (!double.IsInfinity(relativeX)) {
                        var newX = relativeX * newSizeInfo.Size.Width;
                        SetLeftOfRelativePoint(uiElement, newX);
                    }
                }

                if (newSizeInfo.HeightChanged) {
                    var relativeY = (double)uiElement.GetValue(VerticalProportionProperty);

                    if (!double.IsInfinity(relativeY)) {
                        var newY = relativeY * newSizeInfo.Size.Height;
                        SetTopOfRelativePoint(uiElement, newY);
                    }
                }
            }

        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs) {

            UpdateChildLocationsAfterSizeChange(new SizeInfo(sizeChangedEventArgs.NewSize) {
                WidthChanged = sizeChangedEventArgs.WidthChanged,
                HeightChanged = sizeChangedEventArgs.HeightChanged,
            });

        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved) {

            if (visualAdded != null) {
                if (IsLoaded) {
                    UpdateHorizontalProportions(visualAdded);
                    UpdateVerticalProportions(visualAdded);
                }
                AttachToPositionChanged(visualAdded);
            }
            if (visualRemoved != null) {
                DettachToPositionChanged(visualRemoved);
            }

            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }

        private void AttachToPositionChanged(DependencyObject visualAdded) {
            var leftDescriptor = DependencyPropertyDescriptor.FromProperty(LeftProperty, typeof(Canvas));
            leftDescriptor.AddValueChanged(visualAdded, OnChildrenLeftChanged);

            var topDescriptor = DependencyPropertyDescriptor.FromProperty(TopProperty, typeof(Canvas));
            topDescriptor.AddValueChanged(visualAdded, OnChildrenTopChanged);
        }

        private void DettachToPositionChanged(DependencyObject visualAdded) {
            var leftDescriptor = DependencyPropertyDescriptor.FromProperty(LeftProperty, typeof(Canvas));
            leftDescriptor.RemoveValueChanged(visualAdded, OnChildrenLeftChanged);

            var topDescriptor = DependencyPropertyDescriptor.FromProperty(TopProperty, typeof(Canvas));
            topDescriptor.RemoveValueChanged(visualAdded, OnChildrenTopChanged);
        }

        private void UpdateHorizontalProportions(DependencyObject dependencyObject) {

            var currentX = GetLeftOfRelativePoint((UIElement)dependencyObject);

            var relativeX = currentX / ActualWidth;
            SetHorizontalProportion(dependencyObject, relativeX);
        }

        private void UpdateVerticalProportions(DependencyObject dependencyObject) {
            var currentY = GetTopOfRelativePoint((UIElement)dependencyObject);

            var relativeY = currentY / ActualHeight;

            if (!double.IsNaN(relativeY))
                SetVerticalProportion(dependencyObject, relativeY);
        }

        private void OnChildrenLeftChanged(object sender, EventArgs eventArgs) {
            var child = (DependencyObject)sender;
            UpdateHorizontalProportions(child);
        }

        private void OnChildrenTopChanged(object sender, EventArgs e) {
            var child = (DependencyObject)sender;
            UpdateVerticalProportions(child);
        }

        private static double GetLeftOfRelativePoint(UIElement uiElement) {

            var renderOrigin = (Point)uiElement.GetValue(RelativeOriginProperty);

            return GetLeft(uiElement) + renderOrigin.X * uiElement.RenderSize.Width;
        }

        private static double GetTopOfRelativePoint(UIElement uiElement) {

            var renderOrigin = (Point)uiElement.GetValue(RelativeOriginProperty);

            return GetTop(uiElement) + renderOrigin.Y * uiElement.RenderSize.Height;
        }

        private static void SetLeftOfRelativePoint(UIElement uiElement, double left) {

            var renderOrigin = (Point)uiElement.GetValue(RelativeOriginProperty);

            SetLeft(uiElement, left - renderOrigin.X * uiElement.RenderSize.Width);
        }

        private static void SetTopOfRelativePoint(UIElement uiElement, double top) {

            var renderOrigin = (Point)uiElement.GetValue(RelativeOriginProperty);

            SetTop(uiElement, top - renderOrigin.Y * uiElement.RenderSize.Height);
        }

        #region HorizontalProportion

        public static readonly DependencyProperty HorizontalProportionProperty =
            DependencyProperty.RegisterAttached("HorizontalProportion", typeof(double), typeof(RelativeCanvas),
                new FrameworkPropertyMetadata(double.NaN,
                    OnHorizontalProportionChanged));

        public static double GetHorizontalProportion(DependencyObject d) {
            return (double)d.GetValue(HorizontalProportionProperty);
        }

        private static void SetHorizontalProportion(DependencyObject d, double value) {
            d.SetValue(HorizontalProportionProperty, value);
        }

        private static void OnHorizontalProportionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var oldHorizontalProportion = (double)e.OldValue;
            var newHorizontalProportion = (double)d.GetValue(HorizontalProportionProperty);
        }

        #endregion

        #region VerticalProportion

        public static readonly DependencyProperty VerticalProportionProperty =
            DependencyProperty.RegisterAttached("VerticalProportion", typeof(double), typeof(RelativeCanvas),
                new FrameworkPropertyMetadata(double.NaN,
                    OnVerticalProportionChanged));

        public static double GetVerticalProportion(DependencyObject d) {
            return (double)d.GetValue(VerticalProportionProperty);
        }

        public static void SetVerticalProportion(DependencyObject d, double value) {
            d.SetValue(VerticalProportionProperty, value);
        }

        private static void OnVerticalProportionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var oldVerticalProportion = (double)e.OldValue;
            var newVerticalProportion = (double)d.GetValue(VerticalProportionProperty);
        }

        #endregion

        #region RelativeOrigin

        public static readonly DependencyProperty RelativeOriginProperty =
            DependencyProperty.RegisterAttached("RelativeOrigin", typeof(Point), typeof(RelativeCanvas),
                new FrameworkPropertyMetadata(new Point(0.5, 0.5),
                    OnRelativeOriginChanged));

        public static Point GetRelativeOrigin(DependencyObject d) {
            return (Point)d.GetValue(RelativeOriginProperty);
        }

        public static void SetRelativeOrigin(DependencyObject d, Point value) {
            d.SetValue(RelativeOriginProperty, value);
        }

        private static void OnRelativeOriginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var oldRelativeOrigin = (Point)e.OldValue;
            var newRelativeOrigin = (Point)d.GetValue(RelativeOriginProperty);
        }

        #endregion

    }
}
