using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreAngular.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreAngular.Data
{
    public class ApplicationDbContext: IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>().HasData(
                new {Id="1" , Name="Admin" , NormaLiazedName = "ADMIN"},
                new { Id = "2", Name = "Customer", NormaLiazedName = "CUSTOMER" },
                new { Id = "3", Name = "Moderator", NormaLiazedName = "MODERATOR" }
                );
        }

        public DbSet<ProductModel> Products { get; set; }
    }
}
