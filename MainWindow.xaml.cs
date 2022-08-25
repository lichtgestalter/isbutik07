using System;
using System.Diagnostics;
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

namespace Isbutik
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public IsButikFunc func = new IsButikFunc();  // Field. Benyttes for at kalde metoder i klassen IsButikFunc herfra.
        public List<Vare> vareListe = new List<Vare>();
        public List<Bestilling> bestillingsListe = new List<Bestilling>();
        
        public MainWindow()  // Constructor
        {
            //Debug.WriteLine("Hello World"); git1
            DataContext = this;  // MainWindow bliver udgangspunkt for alle sammenbindinger mellem Gui-komponenter. Her unødvendig?
            func.TestDatabase(bestillingsListe, vareListe);
            //func.ReadVarer(vareListe);  // Hent varer fra databasen.
            InitializeComponent();
            ComboIcecremes.ItemsSource = vareListe;  // koble Combobox med vareListe
            dgBestillinger.ItemsSource = bestillingsListe;  // koble DataGrid med bestillingsliste
            tbVareId.Text = "0";
        }
        private void ComboIcecremes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Vare vare = ComboIcecremes.SelectedItem as Vare;
            if(vare!=null)
                IceCremeDescription.Text = vare.Description;
        }

        private void AddOrderLine_Click(object sender, RoutedEventArgs e)  // Tilføj
        {
            bool checkIcecreme = func.CheckIcecremeSelected(ComboIcecremes);
            if (!checkIcecreme)
                MessageBox.Show("Vælg en icecreme!");
            else
            {
                string felt = "Vælg antal";
                int antal = func.CheckPositiveInt(tbAntal.Text);
                if (antal <= 0)
                    MessageBox.Show($"Felt {felt} skal indeholde et positiv helt tal.");
                if (antal > 0 & checkIcecreme)
                {
                    Vare vare = (ComboIcecremes.SelectedItem as Vare);
                    bestillingsListe.Add(new Bestilling(vare, antal));
                    func.CreateBestilling(bestillingsListe[bestillingsListe.Count - 1]);
                    func.ReadBestillinger(bestillingsListe, vareListe);
                }
                tbAntal.Text = "";
            }
            dgBestillinger.Items.Refresh();
        }
        private void RemoveOrderLine_Click(object sender, RoutedEventArgs e) // Fjern
        {
            Bestilling bestilling = (dgBestillinger.SelectedItem as Bestilling);
            func.DeleteBestilling(bestilling);
            func.ReadBestillinger(bestillingsListe, vareListe);
            dgBestillinger.Items.Refresh();
        }

        private void BtBestil_Click(object sender, RoutedEventArgs e) // Bestil
        {
            foreach (var bestilling in bestillingsListe)
            {
                func.DeleteBestilling(bestilling);
            }
            bestillingsListe.Clear();
            dgBestillinger.Items.Refresh();
        }
        private void GemVare_Click(object sender, RoutedEventArgs e) // Gem Vare
        {
            Vare newIcecreme = new Vare { Id = Int32.Parse(tbVareId.Text), Name = tbVareName.Text, Description = tbVareDescription.Text, Price = float.Parse(tbVarePrice.Text) };
            func.GemVare(newIcecreme);
            func.ReadVarer(vareListe);
            tbVareName.Text = "";  // tøm TextBox
            tbVareId.Text = "0";
            tbVareDescription.Text = "";
            tbVarePrice.Text = "";
            ComboIcecremes.Items.Refresh();
        }

         private void RedigerVare_Click(object sender, RoutedEventArgs e) // Rediger Vare
        {
            Vare vare = ComboIcecremes.SelectedItem as Vare;
            tbVareId.Text = vare.Id.ToString();
            tbVareName.Text = vare.Name;
            tbVarePrice.Text = vare.Price.ToString();
            tbVareDescription.Text = vare.Description;
        }
    }
}