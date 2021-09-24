using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using PassportBooking.Models.Entities;

namespace PassportBooking.Models.ViewModels.GymClass
{
    public class GymClassIndexViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime EndTime { get { return StartTime + Duration; } }
        public string Description { get; set; }

        [Display(Name = "Attending Members")]
        public int AttendingMembersCount { get; set; }

        public ICollection<PassportBooking.Models.Entities.ApplicationUser> Members { get; set; }
        public ICollection<ApplicationUserGymclass> AttendingMembers { get; set; }
    }
}
