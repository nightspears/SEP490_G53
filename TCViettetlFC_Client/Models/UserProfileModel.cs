using System.ComponentModel.DataAnnotations;

namespace TCViettetlFC_Client.Models
{
	public class UserProfileModel
	{
		[Required(ErrorMessage = "Số điện thoại không được để trống")]
		[RegularExpression(@"^(84|0[3|5|7|8|9])\d{8}$", ErrorMessage = "Số điện thoại không hợp lệ. Vui lòng kiểm tra lại.")]
		public string? phone { get; set; }
		[Required(ErrorMessage = "Tên đầy đủ không được để trống")]
		public string? fullName { get; set; }
	}
}
