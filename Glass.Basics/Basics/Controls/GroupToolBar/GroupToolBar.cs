using System.Windows;
using System.Windows.Controls;

namespace Glass.Basics.Controls.GroupToolBar
{
    public class GroupToolBar : ToolBar
    {
        static GroupToolBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GroupToolBar), new FrameworkPropertyMetadata(typeof(GroupToolBar)));
        }

        #region SelectedTool

        /// <summary>
        /// SelectedTool Dependency Property
        /// </summary>
        public static readonly DependencyProperty SelectedToolProperty = DependencyProperty.Register("SelectedTool", typeof(object), typeof(GroupToolBar), new FrameworkPropertyMetadata(null));
        /// <summary>
        /// Gets or sets the SelectedTool property. This dependency property 
        /// indicates ....
        /// </summary>
        public object SelectedTool
        {
            get { return GetValue(SelectedToolProperty); }
            set { SetValue(SelectedToolProperty, value); }
        }

        #endregion

    }
}
