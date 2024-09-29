using System.ComponentModel.DataAnnotations;
namespace TCViettetlFC_Client.Models
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "New password is required")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "New password is required")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Please re-enter the new password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ReNewPassword { get; set; }
    }

}
