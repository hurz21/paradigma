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

namespace HauptMenu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool ladevorgangabgeschlossen { get; set; } = false;
        public bool adminModus { get; set; } = false;
        public Point curContentMousePoint { get; set; }
        // Property gisappsDir As String = INI_Databases.getXmlTagValue("GisServer.gisappsDir") '
        private string az;
        private string header;
        public static Kookieliste kookieFenster;

        public string alter_titel;
        public string alter_probaugAz;
        public string altergemKRZ;
        private DataGridCell LastCell = new DataGridCell();
        private string logfile;
        public string startModus;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            System.IO.Directory.SetCurrentDirectory(@"C:\kreisoffenbach\main");
            Left = 1; Top = 100;
            Startroutine("");
            //this.Top = csCLstart.formposition.setPosition("diverse", "winHauptformpositiontop", this.Top);
            //this.Left = csCLstart.formposition.setPosition("diverse", "winHauptformpositionleft", this.Left);
        }

        private void Startroutine(string v)
        {
            throw new NotImplementedException();
        }

        private void btnEigentuemer_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAdmin_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnBestand_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnNeuerVorgang_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnProjekte_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnrefresh_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnWiedervorlage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnZahlungen_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Handbuch_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NeuGis_Click(object sender, RoutedEventArgs e)
        {

        }

        private void stake(object sender, RoutedEventArgs e)
        {

        }

        private void btnZurNr_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void MenuItem_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void tblastvorgangsid_Click(object sender, RoutedEventArgs e)
        {

        }

        private void tbzuVorgang_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void TbzuVorgang_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void zeigeVersion(object sender, RoutedEventArgs e)
        {

        }

        private void showCopyrite_Click(object sender, RoutedEventArgs e)
        {

        }

        private void showLogFile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Konfigurieren_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnZurNr_MouseEnter(object sender, MouseEventArgs e)
        {

        }

  
    }
}
