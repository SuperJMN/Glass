using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Glass.Basics.Behaviors.DragDrop
{
    public class ItemsControlDragDropBehavior : Behavior<ItemsControl>
    {
        private bool _isMouseDown;
        private object _data;
        private Point _dragStartPosition;
        private bool _isDragging;
        private DragAdorner _dragAdorner;
        private InsertAdorner _insertAdorner;
        private int _dragScrollWaitCounter;
        private const int DRAG_WAIT_COUNTER_LIMIT = 10;

        public ItemsControlDragDropBehavior()
        {
            _isMouseDown = false;
            _isDragging = false;
            _dragScrollWaitCounter = DRAG_WAIT_COUNTER_LIMIT;
        }

        protected override void OnAttached()
        {
            this.AssociatedObject.AllowDrop = true;
            this.AssociatedObject.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(itemsControl_PreviewMouseLeftButtonDown);
            this.AssociatedObject.PreviewMouseMove += new MouseEventHandler(itemsControl_PreviewMouseMove);
            this.AssociatedObject.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(itemsControl_PreviewMouseLeftButtonUp);
            this.AssociatedObject.PreviewDrop += new DragEventHandler(itemsControl_PreviewDrop);
            this.AssociatedObject.PreviewQueryContinueDrag += new QueryContinueDragEventHandler(itemsControl_PreviewQueryContinueDrag);
            this.AssociatedObject.PreviewDragEnter += new DragEventHandler(itemsControl_PreviewDragEnter);
            this.AssociatedObject.PreviewDragOver += new DragEventHandler(itemsControl_PreviewDragOver);
            this.AssociatedObject.DragLeave += new DragEventHandler(itemsControl_DragLeave);
        }

        #region Properties

        public Type ItemType { get; set; }

        public DataTemplate DataTemplate { get; set; }

        #endregion

        #region Button Events

        void itemsControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ResetState();
        }

        void itemsControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown)
            {
                ItemsControl itemsControl = (ItemsControl)sender;
                Point currentPosition = e.GetPosition(itemsControl);
                if ((_isDragging == false) && (Math.Abs(currentPosition.X - _dragStartPosition.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                    (Math.Abs(currentPosition.Y - _dragStartPosition.Y) > SystemParameters.MinimumVerticalDragDistance))
                {
                    DragStarted(itemsControl);
                }
            }
        }

        void itemsControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ItemsControl itemsControl = (ItemsControl)sender;
            Point p = e.GetPosition(itemsControl);
            _data = Helper.GetDataObjectFromItemsControl(itemsControl, p);
            if (_data != null)
            {
                _isMouseDown = true;
                _dragStartPosition = p;
            }
        }

        #endregion

        #region Drag Events

        void itemsControl_DragLeave(object sender, DragEventArgs e)
        {
            DetachAdorners();
            e.Handled = true;
        }

        void itemsControl_PreviewDragOver(object sender, DragEventArgs e)
        {
            ItemsControl itemsControl = (ItemsControl)sender;
            if (e.Data.GetDataPresent(ItemType))
            {
                UpdateDragAdorner(e.GetPosition(itemsControl));
                UpdateInsertAdorner(itemsControl, e);
                HandleDragScrolling(itemsControl, e);
            }
            e.Handled = true;
        }

        void itemsControl_PreviewDragEnter(object sender, DragEventArgs e)
        {
            ItemsControl itemsControl = (ItemsControl)sender;
            if (e.Data.GetDataPresent(ItemType))
            {
                object data = e.Data.GetData(ItemType);
                InitializeDragAdorner(itemsControl, data, e.GetPosition(itemsControl));
                InitializeInsertAdorner(itemsControl, e);
            }
            e.Handled = true;
        }

        void itemsControl_PreviewQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (e.EscapePressed)
            {
                e.Action = DragAction.Cancel;
                ResetState();
                DetachAdorners();
                e.Handled = true;
            }
        }

        void itemsControl_PreviewDrop(object sender, DragEventArgs e)
        {
            ItemsControl itemsControl = (ItemsControl)sender;
            DetachAdorners();
            e.Handled = true;
            if (e.Data.GetDataPresent(ItemType))
            {
                object itemToAdd = e.Data.GetData(ItemType);
                if ((e.KeyStates & DragDropKeyStates.ControlKey) != 0 &&
                    Helper.DoesItemExists(itemsControl, itemToAdd))
                {
                    if (MessageBox.Show("Item already exists. Do you want to overwrite it?", "Copy File",
                        MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        Helper.RemoveItem(itemsControl, itemToAdd);
                    }
                    else
                    {
                        e.Effects = DragDropEffects.None;
                        return;
                    }
                }
                e.Effects = ((e.KeyStates & DragDropKeyStates.ControlKey) != 0) ?
                            DragDropEffects.Copy : DragDropEffects.Move;
                Helper.AddItem(itemsControl, itemToAdd, FindInsertionIndex(itemsControl, e));
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        #endregion

        #region Private Methods

        private void DragStarted(ItemsControl itemsControl)
        {
            Type dataType = _data.GetType();

            if (!(dataType.IsSubclassOf(ItemType) || dataType == ItemType || ItemType.IsAssignableFrom(dataType)))
            {
                return;
            }

            UIElement draggedItemContainer = Helper.GetItemContainerFromPoint(itemsControl, _dragStartPosition);
            _isDragging = true;
            DataObject dObject = new DataObject(ItemType, _data);
            DragDropEffects e = System.Windows.DragDrop.DoDragDrop(itemsControl, dObject, DragDropEffects.Copy | DragDropEffects.Move);
            if ((e & DragDropEffects.Move) != 0)
            {
                if (draggedItemContainer != null)
                {
                    int dragItemIndex = itemsControl.ItemContainerGenerator.IndexFromContainer(draggedItemContainer);
                    Helper.RemoveItem(itemsControl, dragItemIndex);
                }
                else
                {
                    Helper.RemoveItem(itemsControl, _data);
                }
            }
            ResetState();
        }

        private void HandleDragScrolling(ItemsControl itemsControl, DragEventArgs e)
        {
            bool? isMouseAtTop = Helper.IsMousePointerAtTop(itemsControl, e.GetPosition(itemsControl));
            if (isMouseAtTop.HasValue)
            {
                if (_dragScrollWaitCounter == DRAG_WAIT_COUNTER_LIMIT)
                {
                    _dragScrollWaitCounter = 0;
                    System.Diagnostics.Debug.WriteLine(DateTime.Now.Ticks);
                    ScrollViewer scrollViewer = Helper.FindScrollViewer(itemsControl);
                    if (scrollViewer != null && scrollViewer.CanContentScroll
                        && scrollViewer.ComputedVerticalScrollBarVisibility == Visibility.Visible)
                    {
                        if (isMouseAtTop.Value)
                        {
                            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - 1.0);

                        }
                        else
                        {
                            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + 1.0);
                        }
                        e.Effects = DragDropEffects.Scroll;
                    }
                }
                else
                {
                    _dragScrollWaitCounter++;
                }
            }
            else
            {
                e.Effects = ((e.KeyStates & DragDropKeyStates.ControlKey) != 0) ?
                               DragDropEffects.Copy : DragDropEffects.Move;
            }
        }

        private int FindInsertionIndex(ItemsControl itemsControl, DragEventArgs e)
        {
            UIElement dropTargetContainer = Helper.GetItemContainerFromPoint(itemsControl, e.GetPosition(itemsControl));
            if (dropTargetContainer != null)
            {
                int index = itemsControl.ItemContainerGenerator.IndexFromContainer(dropTargetContainer);
                if (Helper.IsPointInTopHalf(itemsControl, e))
                    return index;
                else
                    return index + 1;
            }
            return itemsControl.Items.Count;

        }

        private void ResetState()
        {
            _isMouseDown = false;
            _isDragging = false;
            _data = null;
            _dragScrollWaitCounter = DRAG_WAIT_COUNTER_LIMIT;
        }

        private void InitializeDragAdorner(ItemsControl itemsControl, object dragData, Point startPosition)
        {
            if (this.DataTemplate != null)
            {
                if (_dragAdorner == null)
                {
                    var adornerLayer = AdornerLayer.GetAdornerLayer(itemsControl);
                    _dragAdorner = new DragAdorner(dragData, DataTemplate, itemsControl, adornerLayer);
                    _dragAdorner.UpdatePosition(startPosition.X, startPosition.Y);
                }
            }
        }

        private void UpdateDragAdorner(Point currentPosition)
        {
            if (_dragAdorner != null)
            {
                _dragAdorner.UpdatePosition(currentPosition.X, currentPosition.Y);
            }
        }

        private void InitializeInsertAdorner(ItemsControl itemsControl, DragEventArgs e)
        {
            if (_insertAdorner == null)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(itemsControl);
                UIElement itemContainer = Helper.GetItemContainerFromPoint(itemsControl, e.GetPosition(itemsControl));
                if (itemContainer != null)
                {
                    bool isPointInTopHalf = Helper.IsPointInTopHalf(itemsControl, e);
                    bool isItemsControlOrientationHorizontal = Helper.IsItemControlOrientationHorizontal(itemsControl);
                    _insertAdorner = new InsertAdorner(isPointInTopHalf, isItemsControlOrientationHorizontal, itemContainer, adornerLayer);
                }
            }
        }

        private void UpdateInsertAdorner(ItemsControl itemsControl, DragEventArgs e)
        {
            if (_insertAdorner != null)
            {
                _insertAdorner.IsTopHalf = Helper.IsPointInTopHalf(itemsControl, e);
                _insertAdorner.InvalidateVisual();
            }
        }

        private void DetachAdorners()
        {
            if (_insertAdorner != null)
            {
                _insertAdorner.Destroy();
                _insertAdorner = null;
            }
            if (_dragAdorner != null)
            {
                _dragAdorner.Destroy();
                _dragAdorner = null;
            }
        }

        #endregion
    }
}
