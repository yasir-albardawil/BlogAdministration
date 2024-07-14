using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PieShop.Models;
using PieShop.ViewModels;
using System.Threading.Tasks;

[Authorize(Roles = "SuperAdmin")]
public class UserController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> List(string id, string search, string sortOrder)
    {
        ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

        ViewData["IdSortParm"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";

        IEnumerable<IdentityUser> users;

        if (string.IsNullOrEmpty(id))
        {
            users = _userManager.Users;
        }
        else
        {
            users = _userManager.Users.Where(u => u.Id == id);
        }

        if (!string.IsNullOrEmpty(search))
        {

            users = _userManager.Users.Where(u => u.UserName!.Contains(search));

        }

        switch (sortOrder)
        {
            case "name_desc":
                users = users.OrderByDescending(u => u.UserName);
                break;
            case "id_desc":
                users = users.OrderByDescending(u => u.Id);
                break;
            default:
                users = users.OrderBy(u => u.UserName);
                break;
        }

        var roles = await _roleManager.Roles.ToListAsync();
        var userListViewModel = new UserListViewModel(users, roles);
        return View(userListViewModel);
    }

    public async Task<IActionResult> Details(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }
}
