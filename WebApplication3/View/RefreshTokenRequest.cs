using System.ComponentModel.DataAnnotations;

namespace WebApplication3.View
{
    public class RefreshTokenRequest
    {
        [Required]
        public string ExpiredToken { get; set; }
        [Required]
        public string RefreshToken { get; set; }
        
    }
}
