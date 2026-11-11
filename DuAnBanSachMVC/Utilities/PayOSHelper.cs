using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DuAnBanSachMVC.Utilities
{
    public class PayOSHelper
    {
        private readonly string _clientId;
        private readonly string _apiKey;
        private readonly string _checksumKey;

        public PayOSHelper()
        {
            _clientId = ConfigurationManager.AppSettings["PayOS_ClientId"];
            _apiKey = ConfigurationManager.AppSettings["PayOS_ApiKey"];
            _checksumKey = ConfigurationManager.AppSettings["PayOS_ChecksumKey"];
        }

        // Tạo chữ ký HMAC SHA256
        private string CreateSignature(string data)
        {
            var keyBytes = Encoding.UTF8.GetBytes(_checksumKey);
            var dataBytes = Encoding.UTF8.GetBytes(data);
            using (var hmac = new HMACSHA256(keyBytes))
            {
                var hash = hmac.ComputeHash(dataBytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        /// <summary>
        /// Tạo link thanh toán PayOS
        /// </summary>
        public async Task<string> CreatePaymentLink(long orderCode, int amount, string description, string returnUrl, string cancelUrl)
        {
            // Chuỗi data để ký (theo đúng thứ tự PayOS yêu cầu)
            string rawData = $"amount={amount}&cancelUrl={cancelUrl}&description={description}&orderCode={orderCode}&returnUrl={returnUrl}";
            string signature = CreateSignature(rawData);

            var body = new
            {
                orderCode = orderCode,
                amount = amount,
                description = description,
                cancelUrl = cancelUrl,
                returnUrl = returnUrl,
                signature = signature
            };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("x-client-id", _clientId);
                client.DefaultRequestHeaders.Add("x-api-key", _apiKey);

                var json = JsonConvert.SerializeObject(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://api-merchant.payos.vn/v2/payment-requests", content);
                var responseString = await response.Content.ReadAsStringAsync();

                var result = JObject.Parse(responseString);

                if (result["code"]?.ToString() == "00")
                {
                    // Trả về checkoutUrl để redirect khách
                    return result["data"]?["checkoutUrl"]?.ToString();
                }

                throw new Exception("Tạo link PayOS thất bại: " + result["desc"]);
            }
        }
    }
}