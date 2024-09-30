namespace TCViettetlFC_Client.Models
{
    public class UserManagementViewModel
    {
        public List<UserViewModel> Users { get; set; } = new List<UserViewModel>();
        public List<RoleViewModel> Roles { get; set; } = new List<RoleViewModel>();
        public int? EditingUserId { get; set; } // Add this line
    }
}
