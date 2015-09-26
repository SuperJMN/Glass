using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Cinch;
using Glass.Design;
using Glass.Design.Designer;
using MEFedMVVM.ViewModelLocator;

namespace Test {
    [ExportViewModel("Main")]
    public class MainViewModel : ViewModelBase {
        private IList<IDesignable> designables;



        [ImportingConstructor]
        public MainViewModel() {
            Designables = new List<IDesignable>();
            var designableAdapter1 = new DesignerItem { Content = new TextBox { Text = "Hola tío" } };
            designableAdapter1.Left = 100;
            designableAdapter1.Top = 13;
            designableAdapter1.Width = 80;
            designableAdapter1.Height = 25;

            //var designableAdapter2 = new DesignerItem { Content = new TextBox { Text = "Esto es mi texto" } };
            //designableAdapter2.Left = 150;
            //designableAdapter2.Top = 50;
            //designableAdapter2.Width = 100;
            //designableAdapter2.Height = 45;

            //var designableAdapter3 = new DesignerItem { Content = new Image { Source = new BitmapImage(new Uri("Mario.png", UriKind.Relative)) } };


            Designables.Add(designableAdapter1);
            //Designables.Add(designableAdapter2);
            //Designables.Add(designableAdapter3);

            MenuItems = new List<CinchMenuItem>();
            var cinchMenuItem = new CinchMenuItem("Archivo");
            cinchMenuItem.Children.Add(new CinchMenuItem("Salir") { IconUrl = "Mario.png" });
            MenuItems.Add(cinchMenuItem);
        }

        public IList<IDesignable> Designables {
            get { return designables; }
            set {
                designables = value;
                NotifyPropertyChanged("Designables");
            }
        }


        #region Property MenuItems
        private IList<CinchMenuItem> menuItems;
        public static readonly PropertyChangedEventArgs MenuEventArgs = ObservableHelper.CreateArgs<MainViewModel>(x => x.MenuItems);
        public IList<CinchMenuItem> MenuItems {
            get { return menuItems; }
            set {

                if (object.ReferenceEquals(menuItems, value)) return;
                menuItems = value;
                NotifyPropertyChanged(MenuEventArgs);
            }
        }
        #endregion

    }
}