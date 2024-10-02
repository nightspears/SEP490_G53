namespace TCViettelFC_API.Dtos
{
    public class CustomerLoginResponse
    {
        public string token { get; set; }
        public int customerId { get; set; }

        public string email { get; set; }

        public string phone { get; set; }
    }
}
