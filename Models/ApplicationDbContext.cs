using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ProductTask.Models
{
  
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

       

        public DbSet<Category> Categoriess { get; set; }
        public DbSet<Product> Productss { get; set; }

       
    }

}