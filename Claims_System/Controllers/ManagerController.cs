using Claims_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Manager,HR")]
public class ManagerController : Controller
{
    private readonly IClaimService _service;

    public ManagerController(IClaimService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index()
    {
        var claims = await _service.GetPendingClaimsForManagerAsync();
        return View(claims);
    }

    public async Task<IActionResult> ReviewClaim(int claimId)
    {
        var claim = await _service.GetClaimByIdAsync(claimId);

        if (claim == null) return NotFound();

        return View(claim);
    }


    [HttpPost]
    public async Task<IActionResult> Approve(int claimId)
    {
        await _service.UpdateManagerStatusAsync(claimId, "Approved");
        TempData["Message"] = "Claim approved successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Reject(int claimId)
    {
        await _service.UpdateManagerStatusAsync(claimId, "Rejected");
        TempData["Message"] = "Claim rejected successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> DownloadDocument1(int claimId)
    {
        var claim = await _service.GetClaimByIdAsync(claimId);
        if (claim == null || claim.Document1FileData == null || string.IsNullOrEmpty(claim.Document1FileName))
            return NotFound();

        return File(claim.Document1FileData, "application/pdf", claim.Document1FileName);
    }

    [HttpGet]
    public async Task<IActionResult> DownloadDocument2(int claimId)
    {
        var claim = await _service.GetClaimByIdAsync(claimId);
        if (claim == null || claim.Document2FileData == null || string.IsNullOrEmpty(claim.Document2FileName))
            return NotFound();

        return File(claim.Document2FileData, "application/pdf", claim.Document2FileName);
    }


    [HttpGet]
    public IActionResult PreviewDocument(int claimId, int docNumber)
    {
        var claim = _service.GetClaimByIdAsync(claimId).Result;
        if (claim == null) return NotFound();

        byte[]? fileData = null;
        string? fileName = null;

        if (docNumber == 1)
        {
            fileData = claim.Document1FileData;
            fileName = claim.Document1FileName;
        }
        else if (docNumber == 2)
        {
            fileData = claim.Document2FileData;
            fileName = claim.Document2FileName;
        }

        if (fileData == null || string.IsNullOrEmpty(fileName))
            return NotFound();

        var decryptedData = _service.DecryptFileForPreview(fileData);
        var contentType = GetContentType(fileName);

        // Return file inline so browser shows it in <iframe>
        return File(decryptedData, contentType);
    }

    private string GetContentType(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return ext switch
        {
            ".pdf" => "application/pdf",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            _ => "application/octet-stream"
        };
    }

}
