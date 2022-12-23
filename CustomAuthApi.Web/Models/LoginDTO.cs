using System.ComponentModel.DataAnnotations;

namespace CustomAuthApi.Web.Models
{
#pragma warning disable CS8618
    public class LoginDTO
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
#pragma warning restore CS8618
}
