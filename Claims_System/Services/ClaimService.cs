
using Claims_System.Areas.Identity.Data;
using Claims_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Claims_System.Services
{
    //Assited by ChatGPT to implement file encryption and decryption and process the documents
    //As i found it difficult to implement secure file handling
    public class ClaimService : IClaimService
    {
        private const string AesKey = "1234567890ABCDEF";

        private readonly ApplicationDbContext _context;
        public ClaimService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<LecturerClaim>> GetAllClaimsAsync()
        {
            return await _context.LecturerClaims.AsNoTracking().ToListAsync();
        }


        public async Task<LecturerClaim?> GetClaimByIdAsync(int id)
        {
            return await _context.LecturerClaims
                .FirstOrDefaultAsync(c => c.ClaimId == id);
        }


        // Manager
        public async Task<IEnumerable<LecturerClaim>> GetPendingClaimsForManagerAsync()
        {
            return await _context.LecturerClaims
                .OrderByDescending(c => c.Submitted)
                .ToListAsync();
        }

        // Coordinator
        public async Task<IEnumerable<LecturerClaim>> GetPendingClaimsForCoordinatorAsync()
        {
            return await _context.LecturerClaims
                .OrderByDescending(c => c.Submitted)
                .ToListAsync();
        }


        // Manager
        public async Task<bool> UpdateManagerStatusAsync(int claimId, string status)
        {
            var claim = await GetClaimByIdAsync(claimId);
            if (claim == null) return false;

            claim.ManagerStatus = status;

            if (status == "Rejected")
                claim.CoordinatorStatus = "Rejected";

            await _context.SaveChangesAsync();
            return true;
        }

        // Coordinator
        public async Task<bool> UpdateCoordinatorStatusAsync(int claimId, string status)
        {
            var claim = await GetClaimByIdAsync(claimId);
            if (claim == null) return false;

            if (claim.ManagerStatus != "Approved")
                return false;

            claim.CoordinatorStatus = status;
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> CreateClaimAsync(LecturerClaim model, IFormFile? document1, IFormFile? document2)
        {
            try
            {
                // Validate and load documents
                await ProcessDocumentAsync(model, document1, isFirstDoc: true);
                await ProcessDocumentAsync(model, document2, isFirstDoc: false);

                model.CoordinatorStatus = "Pending";
                model.ManagerStatus = "Pending";

                _context.LecturerClaims.Add(model);
                await _context.SaveChangesAsync();
                Console.WriteLine("Saved claim with ID: " + model.ClaimId);

                return true;
            }
            catch (Exception ex)
            {
                // Log exception
                Console.WriteLine("Error saving claim: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }

        public async Task<bool> DeleteClaimAsync(int id)
        {
            var claim = await _context.LecturerClaims.FindAsync(id);
            if (claim == null) return false;

            _context.LecturerClaims.Remove(claim);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateClaimAsync(LecturerClaim model)
        {
            var existingClaim = await _context.LecturerClaims
                .FirstOrDefaultAsync(c => c.ClaimId == model.ClaimId);

            if (existingClaim == null) return false;

            // Update fields
            existingClaim.ModuleName = model.ModuleName;
            existingClaim.HoursWorked = model.HoursWorked;
            existingClaim.Rate = model.Rate;
            existingClaim.Notes = model.Notes;

            
            await ProcessDocumentAsync(existingClaim, null, true);
            await ProcessDocumentAsync(existingClaim, null, false);

            await _context.SaveChangesAsync();
            return true;
        }

        private async Task ProcessDocumentAsync(LecturerClaim model, IFormFile? file, bool isFirstDoc)
        {
            if (file == null || file.Length == 0) return;

            var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx", ".xls" };
            var ext = Path.GetExtension(file.FileName)?.ToLower().Trim() ?? "";

            Console.WriteLine($"Uploading file: {file.FileName} | Extension: {ext} | Size: {file.Length} bytes");

            if (!allowedExtensions.Contains(ext))
                throw new InvalidDataException("Invalid file type");

            if (file.Length > 5 * 1024 * 1024)
                throw new InvalidDataException("File exceeds 5MB limit");

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            // Encrypt file bytes before saving
            var encryptedData = EncryptFile(ms.ToArray(), AesKey);

            if (isFirstDoc)
            {
                model.Document1FileName = file.FileName;
                model.Document1FileData = encryptedData;
            }
            else
            {
                model.Document2FileName = file.FileName;
                model.Document2FileData = encryptedData;
            }
        }

        // AES Encryption helper
        private byte[] EncryptFile(byte[] fileBytes, string key)
        {
            using var aes = System.Security.Cryptography.Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = new byte[16]; // 16-byte IV
            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            return encryptor.TransformFinalBlock(fileBytes, 0, fileBytes.Length);
        }

        private byte[] DecryptFile(byte[] encryptedBytes, string key)
        {
            using var aes = System.Security.Cryptography.Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = new byte[16];
            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            return decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
        }

        public byte[] DecryptFileForPreview(byte[] encryptedBytes)
        {
            return DecryptFile(encryptedBytes, AesKey);
        }


        public FileResult? DownloadDocument1(int claimId)
        {
            var claim = _context.LecturerClaims.Find(claimId);
            if (claim == null || claim.Document1FileData == null) return null;

            var decryptedBytes = DecryptFile(claim.Document1FileData, AesKey);
            return new FileContentResult(decryptedBytes, "application/octet-stream")
            {
                FileDownloadName = claim.Document1FileName
            };
        }

        public FileResult? DownloadDocument2(int claimId)
        {
            var claim = _context.LecturerClaims.Find(claimId);
            if (claim == null || claim.Document2FileData == null) return null;

            var decryptedBytes = DecryptFile(claim.Document2FileData, AesKey);
            return new FileContentResult(decryptedBytes, "application/octet-stream")
            {
                FileDownloadName = claim.Document2FileName
            };
        }

       



       
    }
}
