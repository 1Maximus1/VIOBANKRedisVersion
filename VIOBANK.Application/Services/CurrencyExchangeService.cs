using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace VIOBANK.Application.Services
{
    public class CurrencyExchangeService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public CurrencyExchangeService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["ExchangeRateApi:ApiKey"];
            _baseUrl = configuration["ExchangeRateApi:BaseUrl"];
        }

        public async Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency)
        {
            var url = $"{_baseUrl}{_apiKey}/latest/{fromCurrency}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get exchange rate: {response.ReasonPhrase}");
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (!root.TryGetProperty("conversion_rates", out var rates) ||
                !rates.TryGetProperty(toCurrency, out var rate))
            {
                throw new Exception($"Exchange rate from {fromCurrency} to {toCurrency} not found.");
            }

            return rate.GetDecimal();
        }
    }
}

