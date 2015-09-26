using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Cinch;

namespace Test
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                CinchBootStrapper.Initialise(new List<Assembly>
                                                   {
                                                       Assembly.GetAssembly(typeof (MainWindow))
                                                   });
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            base.OnStartup(e);
        }
    }
}
