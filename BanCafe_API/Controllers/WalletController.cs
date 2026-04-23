using Microsoft.AspNetCore.Mvc;
using PayOS_Integration.Services;

namespace PayOS_Integration.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletServices;

        public WalletController(IWalletService walletServices)
        {
            _walletServices = walletServices;
        }

        [HttpPost("create-payment-link")]
        public async Task<IActionResult> CreatePaymentLink(long Amount)
        {
            try
            {
                var result = await _walletServices.Create(Amount);
                // Trả về Ok (200) trực tiếp để WPF nhận diện thành công
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("get-order-info/{orderCode}")]
        public async Task<IActionResult> GetOrderInfo(long orderCode)
        {
            try
            {
                var result = await _walletServices.GetOrderInfo(orderCode);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
