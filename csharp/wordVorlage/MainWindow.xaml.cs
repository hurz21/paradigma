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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace wordVorlage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int vid = 9609;
        private int eid = 789789;
        private int sid = 3307;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            vid = nsStartup.startUpHelper.getArgByMarker("vid=");
            eid = nsStartup.startUpHelper.getArgByMarker("eid=");
            sid = nsStartup.startUpHelper.getArgByMarker("sid=");
        } 
    }
}
