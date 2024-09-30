namespace TCViettelFC_API.Dtos
{
    public class UserDto
    {
        public int UserId { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Password { get; set; }

        public string? FullName { get; set; }

        public int? RoleId { get; set; }

        public int? Status { get; set; }

        public DateTime? CreatedAt { get; set; }
        public string? RoleName { get; set; }
    }

    // Use this DTO for Add (POST), excluding UserId
    public class UserCreateDto
    {

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Password { get; set; }

        public string? FullName { get; set; }

        public int? RoleId { get; set; }

        public int? Status { get; set; }

        public DateTime? CreatedAt { get; set; }
    }

    public class UserUpdateDto
    {
        public int UserId { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Password { get; set; }

        public string? FullName { get; set; }

        public int? RoleId { get; set; }

        public int? Status { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
