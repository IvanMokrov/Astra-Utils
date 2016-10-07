using System;
using System.Windows;
using Astra_NICNT_Utils.ViewModel;

namespace Astra_NICNT_Utils
{
    public partial class MainWindow : Window
    {

        /// <summary> Main ViewModel </summary>
        public MainWindowVM MVM { get; }


        public MainWindow()
        {
            MVM = new MainWindowVM();
            MVM.OnAddLog += (sender, s) => AddLog(s);
            DataContext = MVM;

            InitializeComponent();
        }

        public void AddLog(string text)
        {
            txtLog.AppendText(text + "\r\n");
            txtLog.ScrollToEnd();
        }

        // Too simple for ICommand, no interaction
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Too simple for ICommand, no interaction
        private void btnNI_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://www.native-instruments.com");
        }

        // Too simple for ICommand, no interaction
        private void btnXML_OnClick(object sender, RoutedEventArgs e)
        {
            View.XmlChecker WND = new View.XmlChecker();
            WND.DataContext = DataContext;
            WND.Show();
        }

        // Too simple for ICommand, no interaction
        private void btnREG_OnClick(object sender, RoutedEventArgs e)
        {
            View.RegistryChecker WND = new View.RegistryChecker();
            WND.DataContext = DataContext;
            WND.Show();
        }

        // Too simple for ICommand, no interaction
        private void Window_Closed(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }

    }
}
