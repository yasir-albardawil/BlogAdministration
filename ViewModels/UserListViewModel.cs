using Microsoft.AspNetCore.Identity;
namespace PieShop.ViewModels
{
    public class UserListViewModel
    {

        public IEnumerable<IdentityUser> AllUsers { get; }
        public IEnumerable<IdentityRole> AllRoles { get; }


        public UserListViewModel(IEnumerable<IdentityUser> users, IEnumerable<IdentityRole> roles)
        {
            AllUsers = users;
            AllRoles = roles;
        }
    }
}
