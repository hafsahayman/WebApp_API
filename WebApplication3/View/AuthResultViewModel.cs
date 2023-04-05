namespace WebApplication3.View
{
    public class AuthResultViewModel
    {
        public int UserRefreshTokenId { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }

        public bool IsSuccess { get; set; }
        public string Reason { get; set; }

        public int UserId { get; set; }

        public DateTime ExpiresAt { get; set; }


    }
}
