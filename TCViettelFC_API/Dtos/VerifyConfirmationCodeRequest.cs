namespace TCViettelFC_API.Dtos
{
    public class VerifyConfirmationCodeRequest
    {
        public string Email { get; set; }
        public string Code { get; set; }
    }
}
