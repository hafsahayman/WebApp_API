using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
//using System.Data.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApplication3.Context;
using WebApplication3.Model;
using WebApplication3.View;

namespace WebApplication3.Service
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
      

        public UserService(IMapper mapper, DataContext context, IConfiguration configuration)
        {
            _mapper = mapper;
            _context = context;
            _configuration = configuration;
        }

        public async Task<ActionResult<UserViewModel>> CreateAsync(UserViewModel user)
        {
            //var userEntity = _mapper.Map<UserViewModel, User>(user);
            var UserCheck = await _context.User.FirstOrDefaultAsync(c => c.Email == user.Email);

            //if (UserCheck is not null && UserCheck.RefreshTokens is not null)
            if (UserCheck is not null && UserCheck.RefreshTokens is null)
                {
                throw new Exception("User already exists!");

            }
          //  else
           // { 
                await _context.User.AddAsync(_mapper.Map<User>(user));
                await _context.SaveChangesAsync();
                return user;

           // }

            

           

        }

  

        public async Task<User> loginAsync(UserViewModel user)
        {
            var UserCheck = _mapper.Map<UserViewModel, User>(user);

            UserCheck = await _context.User.FirstOrDefaultAsync(c => c.Email == user.Email);


            if (UserCheck is null)
            {
                throw new Exception("User doesnt exist!");


            }
            else if (UserCheck.Password == user.Password)
            {
                return(UserCheck);
            }
            else
            {
                throw new Exception("Invalid Username or Password!");


            }

        }
        public async Task<AuthResultViewModel> GetRefreshTokenAsync(string ipAddress, int UserId,string Email,RefreshToken userRefreshToken )
        {

            var refreshToken = userRefreshToken.RefreshTokenString;
            var accessToken = GenerateToken(UserId,Email);
            if(DateTime.UtcNow> userRefreshToken.ExpirationDate)
            {
                 refreshToken = GenerateRefreshToken();

            }
            return await SaveTokenDetails(ipAddress, UserId, accessToken,refreshToken);
        }

        private async Task<AuthResultViewModel> SaveTokenDetails(string ipAddress, int userId, string tokenString,string refreshToken)
        {
            var userRefreshToken = new RefreshToken
            {
                CreatedDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddMinutes(2),
                Ipaddress = ipAddress,
                IsInvalidated = false,
                RefreshTokenString = refreshToken,
                Token = tokenString,
                UserId = userId
            };

            
        
            await _context.RefreshTokens.AddAsync(userRefreshToken);
            await _context.SaveChangesAsync();
            return new AuthResultViewModel { Token = tokenString, 
                RefreshToken = userRefreshToken.RefreshTokenString, 
                IsSuccess = true,
            ExpiresAt= (DateTime)userRefreshToken.ExpirationDate,
                UserId=userRefreshToken.UserId,
                UserRefreshTokenId=userRefreshToken.UserRefreshTokenId};
        }
        public async Task<AuthResultViewModel> GetTokenAsync(User RequestUser,string ipAddress)
        {
            var user = _context.User.FirstOrDefault(x => x.Email.Equals(RequestUser.Email)
           && x.Password.Equals(RequestUser.Password));
            if (user == null)
                return await Task.FromResult<AuthResultViewModel>(null);
            string tokenString = GenerateToken(user.Id,user.Email);
            string refreshToken = GenerateRefreshToken();
            return await SaveTokenDetails(ipAddress, user.Id, tokenString, refreshToken);
            //return await SaveTokenDetails(ipAddress, user.Id, tokenString);

            //var tokenString = GenerateToken(user.Id,user.Email);
            //string refreshToken = GenerateRefreshToken();
            //var userRefreshToken = new RefreshToken
            //{
            //    CreatedDate = DateTime.UtcNow,
            //    ExpirationDate = DateTime.UtcNow.AddMinutes(5),
            //    Ipaddress = ipAddress,
            //    IsInvalidated = false,
            //    Token = tokenString,
            //    RefreshTokenString=refreshToken,
            //    UserId = user.Id
            //};
            //await _context.RefreshToken.AddAsync(userRefreshToken);
            //await _context.SaveChangesAsync();
            //return new AuthResultViewModel { Token = tokenString,RefreshToken=refreshToken };
        }

        private string GenerateRefreshToken()
        {
            var byteArray = new byte[64];
            using(var cryptoProvider=new RNGCryptoServiceProvider())
            {
                cryptoProvider.GetBytes(byteArray);
                return Convert.ToBase64String(byteArray);
            }
        }

        private string GenerateToken(int UserId,string Email)
        {
            var jwtKey = _configuration.GetValue<string>("JwtSettings:Secret");
            var keyBytes = Encoding.ASCII.GetBytes(jwtKey);
            var tokenHandler = new JwtSecurityTokenHandler();
            var descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    //new Claim(ClaimTypes.Email,user.Email),
                    //new Claim(ClaimTypes.NameIdentifier,user.Id.ToString())
                    new Claim(ClaimTypes.Email,Email),
                    new Claim(ClaimTypes.NameIdentifier,UserId.ToString())

                }),
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(descriptor);
            string tokenString = tokenHandler.WriteToken(token);
            return tokenString;

        }
        public async Task<bool> IsTokenValid(string accessToken, string ipAddress)
        {
            var isValid = _context.RefreshTokens.FirstOrDefault(x => x.Token == accessToken
            && x.Ipaddress == ipAddress) != null;
            return await Task.FromResult(isValid);
        }

        

    }



}
