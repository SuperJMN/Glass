using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Glass.Basics;

namespace Glass.Design.Designer
{

    public class DesignerItem : ContentControl, IDesignerItem
    {

        private DragControl dragControl;

        static DesignerItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DesignerItem), new FrameworkPropertyMetadata((typeof(DesignerItem))));
        }

        public DesignerItem()
        {
            AttachHandlersToLeftAndTop();
            base.SizeChanged += OnSizeChanged;
            GotFocus += OnGotFocus;
            PreviewKeyDown += OnPreviewKeyDown;
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            parentWindow = this.TryFindParent<Window>();
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key == Key.Escape && IsEditing)
            {
                IsEditing = false;                
            }
        }

        private void RemoveParentWindowHandler()
        {
            parentWindow.PreviewMouseDown -= WindowOnPreviewMouseDown;
        }

        private void OnGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            if (IsSelected)
            {
                IsEditing = true;
            }
        }

        private void AttachHandlersToLeftAndTop()
        {
            var targetType = GetType();

            var leftProperty = DependencyPropertyDescriptor.FromProperty(Canvas.LeftProperty, targetType);
            leftProperty.AddValueChanged(this, OnCanvasLeftChanged);

            var topProperty = DependencyPropertyDescriptor.FromProperty(Canvas.TopProperty, targetType);
            topProperty.AddValueChanged(this, OnCanvasTopChanged);
        }

        private void OnCanvasLeftChanged(object sender, EventArgs eventArgs)
        {
            RaiseLocationChanged();
        }

        private void OnCanvasTopChanged(object sender, EventArgs eventArgs)
        {
            RaiseLocationChanged();
        }



        private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            RaiseSizeChanged();
        }

        #region IsSelected

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(DesignerItem),
                new FrameworkPropertyMetadata(false, OnIsSelectedChanged));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public event EventHandler<bool> IsSelectedChanged;

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (DesignerItem)d;
            target.OnIsSelectedChanged();
        }


        private void OnIsSelectedChanged()
        {
            //var content = (FrameworkElement)Content;
            //content.Focusable = false;
            RaiseIsSelectedChanged();

        }

        #endregion

        #region IsEditing

        public static readonly DependencyProperty IsEditingProperty =
            DependencyProperty.Register("IsEditing", typeof(bool), typeof(DesignerItem),
                new FrameworkPropertyMetadata((bool)false,
                    new PropertyChangedCallback(OnIsEditingChanged)));

        public bool IsEditing
        {
            get { return (bool)GetValue(IsEditingProperty); }
            set { SetValue(IsEditingProperty, value); }
        }

        private static void OnIsEditingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (DesignerItem)d;
            var oldIsEditing = (bool)e.OldValue;
            var newIsEditing = target.IsEditing;
            target.OnIsEditingChanged(oldIsEditing, newIsEditing);
        }

        protected virtual void OnIsEditingChanged(bool oldIsEditing, bool newIsEditing)
        {
            if (newIsEditing)
            {                
                AddParentWindowHandler();                
            }
            else
            {                
                RemoveParentWindowHandler();                
            }
        }

        private void AddParentWindowHandler()
        {
            parentWindow.PreviewMouseDown += WindowOnPreviewMouseDown;
        }

        private void WindowOnPreviewMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            IsEditing = false;
        }

        #endregion

        private void RaiseIsSelectedChanged()
        {
            if (IsSelectedChanged != null)
            {
                IsSelectedChanged(this, IsSelected);
            }
        }

        public new event EventHandler SizeChanged;
        private void RaiseSizeChanged()
        {
            if (SizeChanged != null)
                SizeChanged(this, EventArgs.Empty);
        }

        public event EventHandler LocationChanged;
        private void RaiseLocationChanged()
        {
            if (LocationChanged != null)
                LocationChanged(this, EventArgs.Empty);
        }

        public Point AnchorPoint { get; set; }

        #region Left

        public static readonly DependencyProperty LeftProperty =
            DependencyProperty.Register("Left", typeof(double), typeof(DesignerItem),
                new FrameworkPropertyMetadata((double)0.0,
                    new PropertyChangedCallback(OnLeftChanged)));

        public double Left
        {
            get { return (double)GetValue(LeftProperty); }
            set { SetValue(LeftProperty, value); }
        }

        private static void OnLeftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (DesignerItem)d;
            var oldLeft = (double)e.OldValue;
            var newLeft = target.Left;
            target.OnLeftChanged(oldLeft, newLeft);
        }

        protected virtual void OnLeftChanged(double oldLeft, double newLeft)
        {
            Canvas.SetLeft(this, newLeft);
        }

        #endregion

        #region Top

        public static readonly DependencyProperty TopProperty =
            DependencyProperty.Register("Top", typeof(double), typeof(DesignerItem),
                new FrameworkPropertyMetadata(0.0,
                    new PropertyChangedCallback(OnTopChanged)));

        private Window parentWindow;


        public double Top
        {
            get { return (double)GetValue(TopProperty); }
            set { SetValue(TopProperty, value); }
        }

        private static void OnTopChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (DesignerItem)d;
            var oldTop = (double)e.OldValue;
            var newTop = target.Top;
            target.OnTopChanged(oldTop, newTop);
        }

        protected virtual void OnTopChanged(double oldTop, double newTop)
        {
            Canvas.SetTop(this, newTop);
        }

        #endregion

        public override void OnApplyTemplate()
        {
            dragControl = (DragControl)Template.FindName("PART_DragControl", this);
            dragControl.Designable = this;

            base.OnApplyTemplate();
        }

        public void SetDraggable(IDesignable designable)
        {
            dragControl.Designable = designable;
        }
    }
}