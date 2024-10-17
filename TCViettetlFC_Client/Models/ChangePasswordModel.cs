using System.ComponentModel.DataAnnotations;
namespace TCViettetlFC_Client.Models
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Mật khẩu cũ không được để trống")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Phải nhập lại mật khẩu mới")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu không khớp")]
        public string ReNewPassword { get; set; }
    }

}
