using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using System.Text;
using WebApplication3.Context;
using WebApplication3.Model;
using WebApplication3.Service;
using WebApplication3.View;

namespace WebApplication3.Controller
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        private readonly TokenValidationParameters _tokenValidationParameters;




        public AuthenticationController(IUserService service, DataContext context, IConfiguration configuration, TokenValidationParameters tokenValidationParameters,IMapper mapper)
        {
            _service = service;
            _context = context;
            _configuration = configuration;
            _tokenValidationParameters = tokenValidationParameters;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserViewModel>> CreateAsync(UserViewModel viewModel)
        {
            try
            {
                var res = await _service.CreateAsync(viewModel);
                return Ok(res);



            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


            // return Ok(await _service.CreateAsync(viewModel));
        }

        [HttpPost("login")]

        public async Task<ActionResult> Login(UserViewModel loginViewModel)
        {
            try
            {
                var result = await _service.loginAsync(loginViewModel);

            }
            catch (Exception ex) when(ex.Message=="User doesnt exist!")
            {
                return NotFound(ex.Message);

            }
            catch (Exception ex) when (ex.Message == "Invalid Username or Password!")
            {
                return BadRequest(ex.Message);

            }

            var sucessUser = await _service.loginAsync(loginViewModel);

            //var tokenValue = await GenerateTokenAsync(sucessUser, null);
            //Console.WriteLine(tokenValue);

            ////return Ok(tokenValue);
            //return Unauthorized();

            if (!ModelState.IsValid)
                return BadRequest(new AuthResultViewModel { IsSuccess = false, Reason = "Email and Password must be provided" });
            //changed
            var authenticationResponse = await _service.GetTokenAsync(sucessUser, HttpContext.Connection.RemoteIpAddress.ToString());
            if (authenticationResponse == null)
                return Unauthorized();
            return Ok(authenticationResponse);


        }

        // [HttpPost("RefreshToken")]
        [HttpPost("[action]")]

        public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new AuthResultViewModel { IsSuccess = false, Reason = "Token must be provided" });
            var token = GetJwtToken(request.ExpiredToken);
            string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            var userRefreshToken = _context.RefreshTokens.FirstOrDefault(
                x => x.IsInvalidated == false && x.Token == request.ExpiredToken
                && x.RefreshTokenString == request.RefreshToken
                && x.Ipaddress == ipAddress);

            AuthResultViewModel response = ValidateDetails(token, userRefreshToken);
            if (!response.IsSuccess)
                return BadRequest(response);
            userRefreshToken.IsInvalidated = true;
            _context.RefreshTokens.Update(userRefreshToken);
            await _context.SaveChangesAsync();

            var userEmail = token.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email).Value;
            var authResponse = await _service.GetRefreshTokenAsync(ipAddress, userRefreshToken.UserId,
                userEmail,userRefreshToken);

            return Ok(authResponse);

        }

        private AuthResultViewModel ValidateDetails(JwtSecurityToken token, RefreshToken userRefreshToken)
        {
            if (userRefreshToken == null)
                return new AuthResultViewModel { IsSuccess = false, Reason = "Invalid Token" };
            if (token.ValidTo > DateTime.UtcNow)
                return new AuthResultViewModel { IsSuccess = false, Reason = "Token not expired",Token=userRefreshToken.Token, RefreshToken=userRefreshToken.RefreshTokenString,
                UserRefreshTokenId=userRefreshToken.UserRefreshTokenId,UserId=userRefreshToken.UserId,ExpiresAt= (DateTime)userRefreshToken.ExpirationDate};
            if ((bool)!userRefreshToken.IsActive)
                return new AuthResultViewModel { IsSuccess = false, Reason = "Refresh Token Expired" };
            //if token has expired
            return new AuthResultViewModel { IsSuccess = true, ExpiresAt= (DateTime)userRefreshToken.ExpirationDate };





        }

        private JwtSecurityToken GetJwtToken(string expiredToken)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.ReadJwtToken(expiredToken);
        }













        //private async Task<AuthResultViewModel> GenerateTokenAsync(User user, RefresherToken rToken)
        //{
        //    var authClaims = new List<Claim>()
        //    {

        //        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        //        new Claim(JwtRegisteredClaimNames.Email, user.Email),

        //        new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
        //    };


        //    var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration.GetValue<string>("JwtSettings:Secret")));

        //    var token = new JwtSecurityToken(
        //        expires: DateTime.UtcNow.AddMinutes(1),
        //        claims: authClaims,
        //        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)

        //        );

        //    var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

        //    if (rToken != null)
        //    {
        //        var rTokenResponse = new AuthResultViewModel()
        //        {
        //            Token = jwtToken,
        //            RefreshToken = rToken.Token,
        //            ExpiresAt = token.ValidTo
        //        };
        //        return rTokenResponse;
        //    }

        //    var refreshToken = new RefresherToken()
        //    {
        //        JwtId = token.Id,
        //        IsRevoked = false,
        //        UserId = user.Id,
        //        DateAdded = DateTime.UtcNow,
        //        DateExpire = DateTime.UtcNow.AddMonths(6),
        //        Token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString()
        //    };

        //    await _context.RefresherTokens.AddAsync(refreshToken);
        //    await _context.SaveChangesAsync();

        //    var response = new AuthResultViewModel()
        //    {
        //        Token = jwtToken,
        //        RefreshToken = refreshToken.Token,
        //        ExpiresAt = token.ValidTo
        //    };
        //    return response;
        //}


        //[HttpPost("RefreshToken")]
        //public async Task<IActionResult> RefreshToken([FromBody] TokenRequestViewModel tokenRequestViewModel)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest("Please provide all required fields");
        //    }
        //    var result = await VerifyAndGenerateTokenAsync(tokenRequestViewModel);
        //    return Ok(result);
        //}

        //private async Task<AuthResultViewModel> VerifyAndGenerateTokenAsync(TokenRequestViewModel tokenRequestViewModel)
        //{
        //    var jwtTokenHandler = new JwtSecurityTokenHandler();
        //    var storedToken = await _context.RefresherTokens.FirstOrDefaultAsync(x => x.Token == tokenRequestViewModel.RefreshToken);
        //    var dbUser = await _context.User.FirstOrDefaultAsync(x => x.Id == storedToken.UserId);

        //    try
        //    {
        //        var tokenCheckResult = jwtTokenHandler.ValidateToken(tokenRequestViewModel.Token, _tokenValidationParameters, out var validatedToken);

        //        return await GenerateTokenAsync(dbUser, storedToken);
        //    }
        //    catch (SecurityTokenExpiredException)
        //    {
        //        if (storedToken.ExpirationDate >= DateTime.UtcNow)
        //        {
        //            return await GenerateTokenAsync(dbUser, storedToken);
        //        }
        //        else
        //        {
        //            return await GenerateTokenAsync(dbUser, null);
        //        }
        //    }
        //}









    }
}
