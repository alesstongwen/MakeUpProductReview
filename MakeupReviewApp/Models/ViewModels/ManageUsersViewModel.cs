using System.Collections.Generic;

namespace MakeupReviewApp.Models.ViewModels
{
    public class ManageUsersViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<UserRoleViewModel> Users { get; set; }
    }
}