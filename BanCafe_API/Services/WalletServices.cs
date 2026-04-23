using PayOS;
using PayOS.Models;
using PayOS.Models.V2.PaymentRequests;

namespace PayOS_Integration.Services
{

    public interface IWalletService
    {
        Task<dynamic> Create(long Amount);
        Task<dynamic> GetOrderInfo(long orderCode);
    }
    public class WalletServices : IWalletService
    {
      
        private readonly IConfiguration _configuration;

        public WalletServices( IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private PayOSClient GetPayOSClient()
        {
            var clientId = _configuration["PayOS:ClientId"];
            var apiKey = _configuration["PayOS:ApiKey"];
            var checksumKey = _configuration["PayOS:ChecksumKey"];
            return new PayOSClient(clientId, apiKey, checksumKey);
        }

        public async Task<dynamic> Create(long Amount)
        {
            var domain = _configuration["PayOS:ReturnUrl"];
            var payOS = GetPayOSClient();

            var paymentLinkRequest = new CreatePaymentLinkRequest
            {
                OrderCode = long.Parse(DateTimeOffset.Now.ToString("ffffff")),
                Amount = Amount,
                Description = "Thanh toan don hang",
                ReturnUrl = domain,
                CancelUrl = domain
            };
            var response = await payOS.PaymentRequests.CreateAsync(paymentLinkRequest);
            return new { response };
        }

        public async Task<dynamic> GetOrderInfo(long orderCode)
        {
            var payOS = GetPayOSClient();
            var response = await payOS.PaymentRequests.GetAsync(orderCode);
            return response;
        }
    }
}
