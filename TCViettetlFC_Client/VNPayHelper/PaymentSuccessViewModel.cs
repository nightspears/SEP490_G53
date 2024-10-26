using TCViettetlFC_Client.Models;

namespace TCViettetlFC_Client.VNPayHelper
{
    public class PaymentSuccessViewModel
    {
        public CheckoutModel CheckoutModel { get; set; }
        public VnPaymentResponseModel VnPayResponse { get; set; }
    }
}
