using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SimpleCurrencyCalculator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            Console.InputEncoding = Encoding.Unicode;

            // API Key
            string apiKey = "cb9da4e12ddb48bc577825b3";

            // Create currency converter
            ICurrencyConverter currencyConverter = new MockCurrencyConverter();
            // Uncomment to get the real rates
            //ICurrencyConverter currencyConverter = new CurrencyConverterExchangeRateAPI(apiKey);

            // Create calculator
            ICalculator calculator = new Calculator(currencyConverter);

            while (true) // Infinite loop to keep the console open
            {
                Console.Write("Enter expression (sum or difference) in format \"number1$ + number2€\": ");
                string expression = Console.ReadLine();

                try
                {
                    // Regex pattern to find values, currencies, and operation sign
                    string pattern = @"(\d+([.,]\d+)?)\s*([^\d.,]+)";

                    MatchCollection matches = Regex.Matches(expression, pattern);

                    // Check if we have the correct number of matches
                    if (matches.Count != 2)
                    {
                        Console.WriteLine("Error parsing expression.");
                        continue;
                    }

                    // Extract values, currencies, and operation sign
                    double value1 = double.Parse(matches[0].Groups[1].Value.Replace('.', ','));
                    ICurrency currency1 = GetCurrency(matches[0].Groups[3].Value.Trim());

                    double value2 = double.Parse(matches[1].Groups[1].Value.Replace('.', ','));
                    ICurrency currency2 = GetCurrency(matches[1].Groups[3].Value.Trim());

                    decimal result;

                    if (expression.Contains("+"))
                    {
                        result = await calculator.AddAsync(value1.ToString(), currency1, value2.ToString(), currency2);
                        Console.WriteLine($"Sum: {result} {currency1.Symbol}");
                    }
                    else if (expression.Contains("-"))
                    {
                        result = await calculator.SubtractAsync(value1.ToString(), currency1, value2.ToString(), currency2);
                        Console.WriteLine($"Difference: {result} {currency1.Symbol}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        // Method for getting the currency code
        static ICurrency GetCurrency(string currency)
        {
            switch (currency)
            {
                case string s when s.Contains("$"):
                    return new USDCurrency();
                case string s when s.Contains("€"):
                    return new EURCurrency();
                default:
                    throw new ArgumentException("Unknown currency symbol");
            }
        }
    }
}
