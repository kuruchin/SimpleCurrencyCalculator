using System.Threading.Tasks;

namespace SimpleCurrencyCalculator
{
    public interface ICalculator
    {
        Task<decimal> AddAsync(string amount1, ICurrency currency1, string amount2, ICurrency currency2);
        Task<decimal> SubtractAsync(string amount1, ICurrency currency1, string amount2, ICurrency currency2);
    }

    class Calculator : ICalculator
    {
        private readonly ICurrencyConverter _currencyConverter;

        public Calculator(ICurrencyConverter currencyConverter)
        {
            _currencyConverter = currencyConverter;
        }

        public async Task<decimal> AddAsync(string amount1, ICurrency currency1, string amount2, ICurrency currency2)
        {
            decimal convertedAmount1 = await _currencyConverter.ConvertAsync(amount1, currency1, currency1);
            decimal convertedAmount2 = await _currencyConverter.ConvertAsync(amount2, currency2, currency1);

            return convertedAmount1 + convertedAmount2;
        }

        public async Task<decimal> SubtractAsync(string amount1, ICurrency currency1, string amount2, ICurrency currency2)
        {
            decimal convertedAmount1 = await _currencyConverter.ConvertAsync(amount1, currency1, currency1);
            decimal convertedAmount2 = await _currencyConverter.ConvertAsync(amount2, currency2, currency1);

            return convertedAmount1 - convertedAmount2;
        }
    }
}
