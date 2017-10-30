using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Db.Models.AccountViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    
        [Display(Name = "Insert New Pasword")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPswd1 { get; set; }

        [Display(Name = "Confirm New Password")]
        [Compare("NewPswd1", ErrorMessage = "The password and confirmation password do not match.")]
        [DataType(DataType.Password)]
        public string NewPswd2 { get; set; }


        public int? Flag { get; set; }
      
        public string Error { get; set; }


        //[Display(Name = "Remember me?")]
        //public bool RememberMe { get; set; }



    }
}
