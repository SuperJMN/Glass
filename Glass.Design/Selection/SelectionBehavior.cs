using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using Glass.Basics;

namespace Glass.Design.Selection
{
    public class SelectionBehavior : Behavior<ItemsControl>
    {
        private Point _dragStart;
        private AdornerLayer _adornerLayer;
        private SelectionAdorner _adorner;
        private bool _isDragging;

        protected override void OnAttached()
        {
            SetMouseHandlers(true);
        }

        protected override void OnDetaching()
        {
            SetMouseHandlers(false);
            _isDragging = false;
        }

        private void AssociatedObjectOnMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            // Si ya se está llevando a cabo una operación de arrastre, ¡huye!
            if (_isDragging) return;

            if (PrimaryButtonState(mouseButtonEventArgs) == MouseButtonState.Pressed)
            {
                _dragStart = mouseButtonEventArgs.GetPosition(AssociatedObject);
                _isDragging = true;

                _adornerLayer = AdornerLayer.GetAdornerLayer(AssociatedObject);

                if (_adornerLayer == null) throw new NullReferenceException("No se ha podido obtener la Adorner Layer");


                AssociatedObject.CaptureMouse();
                _adorner = new SelectionAdorner(AssociatedObject)
                               {Template = SelectionRectangleTemplate, Left = _dragStart.X, Top = _dragStart.Y};
                _adornerLayer.Add(_adorner);
            }
        }

        private void AssociatedObjectOnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            if (_adorner != null && PrimaryButtonState(mouseEventArgs) == MouseButtonState.Pressed && _isDragging)
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
            }
        }

        private void AssociatedObjectOnPreviewMouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (PrimaryButtonState(mouseButtonEventArgs) == MouseButtonState.Released && _isDragging)
            {
                _adornerLayer.Remove(_adorner);
                AssociatedObject.ReleaseMouseCapture();

                var coveredItems = GetCoveredItems();
                foreach (FrameworkElement item in AssociatedObject.Items)
                {
                    if (coveredItems.Contains(item))
                    {
                        if (!SelectedItems.Contains(item))
                        {
                            SelectedItems.Add(item);
                        }
                    }

                    if (!coveredItems.Contains(item))
                    {
                        if (SelectedItems.Contains(item))
                        {
                            SelectedItems.Remove(item);
                        }
                    }

                }


                _isDragging = false;

                // Finalmente, lanzamos el evento de finalización de selección
                RaiseSelectionChangedEvent(AssociatedObject, SelectedItems);

            }
        }

        private FrameworkElement ElementAt(Point location)
        {
            foreach (var item in AssociatedObject.Items)
            {
                
            }
            return null;
        }

        private IList<FrameworkElement> GetCoveredItems()
        {
            var rect = new Rect(_adorner.Left, _adorner.Top, _adorner.Width, _adorner.Height);

            var lista = (from FrameworkElement item in AssociatedObject.Items
                    let vector = VisualTreeHelper.GetOffset(item)
                    let bounds = new Rect(new Point(vector.X, vector.Y), new Size(item.ActualWidth, item.ActualHeight))
                    where rect.Contains(bounds)
                    select item);
            return new Collection<FrameworkElement>(lista.ToList());
        }

        #region SelectionRectangleTemplate

        /// <summary>
        /// SelectionRectangleTemplate Dependency Property
        /// </summary>
        public static readonly DependencyProperty SelectionRectangleTemplateProperty =
            DependencyProperty.Register("SelectionRectangleTemplate", typeof(ControlTemplate), typeof(SelectionBehavior),
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
            DependencyProperty.Register("ItemSelectionMode", typeof(SelectionMode), typeof(SelectionBehavior),
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

        #region Enabled

        /// <summary>
        /// Enabled Dependency Property
        /// </summary>
        public static readonly DependencyProperty EnabledProperty =
            DependencyProperty.Register("Enabled", typeof(bool), typeof(SelectionBehavior),
                new FrameworkPropertyMetadata(true,
                    OnEnabledChanged));

        /// <summary>
        /// Gets or sets the Enabled property. This dependency property 
        /// indicates ....
        /// </summary>
        public bool Enabled
        {
            get { return (bool)GetValue(EnabledProperty); }
            set { SetValue(EnabledProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Enabled property.
        /// </summary>
        private static void OnEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SelectionBehavior target = (SelectionBehavior)d;
            bool oldEnabled = (bool)e.OldValue;
            bool newEnabled = target.Enabled;
            target.OnEnabledChanged(oldEnabled, newEnabled);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Enabled property.
        /// </summary>
        protected virtual void OnEnabledChanged(bool oldEnabled, bool newEnabled)
        {
            SetMouseHandlers(newEnabled);
        }

        private void SetMouseHandlers(bool newEnabled)
        {
            if (newEnabled)
            {
                AssociatedObject.MouseDown += AssociatedObjectOnMouseDown;
                AssociatedObject.PreviewMouseMove += AssociatedObjectOnMouseMove;
                AssociatedObject.PreviewMouseUp += AssociatedObjectOnPreviewMouseUp;
            }
            else
            {
                AssociatedObject.MouseDown -= AssociatedObjectOnMouseDown;
                AssociatedObject.PreviewMouseMove -= AssociatedObjectOnMouseMove;
                AssociatedObject.PreviewMouseUp -= AssociatedObjectOnPreviewMouseUp;
            }
        }

        #endregion

        private MouseButtonState PrimaryButtonState(MouseEventArgs e)
        {
            return SystemParameters.SwapButtons
                       ? e.RightButton
                       : e.LeftButton;
        }

        public enum SelectionMode
        {
            Selective,
            Additive,
            Substractive,
            Reversive,
        }

        #region SelectedItems

        /// <summary>
        /// SelectedItems Dependency Property
        /// </summary>
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(IList<FrameworkElement>), typeof(SelectionBehavior),
                new FrameworkPropertyMetadata(new List<FrameworkElement>(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Gets or sets the SelectedItems property. This dependency property 
        /// indicates ....
        /// </summary>
        public IList<FrameworkElement> SelectedItems
        {
            get { return (IList<FrameworkElement>)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        #endregion

        #region SelectionChanged

        /// <summary>
        /// SelectionChanged Attached Routed Event
        /// </summary>
        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent("SelectionChanged",
            RoutingStrategy.Bubble, typeof(SelectionEventHandler), typeof(SelectionBehavior));

        /// <summary>
        /// Adds a handler for the SelectionChanged attached event
        /// </summary>
        /// <param name="element">UIElement or ContentElement that listens to the event</param>
        /// <param name="handler">Event handler to be added</param>
        public static void AddSelectionChangedHandler(DependencyObject element, SelectionEventHandler handler)
        {
            RoutedEventHelper.AddHandler(element, SelectionChangedEvent, handler);
        }

        /// <summary>
        /// Removes a handler for the SelectionChanged attached event
        /// </summary>
        /// <param name="element">UIElement or ContentElement that listens to the event</param>
        /// <param name="handler">Event handler to be removed</param>
        public static void RemoveSelectionChangedHandler(DependencyObject element, SelectionEventHandler handler)
        {
            RoutedEventHelper.RemoveHandler(element, SelectionChangedEvent, handler);
        }

        /// <summary>
        /// A static helper method to raise the SelectionChanged event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        /// <param name="arg"> </param>
        internal static SelectionEventArgs RaiseSelectionChangedEvent(DependencyObject target, IList<FrameworkElement> arg)
        {
            if (target == null) return null;

            var args = new SelectionEventArgs {Selection = arg};
            args.RoutedEvent = SelectionChangedEvent;
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        #endregion

        public delegate void SelectionEventHandler(object sender, SelectionEventArgs e);

        public class SelectionEventArgs : RoutedEventArgs
        {
            public IList<FrameworkElement> Selection { get; set; }
        }

    }
}