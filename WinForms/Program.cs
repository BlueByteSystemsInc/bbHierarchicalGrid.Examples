using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WinFormsNS  = System.Windows.Forms;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows.Controls;

namespace WinForms
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            // create a new WPF application 
            var wpfApp = new System.Windows.Application();
            var theme = new ResourceDictionary();

            /*
             * Themes are:
             * Default
             * VisualStudio.Blue
             * VisualStudio.Light
             */
            theme.Source = new Uri("pack://application:,,,/bbHierarchicalGrid;Component/Styles/Dark.xaml");
            wpfApp.Resources.MergedDictionaries.Add(theme);

          
            WinFormsNS.Application.EnableVisualStyles();
            WinFormsNS.Application.SetCompatibleTextRenderingDefault(false);
            WinFormsNS.Application.Run(new WinForms());
        }
    }
}
