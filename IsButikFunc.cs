using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isbutik
{
    public class IsButikFunc
    {
        public IsButikData data = new IsButikData(); // Field. Benyttes for at kalde metoder i klassen IsButikData herfra.
        public void TestDatabase(List<Bestilling> bestillingsListe, List<Vare> vareListe)
        {
            data.TestDatabase(bestillingsListe, vareListe);
        }
        
        public static void TestTest()  // Eksempel for en static method. Kaldes med klassenavnen i stedet for objektnavn.
        {
            ;
        }
        public void ReadVarer(List<Vare> vareListe)  // Method. Henter alle varer fra databasen.
        {
            data.ReadVarer(vareListe);
        }
        public void GemVare(Vare vare)
        {
            if (vare.Id > 0)
                data.UpdateVare(vare);
            else
                data.CreateVare(vare);
        }
        public void ReadBestillinger(List<Bestilling> bestillingsListe, List<Vare> vareListe)
        {
            data.ReadBestillinger(bestillingsListe, vareListe);
        }
        public void CreateBestilling(Bestilling bestilling)
        {
            data.CreateBestilling(bestilling);
        }
        public void DeleteBestilling(Bestilling bestilling)
        {
            data.DeleteBestilling(bestilling);
        }
        public int CheckPositiveInt(string strAntal)
        {
            int antal;
            bool isParsable = Int32.TryParse(strAntal, out antal);
            if (isParsable & antal > 0)
            {
                return antal;
            }
            else
            {
                return -1;
            }
        }
        public bool CheckIcecremeSelected(System.Windows.Controls.ComboBox ComboIcecremes)
        {
            try
            {
                string name = (ComboIcecremes.SelectedItem as Vare).Name;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
