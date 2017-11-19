using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Oljeopardy.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [UserName]
        [Display(Name = "Brugernavn")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Adgangskoden skal indeholde mindst 4 og højst 100 tegn.", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "Adgangskode")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekræft adgangskode")]
        [Compare("Password", ErrorMessage = "Adgangskoden og bekræftelsen passer ikke sammen.")]
        public string ConfirmPassword { get; set; }
    }

    public class UserNameAttribute : Attribute
    {
    }
}
