using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Isbutik.IsButikDatasetTableAdapters;
using static Isbutik.IsButikDataset;
using System.Data;

namespace Isbutik
{
    public class IsButikData
    {
        IsButikDataset isButikDataset = new IsButikDataset();  // Field. Benyttes for at kalde metoder i klassen IsButikDataset herfra.
        TableAdapterManager AdapterManager = new TableAdapterManager();  // Field. Benyttes for at kalde metoder i klassen TableAdapterManager herfra.
        SqlAccess sqlAccess = new SqlAccess(); // Field. Benyttes for at kalde metoder i klassen SqlAccess herfra.

        public IsButikData()  // Constructor. Benyttes til at oprette instanser(objekter) af vores adapters.
        {
            AdapterManager.BestillingTableAdapter = new BestillingTableAdapter();
            AdapterManager.VareTableAdapter = new VareTableAdapter();
        }

        public void ReadVarer(List<Vare> vareListe)  // Method. Henter alle varer fra databasen.
        {
            vareListe.Clear();  // Tøm listen før vi fylder den. På den måde forhindrer vi dubletter.
            VareDataTable vareRows = new VareDataTable();
            AdapterManager.VareTableAdapter.Fill(vareRows);

            foreach (VareRow row in vareRows)
            {
                if(row.Stykpris >= 0)  // varer med negativ pris betragter vi som logisk slettet
                {
                    Vare vare = new Vare()
                    {
                        Id = row.Id,
                        Name = row.Varenavn,
                        Price = (float)row.Stykpris,
                        Description = row.Beskrivelse,
                    };
                    vareListe.Add(vare);
                }
            }
        }

        public Vare GetVare(List<Vare> vareListe, int vareId)
        // Find varen med en bestemt vareId. Her lavet med en løkke, andre mere avancerede metoder kan anvendes.
        // Man kunne fx bruge Find-metoden på listen anvende LINQ.
        {
            foreach (Vare vare in vareListe)
            {
                if (vare.Id == vareId)
                {
                    return vare;
                }
            }
            return null;  // ingen vare i vareListe har den Id vi ledte efter
        }
        
        public void ReadBestillinger(List<Bestilling> bestillingsListe, List<Vare> vareListe)  // Method. Henter alle bestillinger fra databasen.
        {
            bestillingsListe.Clear();  // Tøm listen før vi fylder den. På den måde forhindrer vi dubletter.
            BestillingDataTable bestillingRows = new BestillingDataTable();
            AdapterManager.BestillingTableAdapter.Fill(bestillingRows);

            foreach (BestillingRow row in bestillingRows)
            {
                Bestilling bestilling = new Bestilling(row.Id, row.Antal, GetVare(vareListe, row.VareId));
                bestillingsListe.Add(bestilling);
            }
        }
        
        public void CreateVare(Vare vare)
        {
            VareRow row = isButikDataset.Vare.NewVareRow();
            row.Beskrivelse = vare.Description;
            row.Varenavn = vare.Name;
            row.Stykpris = (decimal)vare.Price;
            RækkeTilDatabase(row);
        }

        private void RækkeTilDatabase(VareRow row)
        {
            isButikDataset.Vare.Rows.Add(row);
            AdapterManager.VareTableAdapter.Update(isButikDataset.Vare);
        }

        public void CreateBestilling(Bestilling bestilling)
        {
            BestillingRow row = isButikDataset.Bestilling.NewBestillingRow();
            row.Antal = bestilling.Quantity;
            row.VareId = bestilling.Vare.Id;
            RækkeTilDatabase(row);
        }

        private void RækkeTilDatabase(BestillingRow row)  // Overloading :)
        {
            isButikDataset.Bestilling.Rows.Add(row);
            AdapterManager.BestillingTableAdapter.Update(isButikDataset.Bestilling);
        }

        public void UpdateVare(Vare vare)
        {
            VareDataTable vareRows = new VareDataTable();
            AdapterManager.VareTableAdapter.Fill(vareRows);  // Hent database rows
            VareRow row = vareRows.FindById(vare.Id); // Find row
            row.Beskrivelse = vare.Description;  // Opdater row
            row.Varenavn = vare.Name;
            row.Stykpris = (decimal)vare.Price;
            AdapterManager.VareTableAdapter.Update(vareRows); // Gem forandringer
        }

        public void UpdateBestilling(Bestilling bestilling)
        {
            BestillingDataTable bestillingRows = new BestillingDataTable();
            AdapterManager.BestillingTableAdapter.Fill(bestillingRows);  // Hent database rows
            BestillingRow row = bestillingRows.FindById(bestilling.Id); // Find row
            row.Antal = bestilling.Quantity;  // Opdater row
            row.VareId = bestilling.Vare.Id;
            AdapterManager.BestillingTableAdapter.Update(bestillingRows); // Gem forandringer
        }

        public void DeleteBestilling(Bestilling bestilling)
        {
            AdapterManager.BestillingTableAdapter.Delete(bestilling.Id, bestilling.Quantity, bestilling.Vare.Id);
        }

        public void DeleteVareLogisk(Vare vare)
        {
            vare.Price = -99.0f;
            UpdateVare(vare);
        }
        public List<Vare> Table2VareListe(DataTable table)
        {
            List<Vare> liste = new List<Vare>();
            foreach (DataRow row in table.Rows)
            {
                Vare vare = Row2Vare(row);
                liste.Add(vare);
            }
            return liste;
        }
        private Vare Row2Vare(DataRow row)
        {
            Vare vare = new Vare();
            vare.Id = (int)row["Id"];
            vare.Name = (string)row["Varenavn"];
            vare.Price = Convert.ToSingle(row["Stykpris"]);  // decimal -> float
            vare.Description = (string)row["Beskrivelse"];
            return vare;
        }
        public List<Bestilling> Table2BestillingsListe(DataTable table, List<Vare> vareListe)
        {
            List<Bestilling> liste = new List<Bestilling>();
            foreach (DataRow row in table.Rows)
            {
                Bestilling bestilling = Row2Bestilling(row, vareListe);
                liste.Add(bestilling);
            }
            return liste;
        }
        private Bestilling Row2Bestilling(DataRow row, List<Vare> vareListe)
        {
            Bestilling bestilling = new Bestilling((int)row["Id"], (int)row["Antal"], GetVare(vareListe, (int)row["VareId"]));
            return bestilling;
        }

        public void TestDatabase(List<Bestilling> bestillingsListe, List<Vare> vareListe)
        {
            sqlAccess.Connect("Data Source=(localdb)\\MSSQLLocalDB;Database=IsbutikDB");
            string query = "SELECT* FROM Vare WHERE Stykpris > 0;";
            List<Vare> vareListeTmp = Table2VareListe(sqlAccess.ExecuteSql(query));
            foreach (var vare in vareListeTmp)
            {
                vareListe.Add(vare);
            }
            query = "SELECT* FROM Bestilling WHERE Antal > 5;";
            List<Bestilling> bestillingListeTmp = Table2BestillingsListe(sqlAccess.ExecuteSql(query), vareListe);
            foreach (var bestilling in bestillingListeTmp)
            {
                bestillingsListe.Add(bestilling);
            }

            //ReadVarer(vareListe);
            //ReadBestillinger(bestillingsListe, vareListe);
            //bestillingsListe.Add(new Bestilling(vareListe[0], 17));
            //CreateBestilling(bestillingsListe[bestillingsListe.Count-1]);

            //vareListe[0].Price = 19.98f;
            //UpdateVare(vareListe[0]);

            //ReadVarer(vareListe);
            //DeleteVareLogisk(vareListe[0]);
            //ReadVarer(vareListe);

            //ReadBestillinger(bestillingsListe, vareListe);
            //CreateBestilling(bestillingsListe[0]);

            //ReadBestillinger(bestillingsListe, vareListe);
            //bestillingsListe[2].Quantity = 1;
            //UpdateBestilling(bestillingsListe[2]);

            //DeleteBestilling(bestillingsListe[7]);
        }
    }
}
