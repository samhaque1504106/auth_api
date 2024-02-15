using Microsoft.EntityFrameworkCore;
using SignUp_Api.Models;
using System;

namespace SignUp_Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<SignUp> SignUp { get; set; }

       

    }
}
