namespace TCViettelFC_API.Dtos
{
    public class AdminLoginResponse
    {
        public string Token { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
