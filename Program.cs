using Microsoft.EntityFrameworkCore;
using PieShop.Models;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Identity;
using Serilog;
using BethanysPieShop.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();

builder.Services.AddControllersWithViews();

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddScoped<ICategoryRepository, MockCategoryRepository>();
builder.Services.AddScoped<IPieRepository, MockPieRepository>();
builder.Services.AddScoped<ICustomerRepository, MockCustomerRepository>();
builder.Services.AddScoped<IOrderRepository, MockOrderRepository>();

builder.Services.AddScoped<IShoppingCart, ShoppingCart>(xx => ShoppingCart.GetCart(xx));

builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<PieShopDBContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("QaraDbContextConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>(option => option.SignIn.RequireConfirmedEmail = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<PieShopDBContext>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DeleteAction",
         policy => policy.RequireRole("SuperAdmin"));
});


var app = builder.Build();

// app.MapGet("/", () => "Hello World!");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseStaticFiles();

app.MapRazorPages();



// app.MapDefaultControllerRoute();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.UseSession();
/** app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "products",
        pattern: "app/Products",
        defaults: new { controller = "Item", action = "List" }
    );
    endpoints.MapControllerRoute(
        name: "allproducts",
        pattern: "app/allproducts",
        defaults: new { controller = "Item", action = "List" }
    );
    endpoints.MapControllerRoute(
        name: "products",
        pattern: "app/products/{category}/{id}",
        defaults: new { controller = "Item", action = "Details" }
    );
}); */

DbInitializer.Seed(app);

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { "SuperAdmin", "Admin" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    var superAdminEmail = "yasir.s.albardawil@gmail.com";
    var superAdminUser = await userManager.FindByEmailAsync(superAdminEmail);
    if (superAdminUser != null)
    {
        await userManager.AddToRoleAsync(superAdminUser, "SuperAdmin");

        if (!await userManager.IsInRoleAsync(superAdminUser, "SuperAdmin"))
        {
            await userManager.AddToRoleAsync(superAdminUser, "SuperAdmin");
        }
    }

    var adminEmail = "admin@gmail.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser != null)
    {
        await userManager.AddToRoleAsync(adminUser, "Admin");

        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}


app.UseSerilogRequestLogging();

app.Run();
