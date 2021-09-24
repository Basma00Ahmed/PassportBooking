using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PassportBooking.Models.Entities
{
    public class GymClass
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }

        [Required]
        [RegularExpression(@"^(?:[01][0-9]|2[0-3]):[0-0][0-0]:[0-0][0-0]$", ErrorMessage = "Invalid time format and hh:mm:ss values.")]
        public TimeSpan Duration { get; set; }
        [Display(Name = "End Time")]
        public DateTime EndTime { get { return StartTime + Duration; } }
        public string Description { get; set; }

        //  public ICollection<ApplicationUser> ApplicationUsers { get; set; }
        [Display(Name = "Attending Members")]
        public ICollection<ApplicationUserGymclass> AttendingMembers { get; set; }

    }
}
