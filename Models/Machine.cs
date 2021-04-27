using System.Collections.Generic;

namespace Drinks_Machine_Program.Models
{
    class Machine
    {
        public Machine()
        {

        }

        public Machine(IDictionary<string, Drinks> drinks, Coins coins)
        {
            Drinks = drinks;
            Coins = coins;
        }

        public IDictionary<string, Drinks> Drinks { get; set; }

        public Coins Coins { get; set; }
    }
}
