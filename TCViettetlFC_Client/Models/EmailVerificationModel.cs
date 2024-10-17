using System.ComponentModel.DataAnnotations;

namespace TCViettetlFC_Client.Models
{
    public class EmailVerificationModel
    {
        public string Email { get; set; }
        [Required(ErrorMessage = "Mã xác nhận không được để trống")]
        public string Code { get; set; }
    }
}
