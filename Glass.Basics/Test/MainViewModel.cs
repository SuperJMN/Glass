using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using Cinch;
using MEFedMVVM.ViewModelLocator;

namespace WPFBasics.Test
{
    [ExportViewModel("Main")]
    public class MainViewModel : ViewModelBase
    {
        private readonly IMessageBoxService service;
        private Point dropPoint;
        private UIElement dropTarget;


        [ImportingConstructor]
        public MainViewModel(IMessageBoxService service)
        {
            this.service = service;
            MiListilla = new List<string>
                             {
                                 "Hello",
                                 "I enjoy a lot",
                                 "doing Drag",
                                 "and",
                                 "Drop!",
                                 "Oh yeah!",
                             };

            ShowMessageCommand = new SimpleCommand<object, object>(o => service.ShowInformation(string.Format("You've dropped a {0} into a {1}",  o.ToString(), DropTarget)));
        }

        public IList<string> MiListilla { get; set; }

        public ICommand ShowMessageCommand { get; set; }



        #region Property DropPoint

        public Point DropPoint {
            get { return dropPoint; }
            set {
                dropPoint = value;
                NotifyPropertyChanged("DropPoint");
            }
        }

        #endregion

        #region Property DropTarget

        public UIElement DropTarget {
            get { return dropTarget; }
            set {
                dropTarget = value;
                NotifyPropertyChanged("DropTarget");
            }
        }

        #endregion
    }
}