using Claims_System.Models;
using System.Collections.Generic;

namespace Claims_System.Models
{
    public class LecturerProfileViewModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public decimal HourlyRate { get; set; }

        public List<LecturerClaim> Claims { get; set; } = new List<LecturerClaim>();
    }
}
