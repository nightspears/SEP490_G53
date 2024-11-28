using System.ComponentModel.DataAnnotations;

namespace TCViettetlFC_Client.Models
{
	public class CustomerChangePassRequest
	{
		[Required(ErrorMessage = "Mật khẩu cũ không được để trống")]
		public string OldPass { get; set; }
		[Required(ErrorMessage = "Mật khẩu mới không được để trống")]
		[MinLength(8, ErrorMessage = "Mật khẩu mới phải có ít nhất 8 ký tự")]
		[DataType(DataType.Password)]
		public string NewPassword { get; set; }

		[Required(ErrorMessage = "Phải nhập lại mật khẩu mới")]
		[DataType(DataType.Password)]
		[Compare("NewPassword", ErrorMessage = "Mật khẩu không khớp")]
		public string RePassword { get; set; }
	}
}
