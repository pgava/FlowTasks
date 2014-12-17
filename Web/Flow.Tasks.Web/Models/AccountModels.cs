using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Flow.Tasks.Web.Models
{
    public class LogOnModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        [Display(Name = "Domains")]
        public IEnumerable<string> Domains { get; set; }
    }
}
