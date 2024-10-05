using System.ComponentModel.DataAnnotations;

namespace TCViettetlFC_Client.Models
{
    public class UserLoginViewModel
    {
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
