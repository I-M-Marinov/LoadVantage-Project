using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadVantage.Core.Models.Account
{
    public class LoginViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;


        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; } = null!;


        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
