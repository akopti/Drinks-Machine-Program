using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Drinks_Machine_Program.Models;

namespace Drinks_Machine_Program
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        // Machine object that contains the drinks and coins in stock
        private Machine machine;

        private int _cokeQuantity;

        public int CokeQuantity
        {
            get { return _cokeQuantity; }
            set
            {
                if (value != _cokeQuantity)
                {
                    _cokeQuantity = (int)value;
                    OnPropertChanged();
                }
            }
        }

        private int _pepsiQuantity;

        public int PepsiQuantity
        {
            get { return _pepsiQuantity; }
            set
            {
                if (value != _pepsiQuantity)
                {
                    _pepsiQuantity = (int)value;
                    OnPropertChanged();
                }
            }
        }

        private int _sodaQuantity;

        public int SodaQuantity
        {
            get { return _sodaQuantity; }
            set
            {
                if (value != _sodaQuantity)
                {
                    _sodaQuantity = (int)value;
                    OnPropertChanged();
                }
            }
        }

        private double _totalValue;

        public double TotalValue
        {
            get { return _totalValue; }
            set
            {
                if (value != _totalValue)
                {
                    _totalValue = (double)value;
                    OnPropertChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // If value changed on drinks quantity update total order
        public void OnPropertChanged([CallerMemberName] string caller = "")
        {
            if (PropertyChanged != null)
            {
                if (CokeQuantity >= 0 && PepsiQuantity >= 0 && SodaQuantity >= 0)
                {
                    if (CokeQuantity == 0 && PepsiQuantity == 0 && SodaQuantity == 0)
                    {
                        GetDrinksButton.IsEnabled = false;
                    }
                    else
                    {
                        GetDrinksButton.IsEnabled = true;
                    }
                    TotalValue = CentToDollars((CokeQuantity * 25) + (PepsiQuantity * 36)
                        + (SodaQuantity * 45), 0, 0, 0);
                }
                PropertyChanged(this, new PropertyChangedEventArgs(caller));
            }
        }

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();

            //  Initial values
            Coins coins = new Coins(100, 10, 5, 25);
            IDictionary<string, Drinks> drinks = new Dictionary<string, Drinks>() {
                {"Coke", new Drinks("Coke", 25, 5) },
                {"Pepsi", new Drinks("Pepsi", 36, 15) },
                {"Soda", new Drinks("Soda", 45, 3) },
            };
            // Add to machine obj
            machine = new Machine(drinks, coins);
            // Update the quantity of the drinks
            UpdateQuantities();

            GetDrinksButton.IsEnabled = false;
        }

        public void UpdateQuantities()
        {
            // Setup the quantity and cost of the drinks
            cokeQuantity.Text = machine.Drinks["Coke"].Quantity == 0 ? "Sold out"
                : machine.Drinks["Coke"].Quantity.ToString()
                + " drinks available, Cost = " + machine.Drinks["Coke"].Cost.ToString();

            pepsiQuantity.Text = machine.Drinks["Pepsi"].Quantity == 0 ? "Sold out"
                : machine.Drinks["Pepsi"].Quantity.ToString()
                + " drinks available, Cost = " + machine.Drinks["Pepsi"].Cost.ToString();

            sodaQuantity.Text = machine.Drinks["Soda"].Quantity == 0 ? "Sold out" :
                machine.Drinks["Soda"].Quantity.ToString()
                + " drinks available, Cost = " + machine.Drinks["Soda"].Cost.ToString();

            //  Disable text box if sold out
            DisableTextboxSoldOut();

            //  Clear all text boxes
            ClearTextboxes();

            //  Disable button
            GetDrinksButton.IsEnabled = false;

        }

        private void ClearTextboxes()
        {
            CokeQuantity = 0;
            CokeRequested.Clear();

            PepsiQuantity = 0;
            PepsiRequested.Clear();

            SodaQuantity = 0;
            SodaRequested.Clear();

            cents_txtbox.Clear();
            dime_txtbox.Clear();
            nickel_txtbox.Clear();
            quarter_txtbox.Clear();
        }

        private void DisableTextboxSoldOut()
        {
            if (machine.Drinks["Coke"].Quantity == 0)
            {
                CokeRequested.IsEnabled = false;
            }
            if (machine.Drinks["Pepsi"].Quantity == 0)
            {
                PepsiRequested.IsEnabled = false;
            }
            if (machine.Drinks["Soda"].Quantity == 0)
            {
                SodaRequested.IsEnabled = false;
            }
        }

        // "Get Drinks" button on click listener
        private void ButtonGetDrinks_Click(object sender, RoutedEventArgs e)
        {
            double uCents = 0; double uDime = 0; double uNickels = 0; double uQuarters = 0;

            if (cents_txtbox.Text != "")
            {
                uCents = Int32.Parse(cents_txtbox.Text);
            }
            if (dime_txtbox.Text != "")
            {
                uDime = Int32.Parse(dime_txtbox.Text);
            }
            if (nickel_txtbox.Text != "")
            {
                uNickels = Int32.Parse(nickel_txtbox.Text);
            }
            if (quarter_txtbox.Text != "")
            {
                uQuarters = Int32.Parse(quarter_txtbox.Text);
            }
            if (uCents < 0 || uDime < 0 || uNickels < 0 || uQuarters < 0)
            {
                MessageBox.Show("Cannot accept negative numbers", "Error");
                UpdateQuantities();
                return;
            }

            double total = CentToDollars(uCents, uNickels, uDime, uQuarters);

            List<string> errors = ValidateTransaction(total);
            if (errors.Count > 0)
            {
                string message = "";

                foreach (string error in errors)
                {
                    message += "• " + error + "\n";
                }

                MessageBox.Show(message, "Error");
            }
            else
            {
                double change = total - TotalValue;
                string message = "Your total was: " + TotalValue + " You have bought: \n";
                if (CokeQuantity > 0)
                {
                    message += "• " + CokeQuantity + " bottles of Cokes \n";
                }
                if (PepsiQuantity > 0)
                {
                    message += "• " + PepsiQuantity + " bottles of Pepsi \n";
                }
                if (SodaQuantity > 0)
                {
                    message += "• " + SodaQuantity + " bottles of Soda \n";
                }
                message += "You have paid: " + total + " dollars ";
                if (change != 0)
                {
                    message += "and your change is: " + change.ToString("#.##") + " dollars";
                }
                TransactionSuccess(uCents, uNickels, uDime, uQuarters);
                MessageBox.Show(message);
            }
        }
        
        // Convert cents to dollar format
        public double CentToDollars(double cents, double nickels, double dimes, double quarters)
        {
            double total = (cents + (nickels * 5) + (dimes * 10) + (quarters * 25)) / 100;
            return total;
        }

        // Validate input
        private List<string> ValidateTransaction(double userFunds)
        {

            List<string> errors = new List<string>();
            double machineBudget = CentToDollars(machine.Coins.Penny, machine.Coins.Nickel,
                machine.Coins.Dime, machine.Coins.Quarter);

            if (userFunds < TotalValue)
            {
                errors.Add("Total is greater than " + userFunds + " you need "
                    + (TotalValue - userFunds) + " dollars more");
            }
            if (machine.Drinks["Coke"].Quantity - CokeQuantity < 0)
            {
                errors.Add("We are sorry but it seems that we don't have enough Coke");
            }
            if (machine.Drinks["Pepsi"].Quantity - PepsiQuantity < 0)
            {
                errors.Add("We are sorry but it seems that we don't have enough Pepsi");
            }
            if (machine.Drinks["Soda"].Quantity - SodaQuantity < 0)
            {
                errors.Add("We are sorry but it seems that we don't have enough Soda");
            }
            if (machineBudget < (userFunds - TotalValue))
            {
                errors.Add("We Appologize but it seems that our machine does not have enough change");
            }

            return errors;
        }

        // If the transaction is a success update quantities of the coins and drinks
        public void TransactionSuccess(double uCent, double uNickel, double uDime, double uQuarter)
        {
            machine.Coins.Penny = machine.Coins.Penny - Convert.ToInt32(uCent);
            machine.Coins.Penny = machine.Coins.Nickel - Convert.ToInt32(uNickel);
            machine.Coins.Penny = machine.Coins.Dime - Convert.ToInt32(uDime);
            machine.Coins.Penny = machine.Coins.Quarter - Convert.ToInt32(uQuarter);

            machine.Drinks["Coke"].Quantity -= CokeQuantity;
            machine.Drinks["Pepsi"].Quantity -= PepsiQuantity;
            machine.Drinks["Soda"].Quantity -= SodaQuantity;

            UpdateQuantities();
        }
    }
}
