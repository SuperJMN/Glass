using System.Windows;

namespace Glass.Basics.Behaviors.RubberBand
{
    public class RectEventArgs : RoutedEventArgs
    {
        public RectEventArgs(Rect rect)
        {
            Rect = rect;
        }

        private Rect Rect { get; set; }
    }
}