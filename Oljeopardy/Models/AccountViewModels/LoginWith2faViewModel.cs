using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Oljeopardy.Models.AccountViewModels
{
    public class LoginWith2faViewModel
    {
        [Required]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Autentificeringskode")]
        public string TwoFactorCode { get; set; }

        [Display(Name = "Husk denne maskine")]
        public bool RememberMachine { get; set; }

        [Display(Name = "Forbliv logget ind")]
        public bool RememberMe { get; set; }
    }
}
