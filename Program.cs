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

            ICurrencyConverter currencyConverter = null;
            bool getConverterLoop = true;

            while (getConverterLoop)
            {
                currencyConverter = GetConverter(out getConverterLoop);
            }

            // Create calculator
            ICalculator calculator = new Calculator(currencyConverter);

            while (true)
            {
                Console.Write("Введите выражение (сумма или разность) в формате \"число1$ + число2€\": ");
                string expression = Console.ReadLine();

                try
                {
                    // Regex pattern to find values, currencies, and operation sign
                    string pattern = @"(\d+([.,]\d+)?)\s*([^\d.,]+)";

                    MatchCollection matches = Regex.Matches(expression, pattern);

                    // Check if we have the correct number of matches
                    if (matches.Count != 2)
                    {
                        Console.WriteLine("Ошибка в распознавании строки.");
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
                        Console.WriteLine($"Сумма : {result} {currency1.Symbol}\n");
                    }
                    else if (expression.Contains("-"))
                    {
                        result = await calculator.SubtractAsync(value1.ToString(), currency1, value2.ToString(), currency2);
                        Console.WriteLine($"Разность: {result} {currency1.Symbol}\n");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}\n");
                }
            }
        }

        static ICurrencyConverter GetConverter(out bool getConverterLoop)
        {
            Console.Write("Введите 1 для мок-конвертера или 2 для реального: ");
            string converter = Console.ReadLine();

            switch (converter)
            {
                case string s when s.Contains("1"):
                    getConverterLoop = false;
                    return new MockCurrencyConverter();
                case string s when s.Contains("2"):
                    string apiKey = "cb9da4e12ddb48bc577825b3";
                    getConverterLoop = false;
                    return new CurrencyConverterExchangeRateAPI(apiKey);
                default:
                    Console.Write("Неизвестный символ конвертера\n");
                    getConverterLoop = true;
                    return null;
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
                    throw new ArgumentException("Неизвестный символ валюты");
            }
        }
    }
}
