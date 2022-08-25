using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Isbutik
{
    public class Bestilling
    {
        private int quantity;  // Privat field. Ikke synlig udenfor klassen.
        private float sum;     
        public int Id { get; set; }  // Offentlig property. Synlig udenfor klassen.
        public Vare Vare { get; set; }
        public int Quantity
        {
            get { return quantity; }
            set
            {
                if (quantity != value)  // value er et nøgleord
                {
                    quantity = value;
                    sum = Quantity * Vare.Price;
                }
            }
        }
        public float Sum
        {
            get { return sum; }
            set
            {
                if (sum != value)
                {
                    sum = value;
                }
            }
        }
        public Bestilling(Vare vare, int quantity)  // Konstruktor før bestillingen gemmes i databasen.
        {
            Id = 0;
            Vare = vare;
            Quantity = quantity;
            Sum = Quantity * vare.Price;
        }
        public Bestilling(int id, int quantity, Vare vare)  // Konstruktor når bestillingen hentes fra databasen.
        {
            Id = id;
            Vare = vare;
            Quantity = quantity;
            Sum = Quantity * vare.Price;
        }
    }
}