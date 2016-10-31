﻿using Launchpad.Services.Interfaces;
using System.Threading.Tasks;
using Launchpad.Models;
using Microsoft.AspNet.Identity;
using Launchpad.Services.IdentityManagers;
using Launchpad.Models.EntityFramework;
using Launchpad.Core;
using System;
using System.Security.Claims;
using System.Collections.Generic;
using AutoMapper;
using System.Linq;

namespace Launchpad.Services
{
    public class UserService : IUserService
    {
        private ApplicationUserManager _userManager;
        private IMapper _mapper;


        public UserService(ApplicationUserManager userManager, IMapper mapper)
        {
            _userManager = userManager.ThrowIfNull(nameof(userManager));
            _mapper = mapper.ThrowIfNull(nameof(mapper));
        }

        public IEnumerable<UserModel> GetUsers()
        {
            return _mapper.Map<IEnumerable<UserModel>>(_userManager.Users.ToList());
        }

        public async Task<bool> IsValidCredential(string userName, string password)
        {
            var user = await _userManager.FindAsync(userName, password);
            return user != null;
        }
        
        public async Task<IdentityResult> RegisterAsync(RegistrationModel model)
        {
            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await _userManager.CreateAsync(user, model.Password);

            return result;
        }

        public async Task<ClaimsIdentity> CreateClaimsIdentityAsync(string userName, string authenticationType)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if(user == null)
            {
                throw new ArgumentException($"Unable to find user {userName}");
            }

            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await _userManager.CreateIdentityAsync(user, authenticationType);

            // Add custom user claims here

            return userIdentity;
        }
    }
}
