using Claims_System.Areas.Identity.Data;
using Claims_System.Models;
using Claims_System.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Claims_System.Controllers
{
    public class HRController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public HRController(UserManager<ApplicationUser> userManager,
                            RoleManager<IdentityRole> roleManager,
                            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }


        public async Task<IActionResult> Dashboard()
        {
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.PendingClaims = await _context.LecturerClaims
                .Where(c => c.ManagerStatus == "Pending")
                .CountAsync();

            ViewBag.ApprovedClaims = await _context.LecturerClaims
                .Where(c => c.ManagerStatus == "Approved" && c.CoordinatorStatus == "Approved")
                .CountAsync();

            ViewBag.RejectedClaims = await _context.LecturerClaims
                .Where(c => c.ManagerStatus == "Rejected" || c.CoordinatorStatus == "Rejected")
                .CountAsync();

            return View();
        }


        // Assited by ChatGPT to implement user management functionality, as it was difficult to apply roles and profiles correctly
        public IActionResult CreateUser()
        {
            ViewBag.Roles = _roleManager.Roles.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(ApplicationUser model, string password, string role, decimal? hourlyRate)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = _roleManager.Roles.ToList();
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                ViewBag.Roles = _roleManager.Roles.ToList();
                return View(model);
            }

            await _userManager.AddToRoleAsync(user, role);

            // ONLY lecturers get profiles
            if (role == "Lecturer" && hourlyRate.HasValue)
            {
                var profile = new LecturerProfile
                {
                    UserId = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    HourlyRate = hourlyRate.Value
                };

                _context.LecturerProfiles.Add(profile);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ListUsers");
        }

        // ================= LIST USERS =================
        public async Task<IActionResult> ListUsers()
        {
            var users = await _userManager.Users.ToListAsync();

            var userRoles = new Dictionary<string, string>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles[user.Id] = roles.FirstOrDefault() ?? "N/A";
            }

            ViewBag.UserRoles = userRoles;

            return View(users);
        }

        // ================= EDIT USER =================
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

            var profile = await _context.LecturerProfiles
                                        .FirstOrDefaultAsync(p => p.UserId == user.Id);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                CurrentRole = role,
                HourlyRate = profile?.HourlyRate ?? 0,
                AllRoles = _roleManager.Roles.Select(r => r.Name).ToList()
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            // Update user
            user.FullName = model.FullName;
            user.Email = model.Email;
            user.UserName = model.Email;

            await _userManager.UpdateAsync(user);

            // Handle role change
            var oldRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

            if (oldRole != model.NewRole)
            {
                if (oldRole != null)
                    await _userManager.RemoveFromRoleAsync(user, oldRole);

                await _userManager.AddToRoleAsync(user, model.NewRole);
            }

            // Handle LecturerProfile
            var profile = await _context.LecturerProfiles
                                        .FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (model.NewRole == "Lecturer")
            {
                if (profile == null)
                {
                    profile = new LecturerProfile
                    {
                        UserId = user.Id,
                        FullName = user.FullName,
                        Email = user.Email,
                        HourlyRate = model.HourlyRate
                    };

                    _context.LecturerProfiles.Add(profile);
                }
                else
                {
                    profile.FullName = user.FullName;
                    profile.Email = user.Email;
                    profile.HourlyRate = model.HourlyRate;
                }
            }
            else
            {
                if (profile != null)
                    _context.LecturerProfiles.Remove(profile);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("ListUsers");
        }


        [HttpPost]
        public async Task<IActionResult> DeleteUserConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Remove lecturer profile if exists
            var profile = await _context.LecturerProfiles
                                        .FirstOrDefaultAsync(p => p.UserId == id);

            if (profile != null)
            {
                _context.LecturerProfiles.Remove(profile);
                await _context.SaveChangesAsync();
            }

            // Delete the user from Identity
            await _userManager.DeleteAsync(user);

            return RedirectToAction("ListUsers");
        }
        // GET: List all lecturers
        public async Task<IActionResult> GenerateReport()
        {
            var lecturers = await _context.LecturerProfiles.ToListAsync();
            return View(lecturers); // View displays "Generate PDF" buttons per lecturer
        }

        // GET: Generate PDF for a lecturer
        public async Task<IActionResult> GenerateReportForLecturer(string userId)
        {
            var lecturer = await _context.LecturerProfiles.FirstOrDefaultAsync(l => l.UserId == userId);
            if (lecturer == null) return NotFound();

            var claims = await _context.LecturerClaims
                .Where(c => c.UserId == userId &&
                            c.ManagerStatus == "Approved" &&
                            c.CoordinatorStatus == "Approved")
                .OrderBy(c => c.Year)
                .ThenBy(c => c.Month)
                .ToListAsync();

            byte[] pdfBytes = PdfGeneratorHelper.CreateLecturerReport(lecturer, claims);

            return File(pdfBytes, "application/pdf", $"{lecturer.FullName}-Report.pdf");
        }


    }
}
