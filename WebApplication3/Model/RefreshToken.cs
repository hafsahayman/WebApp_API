using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Model
{
    public class RefreshToken
    {
        [Key]
        public int UserRefreshTokenId { get; set; }
        //public string UserRefreshTokenId { get; set; }

        public string? Token { get; set; }
        public string? RefreshTokenString { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool? IsActive
        {
            get
            {
                return ExpirationDate > DateTime.UtcNow;
            }
        }
        public string? Ipaddress { get; set; }
        public bool? IsInvalidated { get; set; }
        public User? User { get; set; }
        public int UserId { get; set; }
    }
}
