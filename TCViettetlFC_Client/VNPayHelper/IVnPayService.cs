namespace TCViettetlFC_Client.VNPayHelper
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VNPaymentRequestModel vnpmodel);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
