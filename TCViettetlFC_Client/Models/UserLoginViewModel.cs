using System.ComponentModel.DataAnnotations;

namespace TCViettetlFC_Client.Models
{
    public class UserLoginViewModel
    {
        [Required(ErrorMessage = "Phone is required")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
