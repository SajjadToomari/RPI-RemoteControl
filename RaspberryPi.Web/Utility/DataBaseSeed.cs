using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;


namespace RaspberryPi.Web.Utility;

public class DataBaseSeed
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetService<ApplicationDbContext>();

        await context.Database.EnsureCreatedAsync();

        //string[] roles = new string[] { "Owner", "Administrator", "Manager", "Editor", "Buyer", "Business", "Seller", "Subscriber" };
        string[] roles = new string[] { "Admin" };

        foreach (string role in roles)
        {
            var roleStore = new RoleStore<IdentityRole>(context);

            if (!context.Roles.Any(r => r.Name == role))
            {
                context.Roles.Add(new IdentityRole()
                {
                    Name = role,
                    NormalizedName = role.ToUpper()
                });
                await context.SaveChangesAsync();
            }
        }


        var user = new IdentityUser
        {
            Email = "admin@toomari.ir",
            NormalizedEmail = "ADMIN@TOOMARI.IR",
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            PhoneNumber = "09374647452",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString("D")
        };


        if (!context.Users.Any(u => u.UserName == user.UserName))
        {
            var password = new PasswordHasher<IdentityUser>();
            var hashed = password.HashPassword(user, "123456789");
            user.PasswordHash = hashed;

            var userStore = new UserStore<IdentityUser>(context);
            var result = await userStore.CreateAsync(user);

        }

        await AssignRoles(scope, user.Email, roles);

        await context.SaveChangesAsync();
    }

    public static async Task<IdentityResult> AssignRoles(IServiceScope scope, string email, string[] roles)
    {
        UserManager<IdentityUser> _userManager = scope.ServiceProvider.GetService<UserManager<IdentityUser>>();
        IdentityUser user = await _userManager.FindByEmailAsync(email);
        var result = await _userManager.AddToRolesAsync(user, roles);

        return result;
    }

}
