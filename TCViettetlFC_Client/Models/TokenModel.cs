namespace TCViettetlFC_Client.Models
{
    public class TokenModel
    {
        public string token { get; set; }
        public int userId { get; set; }
        public int roleId { get; set; }
        public string fullName { get; set; }
        public string email { get; set; }
        public string password { get; set; }
    }
}
