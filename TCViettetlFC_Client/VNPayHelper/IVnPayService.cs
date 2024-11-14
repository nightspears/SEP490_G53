namespace TCViettetlFC_Client.VNPayHelper
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VNPaymentRequestModel vnpmodel,bool mul);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
