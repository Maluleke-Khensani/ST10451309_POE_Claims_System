using Claims_System.Models;
using Microsoft.AspNetCore.Mvc;

namespace Claims_System.Services
{
    public interface IClaimService
    {
        // Lecturer
        Task<IEnumerable<LecturerClaim>> GetAllClaimsAsync();
        Task<LecturerClaim?> GetClaimByIdAsync(int id);
        Task<bool> CreateClaimAsync(LecturerClaim model, IFormFile? document1, IFormFile? document2);
        Task<bool> DeleteClaimAsync(int id);
        Task<bool> UpdateClaimAsync(LecturerClaim model);



        // Coordinator
        Task<IEnumerable<LecturerClaim>> GetPendingClaimsForCoordinatorAsync();
        Task<bool> UpdateCoordinatorStatusAsync(int claimId, string status);

        // Manager
        Task<IEnumerable<LecturerClaim>> GetPendingClaimsForManagerAsync();
        Task<bool> UpdateManagerStatusAsync(int claimId, string status);

        byte[] DecryptFileForPreview(byte[] encryptedBytes);
        FileResult? DownloadDocument1(int claimId);
        FileResult? DownloadDocument2(int claimId);


    }

}
