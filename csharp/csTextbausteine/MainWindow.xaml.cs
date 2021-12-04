using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.VisualBasic;

namespace csTextbausteine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public string TB_RootPath { get; set; } = @"O:\UMWELT\B\GISDatenEkom\Textbausteine";
        public string username { get; set; } = Environment.GetEnvironmentVariable("username");
        public TB_Auswahl aktTB { get; set; }
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
             aktTB = new TB_Auswahl(TB_RootPath);
            initGruppenCombo();
        }
        void initGruppenCombo()
        {
            var startDir = new DirectoryInfo(TB_RootPath);
            foreach (var dir in startDir.GetDirectories())
            {
                Console.WriteLine(dir.Name);
                cmbGruppe.Items.Add(dir.Name);
            }

            cmbGruppe.IsDropDownOpen = true; // SelectedValue = "Kostenfestsetzung"
        }
        private void initAdmin()
        {
            if (isAdmin())
            {
                stckAdmin.Visibility = Visibility.Visible;
            }
        }
        public bool isAdmin()
        {
            if ((username.ToLower().Equals("weyers_g") |
                username.ToLower().Equals("nehler_u") |
                username.ToLower().Equals("feinen_js") |
                username.ToLower().Equals("kuhn_p")))
            {
                return true;
            }
            return false;
        }

        private void edit_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (string.IsNullOrEmpty(aktTB.datei))
            {
                MessageBox.Show("Es wurde noch keine Datei ausgewählt");
                return;
            }
            System.Diagnostics.Process.Start(aktTB.datei);
        }

        private void cmbGruppe_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            {
                if (cmbGruppe.SelectedItem is null)
                    return;
                string tttt;
                tttt = Convert.ToString(cmbGruppe.SelectedValue.ToString());
                aktTB.Gruppe = tttt;//.Replace("System.Windows.Controls.ComboBoxItem: ", "");
                initsubdir();
                e.Handled = true;
            } 
        } 
        private void initsubdir()
        {
            {
                var startDir = new DirectoryInfo(System.IO.Path.Combine(TB_RootPath, aktTB.Gruppe)); 
                cmbSubdir.Items.Clear();
                foreach (var datei in startDir.GetFiles())
                {
                    Console.WriteLine(datei.Name);
                    cmbSubdir.Items.Add(datei.Name.Replace(".rtf", "").Replace(".RTF", "").Replace(".Rtf", ""));
                }

                tbInfo.Text = "Der Textbaustein befindet sich nun in der Zwischenablage. Mit Strg-v fügen Sie Ihn in das Word-Dokument ein!";
                cmbSubdir.IsDropDownOpen = true;
            } 
        } 
        private void cmbSubdir_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            {
                e.Handled = true;
                if (cmbSubdir.SelectedItem is null)
                    return;
                string tttt;
                tttt = Convert.ToString(cmbSubdir.SelectedValue.ToString());
                aktTB.subdir = tttt.Replace("System.Windows.Controls.ComboBoxItem: ", "").Replace(".rtf", "").Replace(".RTF", "").Replace(".Rtf", "");
                initTb();
            } 
        } 
        private void initTb()
        {
            rtf2Box();
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        } 
        private void rtf2Box()
        {
            Clipboard.Clear();
            aktTB.datei = aktTB.buildFullpath();
            var sr = new StreamReader(aktTB.datei);
            aktTB.inhalt = sr.ReadToEnd();
            sr.Close();
            sr.Dispose();
            var stream = new MemoryStream(Encoding.Default.GetBytes(aktTB.inhalt));
            rtfbox.Selection.Load(stream, DataFormats.Rtf);
            Clipboard.SetText(aktTB.inhalt, TextDataFormat.Rtf);
        }
        private void btnNeueGruppe_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            string input = Microsoft.VisualBasic.Interaction.InputBox("Bitte den Namen der Gruppe eingeben (KEINE Kommas, Punkte o.ö. nur Buchstaben)", "Neue Kategorie", "", -1, -1);
            if (input == string.Empty) {
                MessageBox.Show("Keine Eingabe");
                return;
            }
            System.IO.Directory.CreateDirectory(System.IO.Path.Combine(TB_RootPath,input));
            System.Diagnostics.Process.Start(System.IO.Path.Combine(TB_RootPath, input));
        }

        private void btnDiropen_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            System.Diagnostics.Process.Start(TB_RootPath);
        }
    }
}
