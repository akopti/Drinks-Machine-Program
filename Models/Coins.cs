
namespace Drinks_Machine_Program.Models
{
    class Coins
    {
        //  These values represents the quantity of the coins not the value

        public Coins(int penny, int dime, int nickel, int quarter)
        {
            Penny = penny;
            Nickel = nickel;
            Dime = dime;
            Quarter = quarter;
        }

        public int Penny { get; set; }
        public int Nickel { get; set; }
        public int Dime { get; set; }
        public int Quarter { get; set; }
    }
}
