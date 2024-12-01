using System.ComponentModel.DataAnnotations;

namespace TCViettetlFC_Client.Models
{
    public class CustomerSendMailModel
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }
    }
}
