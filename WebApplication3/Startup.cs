using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApplication3.Context;
using WebApplication3.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using WebApplication3.Configuration;

using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;

namespace WebApplication3
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            var allowedOrigins = _configuration.GetSection("AllowedOrigins").Get<string[]>();
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(_configuration.GetConnectionString("SqlServer"));
            });
         

            services.AddCors(options =>
            {
                options.AddPolicy("App_Cors_Policy", builder =>
                {
                    builder
                    .WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });

            services.AddAutoMapper(config => { config.AddProfile<TitleAutoMapper>(); });
            services.AddScoped<ITitleService, TitleService>();
            services.AddScoped<IRecipeService, RecipeService>();
            services.AddScoped<IIngredientService, IngredientService>();
            services.AddScoped<IUserService, UserService>();



            services.AddControllers();
            var jwtKey = _configuration.GetValue<string>("JwtSettings:Secret");
            var keyBytes = Encoding.ASCII.GetBytes(jwtKey);
            var tokenValidationParameters = new TokenValidationParameters()
            {
              
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                ValidateLifetime = true,
                ValidateAudience = false,
                //ValidAudience = _configuration["JWT:Audience"],
                ValidateIssuer = false,
                //ValidIssuer = _configuration["JWT:Issuer"],
                ClockSkew = TimeSpan.Zero

            };
            services.AddSingleton(tokenValidationParameters);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                //options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                //Add JWT Bearer
                .AddJwtBearer(options =>
                {
                    //options.SaveToken = true;
                   // options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = tokenValidationParameters;
                    options.Events = new JwtBearerEvents();
                    options.Events.OnTokenValidated = async (context) =>
                    {
                        var ipAddress = context.Request.HttpContext.Connection.RemoteIpAddress.ToString();
                        var jwtService = context.Request.HttpContext.RequestServices.GetService<IUserService>();
                        var jwtToken = context.SecurityToken as JwtSecurityToken;
                      if (!await jwtService.IsTokenValid(jwtToken.RawData, ipAddress))
                            context.Fail("Invalid Token Details");


                    };


                });

        }



        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                // app.UseHsts();
            }
            app.UseCors("App_Cors_Policy");
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //app.MapRazorPages();
            //          app.Run();
        }
    }
}
