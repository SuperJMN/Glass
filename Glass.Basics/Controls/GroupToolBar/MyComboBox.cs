using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace Glass.Basics.Controls.GroupToolBar
{
    class MyComboBox : ComboBox
    {
        private Selector parentSelector;
        private bool isRightDown;

        static MyComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MyComboBox), new FrameworkPropertyMetadata(typeof(MyComboBox)));
        }


        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (!IsDropDownOpen)
                mouseButtonEventArgs.Handled = true;

            Focus();
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            ParentSelector = this.TryFindParent<Selector>();

            if (ParentSelector!=null)
            {
                

                IsKeyboardFocusWithinChanged += OnIsKeyboardFocusWithinChanged;

                MouseRightButtonUp += RightClickComboMouseRightButtonUp;
                MouseLeave += RightClickComboMouseLeave;
                PreviewMouseRightButtonDown += RightClickComboPreviewMouseRightButtonDown;
                PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;

                
                SetBinding(SelectedItemProperty, new Binding { Path = new PropertyPath("SelectedItem"), Source = ParentSelector, TargetNullValue = Items[0]});
            }


            base.OnVisualParentChanged(oldParent);
        }

        private void OnIsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ParentSelector.SelectedItem = SelectedItem;
        }

        #region IsChildSelected

        /// <summary>
        /// IsChildSelected Read-Only Dependency Property
        /// </summary>
        private static readonly DependencyPropertyKey IsChildSelectedPropertyKey
            = DependencyProperty.RegisterReadOnly("IsChildSelected", typeof(bool), typeof(MyComboBox),
                new FrameworkPropertyMetadata((bool)false));

        public static readonly DependencyProperty IsChildSelectedProperty
            = IsChildSelectedPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the IsChildSelected property. This dependency property 
        /// indicates ....
        /// </summary>
        public bool IsChildSelected
        {
            get { return (bool)GetValue(IsChildSelectedProperty); }
        }

        private Selector ParentSelector
        {
            get { return parentSelector; }
            set
            {
                if (parentSelector != null)
                    parentSelector.SelectionChanged -= ParentSelectorOnSelectionChanged;

                parentSelector = value;

                if (value!=null)
                    parentSelector.SelectionChanged += ParentSelectorOnSelectionChanged;
            }
        }

        private void ParentSelectorOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            SetIsChildSelected(selectionChangedEventArgs.AddedItems.Contains(SelectedItem));
        }

        private void SetIsChildSelected(bool value)
        {
            SetValue(IsChildSelectedPropertyKey, value);
        }

        #endregion

        void RightClickComboPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            isRightDown = true;
        }

        void RightClickComboMouseLeave(object sender, MouseEventArgs e)
        {
            isRightDown = false;
        }

        void RightClickComboMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isRightDown)
                IsDropDownOpen = true;
            isRightDown = false;
        }
    }
}
