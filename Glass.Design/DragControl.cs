using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Glass.Basics;

namespace Glass.Design {
    public class DragControl : Control {

        private Thumb dragThumb;

        static DragControl() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DragControl), new FrameworkPropertyMetadata(typeof(DragControl)));
        }

        public DragControl() {
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs) {

            if (Designable==null) {
                Designable = VisualTreeUtils.TryFindParent<IDesignable>(this);
                if (Designable==null)
                    throw new NullReferenceException();
            }
        }

        public override void OnApplyTemplate() {

            dragThumb = (Thumb) Template.FindName("PART_DragThumb", this);
            dragThumb.DragDelta += DragThumbOnDragDelta;

            base.OnApplyTemplate();
        }

        #region Designable
        public static readonly DependencyProperty DesignableProperty =
          DependencyProperty.Register("Designable", typeof(IDesignable), typeof(DragControl),
            new FrameworkPropertyMetadata((IDesignable)null));

        public IDesignable Designable {
            get { return (IDesignable)GetValue(DesignableProperty); }
            set { SetValue(DesignableProperty, value); }
        }

        #endregion

        private void DragThumbOnDragDelta(object sender, DragDeltaEventArgs dragDeltaEventArgs) {
            Designable.Left += dragDeltaEventArgs.HorizontalChange;
            Designable.Top += dragDeltaEventArgs.VerticalChange;
            dragDeltaEventArgs.Handled = true;
        }
    }
}
