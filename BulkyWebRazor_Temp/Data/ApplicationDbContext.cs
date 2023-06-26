﻿using BulkyWebRazor_Temp.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyWebRazor_Temp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
                 new Category { Id = 2, Name = "Comedy", DisplayOrder = 2 },
                  new Category { Id = 3, Name = "Seris", DisplayOrder = 3 },
                   new Category { Id = 4, Name = "Sci-Fi", DisplayOrder = 4 }
                );
        }
    }
}
