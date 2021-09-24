using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PassportBooking.Models.Entities
{
    public class ApplicationUserGymclass
    {
        
        public string ApplicationUserId { get; set; }
        public int GymclassId { get; set; }

        public ApplicationUser ApplicationUser;
        public GymClass GymClass;
    }
}
