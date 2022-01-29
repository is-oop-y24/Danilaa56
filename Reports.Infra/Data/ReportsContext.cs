﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Reports.Core.Entities;

namespace Reports.Infra.Data
{
    public class ReportsContext : DbContext
    {
        public ReportsContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Person> Persons { get; set; } = null!;
        public DbSet<Problem> Problems { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Problem>().Property(problem => problem.State)
                .HasConversion(new EnumToStringConverter<Problem.ProblemState>());
            
            base.OnModelCreating(modelBuilder);
        }
    }
}