using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json;

namespace BanCaPhe.Services
{
    public class PayOSPaymentResult
    {
        public string CheckoutUrl { get; set; }
        public string QrCode { get; set; }
        public long OrderCode { get; set; }
    }

    public class PayOSService
    {
        private readonly HttpClient _httpClient;

        public PayOSService()
        {
            _httpClient = new HttpClient();
            // Địa chỉ API (HTTPS của project BanCafe_API)
            _httpClient.BaseAddress = new Uri("https://localhost:7250/");
        }

        public async Task<PayOSPaymentResult> CreatePaymentLink(long amount)
        {
            try
            {
                // Gọi API tạo link thanh toán
                var response = await _httpClient.PostAsync($"api/Wallet/create-payment-link?Amount={amount}", null);

                var jsonString = await response.Content.ReadAsStringAsync();
                
                if (!string.IsNullOrEmpty(jsonString))
                {
                    using var doc = JsonDocument.Parse(jsonString);
                    var root = doc.RootElement;

                    JsonElement data;
                    if (root.TryGetProperty("response", out var responseElement))
                    {
                        data = responseElement;
                    }
                    else
                    {
                        data = root;
                    }

                    return new PayOSPaymentResult
                    {
                        CheckoutUrl = data.TryGetProperty("checkoutUrl", out var url) ? url.GetString() : null,
                        QrCode = data.TryGetProperty("qrCode", out var qr) ? qr.GetString() : null,
                        OrderCode = data.TryGetProperty("orderCode", out var oc) ? oc.GetInt64() : 0
                    };
                }

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(jsonString);
                }
                
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi kết nối API: " + ex.Message);
            }
        }

        public async Task<string> CheckPaymentStatus(long orderCode)
        {
            try
            {
                // Giả định API có endpoint kiểm tra trạng thái theo orderCode
                var response = await _httpClient.GetAsync($"api/Wallet/get-order-info/{orderCode}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(jsonString);
                    var root = doc.RootElement;
                    
                    // Thường PayOS trả về status như "PAID", "PENDING", "CANCELLED"
                    if (root.TryGetProperty("status", out var status))
                    {
                        return status.GetString();
                    }
                }
                return "PENDING";
            }
            catch { return "ERROR"; }
        }
    }
}
