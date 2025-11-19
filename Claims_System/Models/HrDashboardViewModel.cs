namespace Claims_System.Models
{
    public class HRDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int PendingClaims { get; set; }
        public int ApprovedClaims { get; set; }
        public int RejectedClaims { get; set; }
        public List<LecturerClaim> Claims { get; set; } = new();
    }

}
