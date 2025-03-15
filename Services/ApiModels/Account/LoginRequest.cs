using System.ComponentModel.DataAnnotations;

namespace Services.ApiModels.Account
{
    public class LoginRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
