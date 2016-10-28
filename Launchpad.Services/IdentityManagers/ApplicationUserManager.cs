﻿using Launchpad.Models.EntityFramework;
using Microsoft.AspNet.Identity;

namespace Launchpad.Services.IdentityManagers
{
    public class ApplicationUserManager : UserManager<ApplicationUser, string>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser,string> store) : base(store)
        {
        }

        public ApplicationUserManager(IUserStore<ApplicationUser,string> store, IUserTokenProvider<ApplicationUser, string> tokenProvider) : base(store)
        {
            this.UserValidator = new UserValidator<ApplicationUser>(this)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            this.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            this.UserTokenProvider = tokenProvider;
        }

      
    }
}