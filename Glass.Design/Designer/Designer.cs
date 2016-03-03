using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using Glass.Basics;

namespace Glass.Design.Designer
{
    public class Designer : MultiSelector
    {
        private Canvas canvas;
        private readonly DesignableGroup selection;
        private Adorner selectionAdorner;
        private AdornerLayer adornerlayer;

        static Designer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Designer), new FrameworkPropertyMetadata((typeof(Designer))));
        }

        public Designer()
        {
            selection = new DesignableGroup();

            Loaded += OnLoaded;
            MouseDown += OnMouseDown;
            SelectionChanged += OnSelectionChanged;
        }

        public IDesignable Selection
        {
            get
            {
                return selection;
            }
        }        

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            foreach (var addedItem in selectionChangedEventArgs.AddedItems)
            {
                var container = ItemContainerGenerator.ContainerFromItem(addedItem) ??
                                this.ContainerFromElement((DependencyObject) addedItem);

                if (selection.Items.Count == 0) {
                    adornerlayer.Add(selectionAdorner);
                }
                selection.Items.Add((IDesignable)container);
            }

            foreach (var addedItem in selectionChangedEventArgs.RemovedItems)
            {
                var container = ItemContainerGenerator.ContainerFromItem(addedItem) ??
                                this.ContainerFromElement((DependencyObject)addedItem);

                var designerItem = (DesignerItem)container;
                designerItem.IsSelected = false;

                adornerlayer.Remove(selectionAdorner);
                selection.Items.Clear();
            }
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            UnselectAll();
            Focus();
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            canvas = this.GetVisualChild<Canvas>();
            adornerlayer = AdornerLayer.GetAdornerLayer(this);
            var designableDragAndResizeChrome = new DesignableResizeChrome(selection);
            selectionAdorner = new DesignableResizeAdorner(canvas, selection, designableDragAndResizeChrome);
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is DesignerItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DesignerItem();
        }
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            var designerItem = (DesignerItem)element;
            designerItem.IsSelectedChanged += DesignerItemOnIsSelectedChanged;
            designerItem.PreviewMouseLeftButtonDown += DesignerItemOnPreviewMouseLeftButtonDown;
            designerItem.PreviewMouseDoubleClick += DesignerItemOnPreviewMouseDoubleClick;
            designerItem.Loaded += DesignerItemOnLoaded;
           
            base.PrepareContainerForItemOverride(element, item);
        }

        private void DesignerItemOnLoaded(object sender, RoutedEventArgs routedEventArgs) {
            var designable = (DesignerItem)sender;
            designable.SetDraggable(selection);
        }

        private void DesignerItemOnPreviewMouseDoubleClick(object sender, MouseButtonEventArgs mouseButtonEventArgs) {
            var designerItem = (DesignerItem) sender;
            designerItem.Focusable = true;
            designerItem.Focus();
        }

        private void DesignerItemOnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs) {

            

            var designerItem = (DesignerItem) sender;

            if (designerItem.IsSelected)
                return;
            
            if (!Keyboard.IsKeyDown(Key.LeftCtrl))
                UnselectAll();

            designerItem.IsSelected = true;
            
        }

        private void AttachHandlersToLeftAndTop(object item)
        {
            var leftProperty = DependencyPropertyDescriptor.FromProperty(Canvas.LeftProperty, item.GetType());
            leftProperty.AddValueChanged(item, OnCanvasLeftChanged);

            var rightProperty = DependencyPropertyDescriptor.FromProperty(Canvas.LeftProperty, item.GetType());
            rightProperty.AddValueChanged(item, OnCanvasTopChanged);
        }

        private void OnCanvasLeftChanged(object sender, EventArgs eventArgs)
        {
            var subject = sender as FrameworkElement;
            var container = ItemContainerGenerator.ContainerFromItem(subject);
            container.SetValue(Canvas.LeftProperty, subject.GetValue(Canvas.LeftProperty));
        }

        private void OnCanvasTopChanged(object sender, EventArgs eventArgs)
        {
            var subject = sender as FrameworkElement;
            var container = ItemContainerGenerator.ContainerFromItem(subject);
            container.SetValue(Canvas.TopProperty, subject.GetValue(Canvas.TopProperty));
        }

        private void DesignerItemOnIsSelectedChanged(object sender, bool b)
        {
            var designerItem = (DesignerItem)sender;

            BeginUpdateSelectedItems();

            if (designerItem.IsSelected) {
                this.SelectedItems.Add(designerItem.Content);
            } else {
                this.SelectedItems.Remove(designerItem.Content);

            }
            EndUpdateSelectedItems();
        }


    }
}