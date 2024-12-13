using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {

        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { ID = 1, Name = "Maths", DisplayOrder = 1 },
                new Category { ID = 2, Name = "Science", DisplayOrder = 2 },
                new Category { ID = 3, Name = "Language", DisplayOrder = 3 }
            );
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    ID = 1,
                    Title = "Fortune of Time",
                    Description = "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum.",
                    ISBN = "SWD999001",
                    Author = "Carlson Ben",
                    ListPrice = 99,
                    Price = 90,
                    Price50 = 85,
                    Price100 = 80,
                    CategoryID = 1,
                    ImageUrl = "",
                },
                new Product
                {
                    ID = 2,
                    Title = "How to avoid war",
                    Description = "etetur sadipscing elitrut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum.",
                    ISBN = "SWD999002",
                    Author = "Benjamin Franklin",
                    ListPrice = 119,
                    Price = 109,
                    Price50 = 100,
                    Price100 = 95,
                    CategoryID = 3,
                    ImageUrl = "",
                },
                new Product
                {
                    ID = 3,
                    Title = "Religious Freedom",
                    Description = "Tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum.",
                    ISBN = "SWD999003",
                    Author = "Iminabo Tombo",
                    ListPrice = 89,
                    Price = 80,
                    Price50 = 75,
                    Price100 = 70,
                    CategoryID = 2,
                    ImageUrl = "",
                },
                new Product
                {
                    ID = 4,
                    Title = "Marital Success",
                    Description = "Dolor sit ametaliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum.",
                    ISBN = "SWD999004",
                    Author = "Kroma Belema",
                    ListPrice = 55,
                    Price = 50,
                    Price50 = 45,
                    Price100 = 40,
                    CategoryID = 1,
                    ImageUrl = "",
                },
                new Product
                {
                    ID = 5,
                    Title = "Financial Success",
                    Description = "At vero eos et accusam et justo duo dolores et ea rebum.",
                    ISBN = "SWD999005",
                    Author = "Biokpo Alabo",
                    ListPrice = 99,
                    Price = 70,
                    Price50 = 65,
                    Price100 = 60,
                    CategoryID = 2,
                    ImageUrl = "",
                },
                new Product
                {
                    ID = 6,
                    Title = "Starts of a Firm",
                    Description = "Lnvidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum.",
                    ISBN = "SWD999006",
                    Author = "Endurance Goodwill",
                    ListPrice = 99,
                    Price = 90,
                    Price50 = 85,
                    Price100 = 80,
                    CategoryID = 3,
                    ImageUrl = "",
                }
            );
        }
    }
}
