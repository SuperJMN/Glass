using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPFBasics.Test {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        #region MyFlag

        /// <summary>
        /// MyFlag Dependency Property
        /// </summary>
        public static readonly DependencyProperty MyFlagProperty =
            DependencyProperty.Register("MyFlag", typeof(MyFlag), typeof(MainWindow),
                new FrameworkPropertyMetadata(MyFlag.None, OnMyFlagChanged));

        /// <summary>
        /// Gets or sets the MyFlag property. This dependency property 
        /// indicates ....
        /// </summary>
        public MyFlag MyFlag {
            get { return (MyFlag)GetValue(MyFlagProperty); }
            set { SetValue(MyFlagProperty, value); }
        }

        /// <summary>
        /// Handles changes to the MyFlag property.
        /// </summary>
        private static void OnMyFlagChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var target = (MainWindow)d;
            var oldMyFlag = (MyFlag)e.OldValue;
            var newMyFlag = target.MyFlag;
            target.OnMyFlagChanged(oldMyFlag, newMyFlag);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the MyFlag property.
        /// </summary>
        protected virtual void OnMyFlagChanged(MyFlag oldMyFlag, MyFlag newMyFlag) {
        }

        #endregion


    }

    [Flags]
    public enum MyFlag {
        None,
        Left,
        Right,
        Top,
        Bottom,
        All = Right | Left | Top | Bottom,
    }
}
