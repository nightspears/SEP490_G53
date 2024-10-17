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
        [Required(ErrorMessage = "Tên đầy đủ không được để trống")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải bao gồm 10 chữ số.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vai trò không được để trống")]
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
