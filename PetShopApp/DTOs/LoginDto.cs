using System.ComponentModel.DataAnnotations;

namespace PetShopApp.DTOs
{
    public class LoginDto
    {
        [Required]
        [MaxLength(100)]
        public string UsernameOrEmail { get; set; }
        [Required]
        [MaxLength(100)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool IsRemember { get; set; }

    }
}
