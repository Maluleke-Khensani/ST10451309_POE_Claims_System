using Claims_System.Areas.Identity.Data;
using Claims_System.Models;
using Claims_System.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//  use SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.
    GetConnectionString("DefaultConnection"),
     sql => sql.CommandTimeout(60) // 60 seconds
    )
);


builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();




// Dependency Injection for ClaimService
builder.Services.AddScoped<IClaimService, ClaimService>();

// Configure session storage
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Ensure Razor Pages are registered
builder.Services.AddRazorPages();

var app = builder.Build();

// -------------------- SEED DATA --------------------
//I dont need it anymore since I have already seeded the data in the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var context = services.GetRequiredService<ApplicationDbContext>(); // <--- add this

    //await IdentitySeeder.SeedData(userManager, roleManager, context);

}


// -----------------------------------------------------

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Map Razor Pages so /Identity/Account/Login becomes available
app.MapRazorPages();

app.MapControllerRoute(
  name: "default",
  pattern: "{controller=Home}/{action=Index}/{id?}");

  //name: "hr",
    //pattern: "{controller=HR}/{action=Dashboard}/{id?}");

await app.RunAsync();
