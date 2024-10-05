using System.ComponentModel.DataAnnotations;

namespace TCViettetlFC_Client.Models
{
    public class CustomerLoginModel
    {
        [Required(ErrorMessage = "Email không được để trống")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
