using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PassportBooking.Models.Entities;
using System;

namespace PassportBooking.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole,string>
    {
        private IHttpContextAccessor httpContextAccessor;
        public DbSet<GymClass> GymClass { get; set; }
        public DbSet<ApplicationUserGymclass> ApplicationUserGymclass { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUserGymclass>().HasKey(a => new { a.ApplicationUserId, a.GymclassId });
           // modelBuilder.Entity<GymClass>().HasQueryFilter(g=>g.StartTime>DateTime.Now);
        }

    }
}
  
