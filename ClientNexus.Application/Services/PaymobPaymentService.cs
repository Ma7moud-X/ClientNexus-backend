using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.Services
{
    public class PaymobPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly string _secretKey;
        private readonly string _publicKey;
        private readonly string _redirectionUrl;
        private readonly string _notificationUrl;
        private readonly string _baseUrl = "https://accept.paymob.com";
        private readonly int[] _paymentMethodIds;
        private readonly IConfiguration _configuration;

        public PaymobPaymentService(string secretKey, string publicKey, int[] paymentMethodIds, string redirectionUrl, string notificationUrl)
        {
            _httpClient = new HttpClient();
            _secretKey = secretKey;
            _publicKey = publicKey;
            _paymentMethodIds = paymentMethodIds;
            _publicKey = publicKey;
            _paymentMethodIds = paymentMethodIds;
            _redirectionUrl = redirectionUrl;
            _notificationUrl = notificationUrl;
        }

        public async Task<(string ClientSecret, string IntentionId)> StartPayment(
            decimal amount,
            string email,
            string firstName,
            string lastName,
            string phone,
            string specialReference,
            string[] items,
            int expiration = 3600)
        {
            var request = new
            {
                amount = (int)(amount * 100),
                currency = "EGP",
                payment_methods = _paymentMethodIds,
                billing_data = new
                {
                    email,
                    first_name = firstName,
                    last_name = lastName,
                    phone_number = phone,
                    apartment = "NA",
                    floor = "NA",
                    street = "NA",
                    building = "NA",
                    city = "NA",
                    country = "EG",
                    state = "NA",
                    postal_code = "NA"
                },
                items = items.Select(item => new
                {
                    name = item,
                    amount = (int)(amount * 100),
                    description = item,
                    quantity = 1
                }).ToArray(),
                special_reference = specialReference,
                expiration,
                notification_url = _notificationUrl,
                redirection_url =  _redirectionUrl         };

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Token", _secretKey);

            var response = await _httpClient.PostAsync($"{_baseUrl}/v1/intention/", content);
            response.EnsureSuccessStatusCode();

            var data = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
            return ((string)data.client_secret, (string)data.id);
        }

        public decimal GetSubscriptionAmount(string subscriptionType, string subscriptionTier)
        {
            var prices = new Dictionary<(string Type, string Tier), decimal>
            {
                { ("Monthly", "Normal"), 100.00m },
                { ("Monthly", "Advanced"), 150.00m },
                { ("Quarterly", "Normal"), 270.00m },
                { ("Quarterly", "Advanced"), 405.00m },
                { ("Yearly", "Normal"), 960.00m },
                { ("Yearly", "Advanced"), 1440.00m }
            };

            if (!prices.TryGetValue((subscriptionType, subscriptionTier), out var amount))
            {
                throw new ArgumentException("Invalid subscription type or tier");
            }

            return amount;
        }

        public string GetPublicKey()
        {
            return _publicKey;
        }
    }
}
