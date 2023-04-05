using System.ComponentModel.DataAnnotations;

namespace WebApplication3.View
{
    public class TokenRequestViewModel
    {
        //LinkedIn course
        [Required]
        public string Token { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }
}
