using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Db.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string VAT { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string County { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public bool IsFirst { get; set; }
        public bool IsEmailed { get; set; }


        public List<Bills> Bills { get; set; }
    }
}
