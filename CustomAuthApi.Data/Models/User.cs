using System.ComponentModel.DataAnnotations;

namespace CustomAuthApi.Data.Models
{
#pragma warning disable CS8618
    public class User
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public byte[] Password { get; set; }

        [Required]
        public byte[] PasswordSalt { get; set; }

        [Required]
        public string Role { get; set; }

        public User()
        {
            Id = Guid.NewGuid();
        }
    }
#pragma warning restore CS8618
}
