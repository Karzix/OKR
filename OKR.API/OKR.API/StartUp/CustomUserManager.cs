using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using OKR.Models.Entity;

namespace OKR.API.StartUp
{
    public class CustomUserManager : UserManager<ApplicationUser>
    {
        public CustomUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor,
                                 IPasswordHasher<ApplicationUser> passwordHasher,
                                 IEnumerable<IUserValidator<ApplicationUser>> userValidators,
                                 IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
                                 ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors,
                                 IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators,
                   keyNormalizer, errors, services, logger)
        { }

        public override async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            if (user.IsLocked)
            {
                throw new Exception("User is locked");
            }

            return await base.CheckPasswordAsync(user, password);
        }
    }
}
