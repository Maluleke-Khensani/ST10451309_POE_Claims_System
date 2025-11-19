using Microsoft.AspNetCore.Identity;

namespace Claims_System.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }

        public virtual LecturerProfile? LecturerProfile { get; set; }


    }
}
