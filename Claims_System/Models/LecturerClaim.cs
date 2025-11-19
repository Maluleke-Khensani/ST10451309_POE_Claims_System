using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Claims_System.Models
{
    public class LecturerClaim
    {
        [Key]
        public int ClaimId { get; set; }


        [Required]
        public string Username { get; set; }
        public string UserId { get; set; }   


        [Required]
        public string FullName { get; set; }

        [Required]
        public string ModuleName { get; set; }

        [Required]
        [Range(1, 12)]
        public int Month { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Submitted { get; set; }

        [Required]
        [Range(0, 999)]
        public decimal HoursWorked { get; set; }

        [Required]
        public decimal Rate { get; set; }

        [NotMapped]
        public decimal Total => HoursWorked * Rate;

        public string CoordinatorStatus { get; set; } = "Pending";

        public string ManagerStatus { get; set; } = "Pending";

        public string Notes { get; set; }

        // Document 1
        public string? Document1FileName { get; set; }
        public byte[]? Document1FileData { get; set; }

        // Document 2
        public string? Document2FileName { get; set; }
        public byte[]? Document2FileData { get; set; }
    }
}
