using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Glass.Basics.Behaviors.DragDrop
{
    public static class Helper
    {
        public static bool DoesItemExists(ItemsControl itemsControl, object item)
        {
            if (itemsControl.Items.Count > 0)
            {
                return itemsControl.Items.Contains(item);
            }
            return false;
        }

        public static void AddItem(ItemsControl itemsControl, object item, int insertIndex)
        {
            if (itemsControl.ItemsSource != null)
            {
                IList iList = itemsControl.ItemsSource as IList;
                if (iList != null)
                {
                    iList.Insert(insertIndex, item);
                }
                else
                {
                    Type type = itemsControl.ItemsSource.GetType();
                    Type genericList = type.GetInterface("IList`1");
                    if (genericList != null)
                    {
                        type.GetMethod("Insert").Invoke(itemsControl.ItemsSource, new object[] { insertIndex, item });
                    }
                }
            }
            else
            {
                itemsControl.Items.Insert(insertIndex, item);
            }
        }

        public static void RemoveItem(ItemsControl itemsControl, object itemToRemove)
        {
            if (itemToRemove != null)
            {
                int index = itemsControl.Items.IndexOf(itemToRemove);
                if (index != -1)
                {
                    RemoveItem(itemsControl, index);
                }
            }
        }

        public static void RemoveItem(ItemsControl itemsControl, int removeIndex)
        {
            if (removeIndex != -1 && removeIndex < itemsControl.Items.Count)
            {
                if (itemsControl.ItemsSource != null)
                {
                    IList iList = itemsControl.ItemsSource as IList;
                    if (iList != null)
                    {
                        iList.RemoveAt(removeIndex);
                    }
                    else
                    {
                        Type type = itemsControl.ItemsSource.GetType();
                        Type genericList = type.GetInterface("IList`1");
                        if (genericList != null)
                        {
                            type.GetMethod("RemoveAt").Invoke(itemsControl.ItemsSource, new object[] { removeIndex });
                        }
                    }
                }
                else
                {
                    itemsControl.Items.RemoveAt(removeIndex);
                }
            }
        }

        public static object GetDataObjectFromItemsControl(ItemsControl itemsControl, Point p)
        {
            UIElement element = itemsControl.InputHitTest(p) as UIElement;
            while (element != null)
            {
                if (element == itemsControl)
                    return null;

                object data = itemsControl.ItemContainerGenerator.ItemFromContainer(element);
                if (data != DependencyProperty.UnsetValue)
                {
                    return data;
                }
                else
                {
                    element = VisualTreeHelper.GetParent(element) as UIElement;
                }
            }
            return null;
        }

        public static UIElement GetItemContainerFromPoint(ItemsControl itemsControl, Point p)
        {
            UIElement element = itemsControl.InputHitTest(p) as UIElement;
            while (element != null)
            {
                if (element == itemsControl)
                    return null;

                object data = itemsControl.ItemContainerGenerator.ItemFromContainer(element);
                if (data != DependencyProperty.UnsetValue)
                {
                    return element;
                }
                else
                {
                    element = VisualTreeHelper.GetParent(element) as UIElement;
                }
            }
            return null;
        }

        public static UIElement GetItemContainerFromItemsControl(ItemsControl itemsControl)
        {
            UIElement container = null;
            if (itemsControl != null && itemsControl.Items.Count > 0)
            {
                container = itemsControl.ItemContainerGenerator.ContainerFromIndex(0) as UIElement;
            }
            else
            {
                container = itemsControl;
            }
            return container;
        }

        public static bool IsPointInTopHalf(ItemsControl itemsControl, DragEventArgs e)
        {
            UIElement selectedItemContainer = GetItemContainerFromPoint(itemsControl, e.GetPosition(itemsControl));
            Point relativePosition = e.GetPosition(selectedItemContainer);
            if (IsItemControlOrientationHorizontal(itemsControl))
            {
                return relativePosition.Y < ((FrameworkElement)selectedItemContainer).ActualHeight / 2;
            }
            return relativePosition.X < ((FrameworkElement)selectedItemContainer).ActualWidth / 2;
        }

        public static bool IsItemControlOrientationHorizontal(ItemsControl itemsControl)
        {
            if (itemsControl is TabControl)
                return false;
            return true;
        }

        public static bool? IsMousePointerAtTop(ItemsControl itemsControl, Point pt)
        {
            if (pt.Y > 0.0 && pt.Y < 25)
            {
                return true;
            }
            else if (pt.Y > itemsControl.ActualHeight - 25 && pt.Y < itemsControl.ActualHeight)
            {
                return false;
            }
            return null;
        }

        public static ScrollViewer FindScrollViewer(ItemsControl itemsControl)
        {
            UIElement ele = itemsControl;
            while (ele != null)
            {
                if (VisualTreeHelper.GetChildrenCount(ele) == 0)
                {
                    ele = null;
                }
                else
                {
                    ele = VisualTreeHelper.GetChild(ele, 0) as UIElement;
                    if (ele != null && ele is ScrollViewer)
                        return ele as ScrollViewer;
                }
            }
            return null;
        }


        public static double ScrollOffsetUp(double verticaloffset, double offset)
        {
            return verticaloffset - offset < 0.0 ? 0.0 : verticaloffset - offset;
        }
    }
}
