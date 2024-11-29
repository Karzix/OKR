using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OKR.DTO;
using OKR.DTO.Auth;
using OKR.Models;
using OKR.Models.Entity;
using OKR.Repository.Contract;
using OKR.Service.Contract;
using RTools_NTS.Util;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static MayNghien.Infrastructure.CommonMessage.AuthResponseMessage;

namespace OKR.Service.Implementation
{
    public class AuthencationService : IAuthencationService
    {
        private IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private IHttpContextAccessor _httpContextAccessor;
        private IRefreshTokenModelRepository _refreshTokenModelRepository;


        public AuthencationService(IConfiguration config, UserManager<ApplicationUser> userManager,
             RoleManager<IdentityRole> roleManager, IHttpContextAccessor httpContextAccessor, IRefreshTokenModelRepository refreshTokenModelRepository)
        {
            _config = config;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _refreshTokenModelRepository = refreshTokenModelRepository;
        }
        public async Task<AppResponse<LoginResult>> AuthencationUser(LoginRequest login)
        {
            var result = new AppResponse<LoginResult>();
            try
            {
                UserRespone user = null;
                ApplicationUser identityUser = new ApplicationUser();
                var loginResult = new LoginResult();
                //Validate the User Credentials    
                //Demo Purpose, I have Passed HardCoded User Information    

                identityUser = await _userManager.FindByNameAsync(login.Email);
                if (identityUser != null)
                {
                    if (await _userManager.CheckPasswordAsync(identityUser, login.Password))
                    {
                        user = new UserRespone { UserName = identityUser.UserName, Email = identityUser.Email };
                    }

                }
                else if (login.Email == "karzix1809@gmail.com")
                {
                    var newIdentity = new ApplicationUser { UserName = login.Email, Email = login.Email, EmailConfirmed = true };
                    await _userManager.CreateAsync(newIdentity);
                    await _userManager.AddPasswordAsync(newIdentity, "Abc@123");
                    if (!(await _roleManager.RoleExistsAsync("Admin")))
                    {
                        IdentityRole role = new IdentityRole { Name = "Admin" };
                        await _roleManager.CreateAsync(role);
                    }
                    await _userManager.AddToRoleAsync(newIdentity, "Admin");
                }
                if (user != null)
                {
                    var claims = await GetClaims(user, identityUser);
                    var tokenString = GenerateAccessToken(claims);
                    loginResult.AccessToken = tokenString;
                    loginResult.UserName = user.UserName;
                    loginResult.RefreshToken = GenerateRefreshToken(claims);
                    var roles = await _userManager.GetRolesAsync(identityUser);
                    loginResult.Roles = roles.ToList();
                    
                    return result.BuildResult(loginResult);
                }
                else
                {
                    return result.BuildError(ERR_MSG_UserNotFound);
                }
            }
            catch (Exception ex)
            {

                return result.BuildError(ex.ToString());
            }
        }
        private async Task<List<Claim>> GetClaims(UserRespone user, ApplicationUser identityUser)
        {
         
            var claims = new List<Claim>
            {
                new Claim("UserName", user.UserName),

                new Claim("Email", user.Email),
           
            };
            var roles = await _userManager.GetRolesAsync(identityUser);
            foreach (var role in roles)
            {
                claims.Add(new Claim("Roles", role));
            }
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            return claims;

        }
        private string GenerateRefreshToken(IEnumerable<Claim> claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:KeyReferToken"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims: claims,
              expires: DateTime.UtcNow.AddHours(10),
              signingCredentials: credentials);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;
        }
        private ClaimsPrincipal ValidateRefreshToken(string refreshToken)
        {
            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:KeyReferToken"])),
                    ValidateLifetime = false 
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(refreshToken, tokenValidationParameters, out securityToken);
                var jwtSecurityToken = securityToken as JwtSecurityToken;
                if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token");
                return principal;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims: claims,
              expires: DateTime.UtcNow.AddHours(1),
              signingCredentials: credentials);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;
        }
        public AppResponse<LoginResult> Refresh(string RefreshToken)
        {
            var result = new AppResponse<LoginResult>();
            try
            {
                var principal = ValidateRefreshToken(RefreshToken);
                if (principal == null)
                {
                    return result.BuildError("Invalid Refresh Token.");
                }

                // Tạo AccessToken và RefreshToken mới
                var newAccessToken = GenerateAccessToken(principal.Claims);
                var newRefreshToken = GenerateRefreshToken(principal.Claims);

                // Trả về kết quả
                result.Data = new LoginResult
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                };
            }
            catch (Exception ex)
            {
                return result.BuildError(ex.Message+ " " + ex.StackTrace);
            }
            return result;
        }

        public async Task<AppResponse<UserRespone>> GetInforAccount()
        {
            var result = new AppResponse<UserRespone>();
            try
            {
                var user = await _userManager.FindByNameAsync(_httpContextAccessor.HttpContext.User.Identity.Name);
                var roles = await _userManager.GetRolesAsync(user);
                var userDto = new UserRespone
                {
                    UserName = user.UserName,
                    Roles = roles.ToArray(),
                    Email = user.Email,
                    Id = user.Id,
                    DepartmentId = user.DepartmentId,
                };
                Log.Information(user.UserName + " login");
                result.BuildResult(userDto);
            }
            catch (Exception ex)
            {
                return result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
