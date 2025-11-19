using System.Collections.Generic;

namespace Claims_System.Models
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        public string FullName { get; set; }
        public string Email { get; set; }

        public string CurrentRole { get; set; }
        public string NewRole { get; set; }

        public decimal HourlyRate { get; set; }

        public List<string> AllRoles { get; set; }
    }
}
