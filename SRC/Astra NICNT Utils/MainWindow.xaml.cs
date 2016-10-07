using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Astra_NICNT_Utils.Annotations;
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

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            AddLog("Mabuta1 asdf asdf as.ld,fj a;sdf asdhf laksdjfh aslkdjfh laksdjfh aslkdf hlaskdfh laksdhfj alskdfh laskdfh jlaskdhf laskdf hlaskdjfh alskdfh alskdfh alskdhf j");
            AddLog("Not yet implemened");
            AddLog("Not yet implemened");
            AddLog("Not yet implemened");
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
