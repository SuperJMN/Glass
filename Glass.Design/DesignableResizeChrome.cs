using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Glass.Basics;

namespace Glass.Design
{

    public class DesignableResizeChrome : Control
    {

        private readonly IDesignable designable;

        static DesignableResizeChrome()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DesignableResizeChrome), new FrameworkPropertyMetadata(typeof(DesignableResizeChrome)));
        }

        public DesignableResizeChrome() {
            
        }

        public DesignableResizeChrome(IDesignable designable)
        {
            this.designable = designable;
        }


        public override void OnApplyTemplate()
        {
            var thumbs = this.GetDescendants<Thumb>();

            foreach (var thumb in thumbs)
            {               
                var hook = GetHookPointFromAlignments(thumb.HorizontalAlignment, thumb.VerticalAlignment);
                if (IsASideHook(hook)) {
                    thumb.DragStarted += (sender, args) => IsResizing = true;
                    thumb.DragCompleted += (sender, args) => IsResizing = false;
                }                
                
                thumb.DragDelta += ThumbOnDragDelta;
            }

            base.OnApplyTemplate();
        }

        private void ThumbOnDragDelta(object sender, DragDeltaEventArgs dragDeltaEventArgs)
        {
            var thumb = (Thumb)sender;

            var hook = GetHookPointFromAlignments(thumb.HorizontalAlignment, thumb.VerticalAlignment);

            if (IsASideHook(hook))
            {
                designable.AnchorPoint = hook;

                if (IsHorizontalHook(hook))
                {
                    var widthDelta = (0.5 - hook.X) * 2 * dragDeltaEventArgs.HorizontalChange;
                    designable.Width += widthDelta ;
                }

                if (IsVerticalHook(hook))
                {
                    var heightDelta = (0.5 - hook.Y) * 2 * dragDeltaEventArgs.VerticalChange;
                    designable.Height += heightDelta;
                }
            }
            else
            {
                var leftDelta = dragDeltaEventArgs.HorizontalChange;
                var topDelta = dragDeltaEventArgs.VerticalChange;

                designable.Left += leftDelta;
                designable.Top += topDelta;
            }

            dragDeltaEventArgs.Handled = true;
        }

        private static bool IsASideHook(Point hook)
        {
            return IsHorizontalHook(hook) || IsVerticalHook(hook);
        }

        private static bool IsVerticalHook(Point hook)
        {
            const double epsilon = 0.01;
            return Math.Abs(hook.Y - 0.5) > epsilon;
        }

        private static bool IsHorizontalHook(Point hook)
        {
            const double epsilon = 0.01;
            return Math.Abs(hook.X - 0.5) > epsilon;
        }

        private static Point GetHookPointFromAlignments(HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
        {
            Point hook = new Point(0.5, 0.5);
            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    hook.X = 1;
                    break;
                case HorizontalAlignment.Right:
                    hook.X = 0;
                    break;
            }

            switch (verticalAlignment)
            {
                case VerticalAlignment.Top:
                    hook.Y = 1;
                    break;
                case VerticalAlignment.Bottom:
                    hook.Y = 0;
                    break;
            }
            return hook;
        }

        #region IsResizing
        public static readonly DependencyProperty IsResizingProperty =
          DependencyProperty.Register("IsResizing", typeof(bool), typeof(DesignableResizeChrome),
            new FrameworkPropertyMetadata(false));

        public bool IsResizing {
            get { return (bool)GetValue(IsResizingProperty); }
            set { SetValue(IsResizingProperty, value); }
        }

        #endregion


    }
}
