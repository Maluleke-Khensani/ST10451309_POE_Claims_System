using Claims_System.Models;
using Claims_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize(Roles = "Lecturer,HR")]
public class LecturerController : Controller
{
    private readonly IClaimService _claimService;
    private readonly UserManager<ApplicationUser> _userManager;

    public LecturerController(IClaimService claimService,
                              UserManager<ApplicationUser> userManager)
    {
        _claimService = claimService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        var allClaims = await _claimService.GetAllClaimsAsync();
        var userClaims = allClaims.Where(c => c.UserId == user.Id);

        return View(userClaims);
    }

    [Authorize(Roles = "Lecturer")]
    public async Task<IActionResult> Profile()
    {
        // Get the logged-in user
        var user = await _userManager.Users
            .Include(u => u.LecturerProfile)
            .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

        if (user == null)
            return RedirectToAction("Index", "Home");

        // Get claims of this lecturer
        var claims = await _claimService.GetAllClaimsAsync();
        var userClaims = claims.Where(c => c.UserId == user.Id).ToList();

        var model = new LecturerProfileViewModel
        {
            FullName = user.FullName,
            Email = user.Email,
            HourlyRate = user.LecturerProfile?.HourlyRate ?? 0,
            Claims = userClaims
        };

        return View(model);
    }


    public async Task<IActionResult> ClaimForm()
    {
        var user = await _userManager.Users
            .Include(u => u.LecturerProfile)
            .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

        if (user == null)
            return RedirectToAction("Index", "Home");

        var model = new LecturerClaimViewModel
        {
            Month = DateTime.Now.Month,
            Year = DateTime.Now.Year,
            HoursWorked = 0,
            ModuleName = "",
            Notes = ""
        };

        // Pre-fill ViewBag for display
        ViewBag.Username = user.Email;
        ViewBag.FullName = user.FullName;
        ViewBag.Submitted = DateTime.Now.ToString("yyyy-MM-dd");
        ViewBag.Rate = user.LecturerProfile?.HourlyRate ?? 0;

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ClaimForm(LecturerClaimViewModel model)
    {
        var user = await _userManager.Users
            .Include(u => u.LecturerProfile)
            .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));
        if (user == null)
            return RedirectToAction("Index", "Home");

        if (!ModelState.IsValid)
        {
            // Re-populate ViewBag in case of error
            ViewBag.Username = user.Email;
            ViewBag.FullName = user.FullName;
            ViewBag.Submitted = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.Rate = user.LecturerProfile?.HourlyRate ?? 0;

            return View(model);
        }

        var hourlyRate = user.LecturerProfile?.HourlyRate ?? 0;

        var claim = new LecturerClaim
        {
            UserId = user.Id,
            Username = user.Email,
            FullName = user.FullName,
            Submitted = DateTime.Now,
            Month = model.Month,
            Year = model.Year,
            ModuleName = model.ModuleName,
            HoursWorked = model.HoursWorked,
            Rate = hourlyRate,
            Notes = model.Notes
        };

        // Handle documents
        await _claimService.CreateClaimAsync(claim,
            Request.Form.Files["Document1"],
            Request.Form.Files["Document2"]);

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> ViewClaim(int id)
    {
        // Get logged-in user
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return RedirectToAction("Index", "Home");

        // Fetch claim
        var claim = await _claimService.GetClaimByIdAsync(id);

        if (claim == null)
            return NotFound();

        // Security check: Ensure the logged-in lecturer owns the claim
        if (claim.UserId != user.Id)
            return Unauthorized();

        return View(claim);
    }

 
    public async Task<IActionResult> Edit(int id)
    {
        var username = HttpContext.Session.GetString("Username");
        var claim = await _claimService.GetClaimByIdAsync(id);

        if (claim == null || claim.Username != username)
        {
            TempData["Error"] = "Claim not found or you cannot edit it.";
            return RedirectToAction("Index");
        }

        if (claim.CoordinatorStatus != "Pending" || claim.ManagerStatus != "Pending")
        {
            TempData["Error"] = "Claim has already been reviewed and cannot be edited.";
            return RedirectToAction("Index");
        }

        return View(claim);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, LecturerClaim model)
    {
        var username = HttpContext.Session.GetString("Username");
        if (model.Username != username)
        {
            TempData["Error"] = "You are not authorized to edit this claim.";
            return RedirectToAction("Index");
        }

        var result = await _claimService.UpdateClaimAsync(model);

        TempData[result ? "Success" : "Error"] = result
            ? "Claim updated successfully!"
            : "Failed to update claim. Please try again.";

        return RedirectToAction("Index");
    }



    public async Task<IActionResult> Delete(int id)
    {
        var username = HttpContext.Session.GetString("Username");
        var claim = await _claimService.GetClaimByIdAsync(id);

        if (claim == null || !claim.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
        {
            TempData["Error"] = "Claim not found or you cannot delete it.";
            return RedirectToAction("Index");
        }

        if (claim.ManagerStatus != "Pending")
        {
            TempData["Error"] = "Claim has been reviewed and cannot be deleted.";
            return RedirectToAction("Index");
        }

        return View(claim);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var username = HttpContext.Session.GetString("Username");
        var claim = await _claimService.GetClaimByIdAsync(id);

        if (claim == null || !claim.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
        {
            TempData["Error"] = "Claim not found or unauthorized action.";
            return RedirectToAction("Index");
        }

        var success = await _claimService.DeleteClaimAsync(id);

        TempData[success ? "Success" : "Error"] = success
            ? "Claim deleted successfully!"
            : "Failed to delete claim. Please try again.";

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> MyReports()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Index", "Home");

        var approvedClaims = (await _claimService.GetAllClaimsAsync())
            .Where(c => c.UserId == user.Id &&
                        c.ManagerStatus == "Approved" &&
                        c.CoordinatorStatus == "Approved")
            .ToList();

        return View(approvedClaims);
    }


    [HttpGet]
    public async Task<IActionResult> DownloadDocument1(int claimId)
    {
        var claim = await _claimService.GetClaimByIdAsync(claimId);
        if (claim == null || claim.Document1FileData == null || string.IsNullOrEmpty(claim.Document1FileName))
            return NotFound();

        return File(claim.Document1FileData, "application/pdf", claim.Document1FileName);
    }

    [HttpGet]
    public async Task<IActionResult> DownloadDocument2(int claimId)
    {
        var claim = await _claimService.GetClaimByIdAsync(claimId);
        if (claim == null || claim.Document2FileData == null || string.IsNullOrEmpty(claim.Document2FileName))
            return NotFound();

        return File(claim.Document2FileData, "application/pdf", claim.Document2FileName);
    }


    [HttpGet]
    public async Task<IActionResult> DownloadDocument(int claimId, int docNumber)
    {
        var claim = await _claimService.GetClaimByIdAsync(claimId);
        if (claim == null) return NotFound();

        byte[] fileData;
        string fileName;
        string contentType;

        if (docNumber == 1)
        {
            fileData = claim.Document1FileData;
            fileName = claim.Document1FileName;
            contentType = "application/pdf"; // adjust by extension
        }
        else if (docNumber == 2)
        {
            fileData = claim.Document2FileData;
            fileName = claim.Document2FileName;
            contentType = "application/pdf";
        }
        else return NotFound();

        if (fileData == null || string.IsNullOrEmpty(fileName)) return NotFound();

        return File(fileData, contentType, fileName);
    }



}
