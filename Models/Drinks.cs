
namespace Drinks_Machine_Program.Models
{
    class Drinks
    {
        public Drinks(string type, int cost, int quantity)
        {
            Type = type;
            Cost = cost;
            Quantity = quantity;
        }

        public string Type { get; set; }
        public int Cost { get; set; }
        public int Quantity { get; set; }
    }
}
