using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WindowsHostsFileManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HostEntryManager manager = new HostEntryManager();

        public MainWindow()
        {
            InitializeComponent();
        }
        private void WindowMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            btnRestart.IsEnabled = !Utilities.IsAdministrator();
            btnSave.IsEnabled = Utilities.IsAdministrator();
            manager.LoadHostsFile();
            lstEntries.DataContext = manager.HostEntries;
        }



        private void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            if (Utilities.IsAdministrator() == false)
            {
                // Restart program and run as admin
                var exeName = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                var startInfo = new ProcessStartInfo(exeName);
                startInfo.Verb = "runas";
                System.Diagnostics.Process.Start(startInfo);
                Application.Current.Shutdown();
                return;
            }

        }

        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            manager.LoadHostsFile();
            lstEntries.DataContext = manager.HostEntries;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            manager.Save();
            manager.LoadHostsFile();
            lstEntries.DataContext = manager.HostEntries;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Maximized) { this.WindowState = System.Windows.WindowState.Normal; }
        }

        private void BtnRemoveComments_OnClick(object sender, RoutedEventArgs e)
        {
            manager.RemoveComments();
            lstEntries.DataContext = manager.HostEntries;
        }

        private void ToggleState_EventHandler(object sender, RoutedEventArgs e)
        {
            manager.ToggleState(((Button)sender).DataContext as HostEntry);
            lstEntries.Items.Refresh();
        }

        private void BtnClose_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnMinimize_OnClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }
    }
}
