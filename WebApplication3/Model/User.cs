namespace WebApplication3.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<RefreshToken>? RefreshTokens { get; set; }
        //public List<RefresherToken>? RefresherTokens { get; set; }



    }
}
