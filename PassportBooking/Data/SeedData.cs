using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PassportBooking.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PassportBooking.Data
{
    public class SeedData
    {
        private static ApplicationDbContext db;
        private static RoleManager<IdentityRole> roleManeger;
        private static UserManager<ApplicationUser> userManeger;

        public static async Task IntAsync(ApplicationDbContext context, IServiceProvider services, string adminPW)
        {
            if (string.IsNullOrWhiteSpace(adminPW)) throw new Exception("Can't get password from config");
            if (context is null) throw new NullReferenceException(nameof(ApplicationDbContext));

            db = context;
            if (db.GymClass.Any()) return;

             roleManeger=services.GetRequiredService<RoleManager<IdentityRole>>();
            if (roleManeger is null) throw new NullReferenceException(nameof(RoleManager<IdentityRole>));

            userManeger = services.GetRequiredService<UserManager<ApplicationUser>>();
            if (userManeger is null) throw new NullReferenceException(nameof(UserManager<ApplicationUser>));

            var roleNmaes = new[] { "Student","Admin"};
            var adminEmail = "admin@gymbokning.se";

            var gymClasses = GetGymClasses();
             await db.AddRangeAsync(gymClasses);
            
            await AddRolesAsync(roleNmaes);

            var admin = await AddAdminAsync(adminEmail,adminPW);
            await AddToRolesAsync(admin,roleNmaes);

            await db.SaveChangesAsync();
        }

        private static async Task AddToRolesAsync(ApplicationUser admin, string[] roleNmaes)
        {
            if (admin is null) throw new NullReferenceException(nameof(admin));

            foreach (var role in roleNmaes)
            {
                if (await userManeger.IsInRoleAsync(admin, role)) continue;

                var result = await userManeger.AddToRoleAsync(admin,role);

                if (!result.Succeeded) throw new Exception(string.Join("/n", result.Errors));
            }
        }

        private static IEnumerable<GymClass> GetGymClasses()
        {
            var faker = new Faker("sv");
            var gymClasses = new List<GymClass>();

            for (int i = 0; i < 20; i++)
            {
                var temp = new GymClass
                {
                    Name = faker.Company.CatchPhrase(),
                    Description = faker.Hacker.Verb(),
                    Duration = new TimeSpan(0, 55, 0),
                    StartTime = DateTime.Now.AddDays(faker.Random.Int(-5,5))
                };
                gymClasses.Add(temp);
            }
            return gymClasses;
        }

        private static async Task<ApplicationUser> AddAdminAsync(string adminEmail, string adminPW)
        {
            var found = await userManeger.FindByEmailAsync(adminEmail);
            if (found != null) return null;

            var admin = new ApplicationUser
            {
                UserName=adminEmail,
                Email= adminEmail
            };

            var result = await userManeger.CreateAsync(admin,adminPW);

            if (!result.Succeeded) throw new Exception(string.Join("/n", result.Errors));

            return admin;
        }

        private static async Task AddRolesAsync(string[] roleNmaes)
        {
            foreach (var roleNmae in roleNmaes)
            {
                if (await roleManeger.RoleExistsAsync(roleNmae)) continue;
                var role = new IdentityRole {Name=roleNmae };
                var result = await roleManeger.CreateAsync(role);

                if (!result.Succeeded) throw new Exception(string.Join("/n",result.Errors));
            }
            
        }
    }
}
