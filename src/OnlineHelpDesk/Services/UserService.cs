using OnlineHelpDesk.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Windows;

namespace OnlineHelpDesk.Services
{
    public class UserService : IDisposable
    {
        private ApplicationDbContext context;
        private ApplicationDbContext DbContext
        {
            get => context ?? ApplicationDbContext.Create();
            set => context = value;
        }
        //public UserService(ApplicationDbContext db) => this.db = db;

        public async Task<bool> CreateUser(ApplicationUser user, string role = "Student", string password = null)
        {
            return await Task.Run(() =>
            {
                user.MustChangePassword = true;
                user.CreatedAt = DateTime.UtcNow;
                user.Avatar = user.Avatar ?? AppInfo.DefaultProfilePicture;
                var result = UserManager.Create(user, password ?? AppInfo.DefaultUserPassword);
                if (result.Succeeded)
                {
                    result = UserManager.AddToRole(user.Id, role);

                    if (role.Equals("FacilityHead", StringComparison.OrdinalIgnoreCase))
                    {
                        DbContext.FacilityHeads.Add(new FacilityHead
                        {
                            UserId = user.Id,
                            PositionName = "New Head",
                        });
                        DbContext.SaveChangesAsync();
                    }
                }
                return result.Succeeded;
            });
        }

        public async Task<bool> CreateUser(ProfileViewModel user)
        {
            var result = CreateUser(new ApplicationUser
            {
                UserName = user.UserIdentity ?? "unknown",
                FullName = user.FullName ?? "unknown",
                Avatar = user.ProfilePicture ?? AppInfo.DefaultProfilePicture,
                Email = user.Email ?? "unknown",
                Contact = user.Contact ?? ""
            }, role: user.Role);
            return await result;
        }

        #region Helpers
        private UserManager<ApplicationUser> _userManager;
        public UserManager<ApplicationUser> UserManager
        {
            get => _userManager ?? new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(DbContext));
            private set => _userManager = value;
        }

        private RoleManager<IdentityRole> _roleManager;
        public RoleManager<IdentityRole> RoleManager
        {
            get => _roleManager ?? new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(DbContext));
            private set => _roleManager = value;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _userManager?.Dispose();
                _userManager = null;

                _roleManager?.Dispose();
                _roleManager = null;

                // coalescing statement in C# 8.0 :))
                //db ??= null;
                DbContext?.Dispose();
                DbContext = null;
            }
        }
        #endregion
    }
}