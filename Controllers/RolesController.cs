using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class RolesController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public RolesController(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<IActionResult> CreateRole()
    {
        var roleExists = await _roleManager.RoleExistsAsync("User");
        if (!roleExists)
        {
            var role = new IdentityRole { Name = "User" };
            await _roleManager.CreateAsync(role);
        }
        return View();
    }
}