namespace TCViettelFC_API.Dtos
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public int UserId { get; set; }
        public int? RoleId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
