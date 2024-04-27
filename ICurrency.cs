namespace SimpleCurrencyCalculator
{
    public interface ICurrency
    {
        string Symbol { get; }
        string Name { get; }
    }

    public class USDCurrency : ICurrency
    {
        public string Symbol => "$";
        public string Name => "USD";
    }

    public class EURCurrency : ICurrency
    {
        public string Symbol => "€";
        public string Name => "EUR";
    }

}
