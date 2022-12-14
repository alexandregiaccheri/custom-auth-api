using System.ComponentModel.DataAnnotations;

namespace CustomAuthApi.Web.Models
{
#pragma warning disable CS8618
    public class RegisterDTO
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string RepeatPassword { get; set; }

        [Required]
        public string Role { get; set; }
    }
#pragma warning restore CS8618
}
