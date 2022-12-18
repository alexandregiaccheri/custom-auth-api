using System.ComponentModel.DataAnnotations;

namespace CustomAuthApi.Data.DTO
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
