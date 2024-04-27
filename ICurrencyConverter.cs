using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SimpleCurrencyCalculator
{
    public interface ICurrencyConverter
    {
        Task<decimal> ConvertAsync(string amount, ICurrency fromCurrency, ICurrency toCurrency);
    }

    class MockCurrencyConverter : ICurrencyConverter
    {
        private readonly Dictionary<string, decimal> _exchangeRates;

        public MockCurrencyConverter()
        {
            // Initialize mock exchange rates
            _exchangeRates = new Dictionary<string, decimal>()
            {
                ["USD"] = 1.0m,
                ["EUR"] = 0.9m
            };
        }

        public async Task<decimal> ConvertAsync(string amount, ICurrency fromCurrency, ICurrency toCurrency)
        {
            // Simulate a delay before fetching the exchange rate
            await Task.Delay(10);

            if (!_exchangeRates.ContainsKey(fromCurrency.Name) || !_exchangeRates.ContainsKey(toCurrency.Name))
            {
                throw new ArgumentException("Unsupported currency");
            }

            decimal exchangeRate = _exchangeRates[toCurrency.Name] / _exchangeRates[fromCurrency.Name];
            return decimal.Parse(amount) * exchangeRate;
        }
    }

    class CurrencyConverterExchangeRateAPI : ICurrencyConverter
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public CurrencyConverterExchangeRateAPI(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public async Task<decimal> ConvertAsync(string amount, ICurrency fromCurrency, ICurrency toCurrency)
        {
            string url = $"https://api.exchangerate-api.com/v4/latest/{fromCurrency.Name}";

            using (var response = await _httpClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    var exchangeRateData = await response.Content.ReadAsStringAsync();
                    var exchangeRate = JsonConvert.DeserializeObject<ExchangeRateData>(exchangeRateData);
                    return decimal.Parse(amount) * exchangeRate.Rates[toCurrency.Name];
                }
                else
                {
                    throw new Exception($"Ошибка при получении курса валюты: {response.StatusCode}");
                }
            }
        }
    }

    class ExchangeRateData
    {
        public string Base { get; set; }
        public DateTime Date { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
    }
}
