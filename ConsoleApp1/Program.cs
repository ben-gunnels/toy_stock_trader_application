// See https://aka.ms/new-console-template for more information
using StockMarket;
using StockPortfolio;

using System;
using System.Diagnostics.Contracts;
using System.Threading;

namespace StockMarket {
    class StockTable {
        private Dictionary<string, float> marketValues = new Dictionary<string, float>
                                                        {{"AAPL", 110.57f},
                                                        {"GOOGL", 200.15f}, 
                                                        {"NVDA", 300.13f}, 
                                                        {"TSLA", 710.45f},
                                                        {"SCHWN", 25.63f}};
        
        public float GetValue(string symbol) {
            /* 
            Params:
                string symbol: Stock symbol needed for reference. 
            Returns:
                Value of the stock if it exists in the database. Returns 0.0f otherwise.
            */
            if (this.marketValues.TryGetValue(symbol, out float value)) { return value; } // Do checking here to see if symbol exists
            else { return 0.0f; }
        }

        // Function is called when the price should be dynamically updated
        public void SetTicker(string symbol, float newVal) {
            /* 
            Params:
                string symbol: Stock symbol needed for reference. 
                float newVal: Desired price to set stock in database.
            Returns:
                Void.
            */
            if (this.marketValues.TryGetValue(symbol, out float value)) {
                this.marketValues[symbol] = newVal;
                Console.WriteLine($"Symbol Price Adjusted: {symbol} with value: ${newVal}\n");
            } else {
                this.marketValues[symbol] = newVal;
                Console.WriteLine($"New Symbol Added: {symbol} with value: ${newVal}\n");
            }
        }
        

    }
}

namespace HedgeFund {
    class Fund {
        private int idCounter = 0;
        public string fundName; // Should be a unique name to distinguish the group of investors

        Dictionary<int, PersonalPortfolio> investorRolodex = new Dictionary<int, PersonalPortfolio>(); // { userId: Portfolio }

        public Fund(string name) {
            this.fundName = name;
        }

        // Use this function to acess or store a users portfolio
        public PersonalPortfolio GetInvestor(int id) {
            /* 
            Params:
                int id: Known id of investor within the investment group. 
            Returns:
                Personal Portfolio associated with the investor id. Return null otherwise.
            */
            // Returns the portfolio for a known investor id.
            if (this.investorRolodex.TryGetValue(id, out PersonalPortfolio portfolio)) {
                Console.WriteLine($"Investor {portfolio.Name} welcome back!\n");
                Console.WriteLine($"You have ${portfolio.Funds} in your account!\n");
                return portfolio;
            } else {
                Console.WriteLine($"Investor not found. Try adding investor to the database\n");
                return null;
            }
        }  

        public void AddInvestor(string name, float funds, StockTable stockTable) {
            /* 
            Params:
                string symbol: Stock symbol needed for reference. 
            Returns:
                Void. Prints to stdout to welcome investor to the firm. 
            */
            investorRolodex[idCounter] = new PersonalPortfolio(name, this.idCounter, funds, stockTable);
            Console.WriteLine($"Welcome to the investment {this.fundName} firm {name}!\n");
            this.idCounter++;
        }
    }
}


namespace StockPortfolio {
    class PersonalPortfolio {
        private string name;
        private int id;
        private float funds;
        private StockTable stockTable;
        public Dictionary<string, int> portfolio = new Dictionary<string, int>(); // Holdings should be visible for legal reasons
        public PersonalPortfolio(string name, int id, float funds, StockTable stockTable) {
            this.name = name;
            this.id = id;
            this.funds = funds;
            this.stockTable = stockTable;
        }
        
        public void BuyShares(string symbol, int shares) {
            /* 
            Params:
                string symbol: Stock symbol needed for reference.
                int shares: Desired number of shares to purchase. 
            Returns:
                Void. Prints to stdout to give user feedback on whether the purchase was successful or not. 
            */
            if (this.stockTable.GetValue(symbol) == 0.0f) {
                Console.WriteLine($"Symbol {symbol} is not in our database. Try another symbol\n");
                return;
            }
            float cost = shares * this.stockTable.GetValue(symbol);

            // Only make the purchase with sufficient funds
            if (cost <= this.funds) {
                if (this.portfolio.TryGetValue(symbol, out int holdings)) { // if there are already shares in the current investors portf.
                    this.portfolio[symbol] = holdings + shares; // Add the purchase shares to current shares1
                    
                } else {
                    this.portfolio[symbol] = shares; // No previous holdings
                }
                this.funds -= cost;
                Console.WriteLine($"Purchase Successful for {shares} shares of {symbol} @ ${cost/shares}\n");
            } else {
                Console.WriteLine($"Purchase was unsuccessful for {shares} shares of {symbol}. Add more funds and try again or ensure you are using the correct symbol.\n");
            }
        }

        public void SellShares(string symbol, int shares) {
            /* 
            Params:
                string symbol: Stock symbol needed for reference. 
                int shares: Desired number of shares to sell.
            Returns:
                Void. Prints to stdout to give feedback on whether sale was successful or not.
            */
            if (this.portfolio.TryGetValue(symbol, out int holdings))
            {
                float price = this.stockTable.GetValue(symbol) * shares;
                if (holdings >= shares) { // Check sufficient sellable shares
                    this.portfolio[symbol] = this.portfolio[symbol] - shares;
                    this.funds += price;
                    Console.WriteLine($"Successfully sold {shares} shares of {symbol} @ ${price}\n");
                } else {
                    Console.WriteLine($"Insufficient shares to sell for this transaction of {shares} shares of {symbol}, or symbol not in database. Please try again.\n");

                }
            } 
                
        }

        // Optional: Properties to access private fields
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public float Funds
        {
            get { return funds; }
            set { funds = value; }

        }
    }
}
  

class Program {
    static void Main(string[] args) {
        StockMarket.StockTable defaultStockTable = new StockMarket.StockTable(); // Stock table reference will be passed through to all portfolios.

        HedgeFund.Fund alphaInvestmentGroup = new HedgeFund.Fund("Alpha"); // Initialize the investment group

        string[] names = { "Allan", 
                           "Steve", 
                           "Reggie", 
                           "Ben", 
                           "Tom", 
                           "Alvin", 
                           "Lisa", 
                           "Kathy", 
                           "Hobart", 
                           "Norbit", 
                           "Cosmo", 
                           "Descartes"};
        
        float[] funds = { 20000f,
                          5000f,
                          30000.13f,
                          60000f,
                          59000f,
                          102400f,
                          108600f,
                          203400f,
                          346000f,
                          204800f,
                          309600f,
                          408800f};

        // Initiate the new investors
        for (int i = 0; i < 12; i++) {
            Thread.Sleep(1000);
            alphaInvestmentGroup.AddInvestor(names[i], funds[i], defaultStockTable);
        }
        
        // Display the investors' fund allocations
        for (int i = 0; i < 13; i++) {
            Thread.Sleep(1000);
            alphaInvestmentGroup.GetInvestor(i);
        }

        Console.WriteLine("\n\n");

        // Testing Investor Transactions using Allan, Ben and Cosmo
        StockPortfolio.PersonalPortfolio Allan = alphaInvestmentGroup.GetInvestor(0);
        
        // Test Transacting with Allan
        Console.WriteLine($"Allan's Funds Available @ Start: ${Allan.Funds}\n");
        Allan.BuyShares("AAPL", 15);
        Allan.BuyShares("TSLA", 100); // Balance with be insufficient
        Allan.BuyShares("TSLA", 2);
        Allan.SellShares("AAPL", 10);
        Allan.SellShares("TSLA", 3); // Holdings will be insufficient
        Console.WriteLine($"Allan's Funds Available @ End: ${Allan.Funds}\n");
        Console.WriteLine($"Allan's Holdings:\n");
        foreach (var kvp in Allan.portfolio) {
            Console.WriteLine($"{kvp.Key}: {kvp.Value} shares");
        }
        Thread.Sleep(4000);
        
        Console.WriteLine("\n\n");


        // Test Transacting with Ben
        StockPortfolio.PersonalPortfolio Ben = alphaInvestmentGroup.GetInvestor(3);
       
        Console.WriteLine($"Ben's Funds Available @ Start: ${Ben.Funds}\n");
        Ben.BuyShares("SCHWN", 50);
        Ben.BuyShares("GOOGL", 10); 
        Ben.BuyShares("NVDA", 2);
        Ben.BuyShares("Apple", 55); // Symbol does not exist
        Ben.SellShares("AAPL", 10); // No Holdings available
        Ben.SellShares("SCHWN", 15); 
        Console.WriteLine($"Ben's Funds Available @ End: ${Ben.Funds}\n");
        Console.WriteLine($"Ben's Holdings:\n");
        foreach (var kvp in Ben.portfolio) {
            Console.WriteLine($"{kvp.Key}: {kvp.Value} shares");
        }
        
        Thread.Sleep(4000);

        Console.WriteLine("\n\n");

        // Test Transacting with Cosmo
        StockPortfolio.PersonalPortfolio Cosmo = alphaInvestmentGroup.GetInvestor(10);

        Console.WriteLine($"Cosmo's Funds Available @ Start: ${Cosmo.Funds}\n");
        Cosmo.BuyShares("TIKTOK", 5); // Symbol does not exist
        Cosmo.BuyShares("GOOGL", 10); 
        Cosmo.BuyShares("GOOGL", 2);
        Cosmo.SellShares("NVDA", 10000); // Insufficient Funds
        Cosmo.SellShares("TSLA", 3); // Holdings will be insufficient
        Cosmo.BuyShares("TSLA", 12);
        Cosmo.SellShares("GOOGL", 12); // Liquidate Assets
        Console.WriteLine($"Cosmo's Funds Available @ End: ${Cosmo.Funds}\n");
        Console.WriteLine($"Cosmo's Holdings:\n");
        foreach (var kvp in Cosmo.portfolio) {
            Console.WriteLine($"{kvp.Key}: {kvp.Value} shares");
        }

    }

}