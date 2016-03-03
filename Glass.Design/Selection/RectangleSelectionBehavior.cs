using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace Glass.Design.Selection
{
    public class RectangleSelectionBehavior : Behavior<MultiSelector>
    {
        private Point _dragStart;
        private MouseButton _primaryMouseButton;
        private AdornerLayer _adornerLayer;
        private RectangleSelectionAdorner _adorner;

        protected override void OnAttached()
        {
            AssociatedObject.PreviewMouseDown += AssociatedObjectOnMouseDown;
            AssociatedObject.MouseMove += AssociatedObjectOnMouseMove;
            AssociatedObject.MouseUp += AssociatedObjectOnMouseUp;

            if (AssociatedObject.Background == null)
                AssociatedObject.Background = Brushes.Transparent;

            _primaryMouseButton = (SystemParameters.SwapButtons ? MouseButton.Right : MouseButton.Left);
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseDown -= AssociatedObjectOnMouseDown;
        }


        private void AssociatedObjectOnMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            Debug.WriteLine("RectangleSelectionBehavior_MouseDown");

            _adornerLayer = AdornerLayer.GetAdornerLayer(AssociatedObject);
            if (_adornerLayer != null)
            {
                if (mouseButtonEventArgs.ChangedButton == _primaryMouseButton)
                {

                    var currentPoint = mouseButtonEventArgs.GetPosition(AssociatedObject);
                    _dragStart = currentPoint;

                    var elementUnderMouse = ElementUnderMouse(currentPoint);

                    if (elementUnderMouse!=null)
                    {
                        if (ItemSelectionMode == SelectionMode.Selective)
                        {
                            if (!AssociatedObject.SelectedItems.Contains(elementUnderMouse))
                            AssociatedObject.UnselectAll();
                            AssociatedObject.SelectedItems.Add(elementUnderMouse);
                        }
                    }
                    else
                    {
                        if (!AssociatedObject.IsMouseCaptured)
                            AssociatedObject.CaptureMouse();

                        _adorner = new RectangleSelectionAdorner(AssociatedObject);
                        _adorner.Template = SelectionRectangleTemplate;
                        
                        _adorner.Left = _dragStart.X;
                        _adorner.Top = _dragStart.Y;

                        _adornerLayer.Add(_adorner);

                        if (ItemSelectionMode == SelectionMode.Selective)
                            AssociatedObject.UnselectAll();


                        AssociatedObject.MouseMove += AssociatedObjectOnMouseMove;
                        AssociatedObject.MouseUp += AssociatedObjectOnMouseUp;
                    }
                }


            }
        }

        private FrameworkElement ElementUnderMouse(Point currentPoint)
        {
            foreach (FrameworkElement item in AssociatedObject.Items)
            {
                var bounds = GetBounds(item);
                if (bounds.Contains(currentPoint))
                    return item;
            }
            return null;
        }

        private void AssociatedObjectOnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            if (_adorner != null && mouseEventArgs.LeftButton == MouseButtonState.Pressed)
            {
                var currentPoint = mouseEventArgs.GetPosition(AssociatedObject);

                double left = Math.Min(currentPoint.X, _dragStart.X);
                double top = Math.Min(currentPoint.Y, _dragStart.Y);

                double width = Math.Abs(currentPoint.X - _dragStart.X);
                double height = Math.Abs(currentPoint.Y - _dragStart.Y);

                _adorner.Width = width;
                _adorner.Height = height;
                _adorner.Left = left;
                _adorner.Top = top;

                var coveredItems = GetCoveredItems();
                ApplySelection(coveredItems);
            }
        }

        private void AssociatedObjectOnMouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (_adornerLayer != null && mouseButtonEventArgs.ChangedButton == _primaryMouseButton)
            {
                _adornerLayer.Remove(_adorner);

                AssociatedObject.MouseMove -= AssociatedObjectOnMouseMove;
                AssociatedObject.MouseUp -= AssociatedObjectOnMouseUp;
            }

            if (AssociatedObject.IsMouseCaptured)
                AssociatedObject.ReleaseMouseCapture();


        }

        private void ApplySelection(IList coveredItems)
        {
            switch (ItemSelectionMode)
            {
                case SelectionMode.Selective:

                    foreach (DependencyObject item in AssociatedObject.Items)
                    {
                        if (coveredItems.Contains(item))
                        {
                            if (!AssociatedObject.SelectedItems.Contains(item))
                            {
                                AssociatedObject.SelectedItems.Add(item);
                            }
                        }
                        else
                        {
                            if (AssociatedObject.SelectedItems.Contains(item))
                            {
                                AssociatedObject.SelectedItems.Remove(item);
                            }
                        }
                    }
                    break;
            }
        }

        private IList GetCoveredItems()
        {
            var rect = new Rect(_adorner.Left, _adorner.Top, _adorner.Width, _adorner.Height);

            var covered = new List<FrameworkElement>();
            foreach (FrameworkElement item in AssociatedObject.Items)
            {
                var bounds = GetBounds(item);
                if (rect.Contains(bounds)) covered.Add(item);
            }

            return covered.ToList();
        }

        private Rect GetBounds(FrameworkElement item)
        {
            var transform = item.TransformToAncestor(AssociatedObject);
            var startingPoint = transform.Transform(new Point(0, 0));

            Rect bounds = new Rect(startingPoint, new Size(item.ActualWidth, item.ActualHeight));
            return bounds;
        }

        #region SelectionRectangleTemplate

        /// <summary>
        /// SelectionRectangleTemplate Dependency Property
        /// </summary>
        public static readonly DependencyProperty SelectionRectangleTemplateProperty =
            DependencyProperty.Register("SelectionRectangleTemplate", typeof(ControlTemplate), typeof(RectangleSelectionBehavior),
                new FrameworkPropertyMetadata((ControlTemplate)null));

        /// <summary>
        /// Gets or sets the SelectionRectangleTemplate property. This dependency property 
        /// indicates ....
        /// </summary>
        public ControlTemplate SelectionRectangleTemplate
        {
            get { return (ControlTemplate)GetValue(SelectionRectangleTemplateProperty); }
            set { SetValue(SelectionRectangleTemplateProperty, value); }
        }

        #endregion

        #region ItemSelectionMode

        /// <summary>
        /// ItemSelectionMode Dependency Property
        /// </summary>
        public static readonly DependencyProperty ItemSelectionModeProperty =
            DependencyProperty.Register("ItemSelectionMode", typeof(SelectionMode), typeof(RectangleSelectionBehavior),
                new FrameworkPropertyMetadata(SelectionMode.Selective));

        /// <summary>
        /// Gets or sets the ItemSelectionMode property. This dependency property 
        /// indicates ....
        /// </summary>
        public SelectionMode ItemSelectionMode
        {
            get { return (SelectionMode)GetValue(ItemSelectionModeProperty); }
            set { SetValue(ItemSelectionModeProperty, value); }
        }

        #endregion



        public enum SelectionMode
        {
            Selective,
            Additive,
            Substractive,
            Reversive,
        }


    }
}