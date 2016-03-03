using System;
using System.Windows;
using Cinch;

namespace Test {
    
    [PopupNameToViewLookupKeyMetadata("Main", typeof(MainWindow))]
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
           
        }
    }
}
