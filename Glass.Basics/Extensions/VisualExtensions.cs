using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Glass.Basics.Extensions
{
    static public class VisualExtensions
    {
        public static Rect GetRectRelativeToParent(this Visual child)
        {
            var rect = VisualTreeHelper.GetDescendantBounds(child);
            var offset = VisualTreeHelper.GetOffset(child);
            var rectRelativeToParent = Rect.Offset(rect, offset);
            return rectRelativeToParent;
        }

        /// <summary>
        /// http://social.msdn.microsoft.com/Forums/en-US/wpf/thread/efaf9cb8-667b-4299-a25e-378ef0b8a02d
        /// </summary>
        /// <param name="visual"></param>
        /// <returns></returns> 
        public static Panel FindItemsPanel(this Visual visual)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
            {
                var child = VisualTreeHelper.GetChild(visual, i) as Visual;

                if (child != null)
                {
                    if (child is Panel && VisualTreeHelper.GetParent(child) is ItemsPresenter)
                    {

                        object temp = child;

                        return (Panel)temp;

                    }

                    var panel = FindItemsPanel(child);

                    if (panel != null)
                    {
                        object temp = panel;
                        return (Panel)temp; // return the panel up the call stack
                    }
                }
            }

            return default(Panel);
        }
    }
}