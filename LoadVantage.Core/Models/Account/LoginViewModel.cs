using System.ComponentModel.DataAnnotations;

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

    }
}
