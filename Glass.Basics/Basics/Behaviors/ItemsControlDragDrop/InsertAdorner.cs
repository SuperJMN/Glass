using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Glass.Basics.Behaviors.ItemsControlDragDrop
{
    public class InsertAdorner : Adorner
    {
        public bool IsTopHalf { get; set; }
        private AdornerLayer _adornerLayer;
        private Pen _pen;
        private bool _drawHorizontal;

        public InsertAdorner(bool isTopHalf, bool drawHorizontal, UIElement adornedElement, AdornerLayer adornerLayer)
            : base(adornedElement)
        {
            this.IsTopHalf = IsTopHalf;
            _adornerLayer = adornerLayer;
            _drawHorizontal = drawHorizontal;
            _adornerLayer.Add(this);
            _pen = new Pen(new SolidColorBrush(Colors.Red), 3.0);

            DoubleAnimation animation = new DoubleAnimation(0.5, 1, new Duration(TimeSpan.FromSeconds(0.5)));
            animation.AutoReverse = true;
            animation.RepeatBehavior = RepeatBehavior.Forever;
            _pen.Brush.BeginAnimation(Brush.OpacityProperty, animation);
        }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            Point startPoint;
            Point endPoint;

            if (_drawHorizontal)
                DetermineHorizontalLinePoints(out startPoint, out endPoint);
            else
                DetermineVerticalLinePoints(out startPoint, out endPoint);

            drawingContext.DrawLine(_pen, startPoint, endPoint);
        }

        private void DetermineHorizontalLinePoints(out Point startPoint, out Point endPoint)
        {
            double width = this.AdornedElement.RenderSize.Width;
            double height = this.AdornedElement.RenderSize.Height;

            if (!this.IsTopHalf)
            {
                startPoint = new Point(0, height);
                endPoint = new Point(width, height);
            }
            else
            {
                startPoint = new Point(0, 0);
                endPoint = new Point(width, 0);
            }
        }

        private void DetermineVerticalLinePoints(out Point startPoint, out Point endPoint)
        {
            double width = this.AdornedElement.RenderSize.Width;
            double height = this.AdornedElement.RenderSize.Height;

            if (!this.IsTopHalf)
            {
                startPoint = new Point(width, 0);
                endPoint = new Point(width, height);
            }
            else
            {
                startPoint = new Point(0, 0);
                endPoint = new Point(0, height);
            }
        }

        public void Destroy()
        {
            _adornerLayer.Remove(this);
        }
    }
}
