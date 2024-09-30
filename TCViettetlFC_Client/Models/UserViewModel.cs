using System.ComponentModel.DataAnnotations;

namespace TCViettetlFC_Client.Models
{
    public class UserViewModel
    {
        public int userId { get; set; }

        public string? email { get; set; }

        public string? phone { get; set; }

        public string? password { get; set; }

        public string? fullName { get; set; }

        public int? roleId { get; set; }

        public int? status { get; set; }

        public DateTime? createdAt { get; set; }
        public string? roleName { get; set; }
    }

    // Use this DTO for Add (POST), excluding UserId
    public class UserCreateDto
    {
        [Required(ErrorMessage = "Full Name is required.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone is required.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        public int RoleId { get; set; }
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
