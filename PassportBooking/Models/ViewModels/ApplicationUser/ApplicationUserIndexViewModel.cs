using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PassportBooking.Models.Entities;

namespace PassportBooking.Models.ViewModels.ApplicationUser
{
    public class ApplicationUserIndexViewModel: IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";

        [Display(Name = "Booked Courses")]
        public int BookedCourses { get; set; }
        //public ICollection<PassportBooking.Models.Entities.GymClass> GymClasses { get; set; }
        public ICollection<ApplicationUserGymclass> AttendingClasses { get; set; }
    }
}
