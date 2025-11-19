using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Claims_System.Models
{
    public class LecturerProfile
    {
        [Key]
        public string UserId { get; set; }  


        public string FullName { get; set; }    

        [Required]
        public string Email { get; set; }       

        [Required]
        public decimal HourlyRate { get; set; } // only lecturers need this

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }


    }
}
