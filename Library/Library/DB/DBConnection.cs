using Library.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.DB
{
    class DBConnection : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Author> Author { get; set; }

        public DbSet<Category> Category { get; set; }

        public DbSet<Book> Book { get; set; }

        public DbSet<Loan> Loan { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=localhost\\SQLEXPRESS;Database=LibraryDB;Trusted_Connection=True;TrustServerCertificate=True"
            );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>(entity =>
            {
                entity.ToTable("Book");
                entity.HasKey(x => x.IdBook);
                entity.Property(x => x.IdBook).HasColumnName("id_book");
                entity.Property(x => x.IdAuthor).HasColumnName("id_author");
                entity.Property(x => x.IdCategory).HasColumnName("id_category");
                entity.Property(x => x.Year).HasColumnName("year");
                entity.Property(x => x.ISBN).HasColumnName("isbn");
                entity.Property(x => x.TotalCopies).HasColumnName("total_copies");
                entity.Property(x => x.AvailableCopies).HasColumnName("available_copies");

                entity.HasOne(d => d.Author)
                      .WithMany(p => p.Books)
                      .HasForeignKey(d => d.IdAuthor);

                entity.HasOne(d => d.Category)
                      .WithMany(p => p.Books)
                      .HasForeignKey(d => d.IdCategory);
            });

            modelBuilder.Entity<Loan>(entity =>
            {
                entity.ToTable("Loan");
                entity.HasKey(x => x.IdLoan);
                entity.Property(x => x.IdLoan).HasColumnName("id_loan");
                entity.Property(x => x.IdBook).HasColumnName("id_book");
                entity.Property(x => x.IdUser).HasColumnName("id_user");
                entity.Property(x => x.LoanDate).HasColumnName("loan_date");
                entity.Property(x => x.DueDate).HasColumnName("due_date");
                entity.Property(x => x.ReturnDate).HasColumnName("return_date");

                entity.Property(x => x.Fine)
                    .HasColumnName("fine")
                    .HasColumnType("decimal(6,2)")
                    .HasDefaultValue(0.00m);

                entity.HasOne(d => d.Book)
                    .WithMany(p => p.Loans)
                    .HasForeignKey(d => d.IdBook)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Loans)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Author>(entity =>
            {
                entity.ToTable("Author");
                entity.HasKey(x => x.IdAuthor);
                entity.Property(x => x.IdAuthor).HasColumnName("id_author");
                entity.Property(x => x.FirstName).HasColumnName("first_name");
                entity.Property(x => x.LastName).HasColumnName("last_name");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");
                entity.HasKey(x => x.IdCategory);
                entity.Property(x => x.IdCategory).HasColumnName("id_category");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("user_role"); 
                entity.HasKey(x => x.RoleName);
                entity.Property(x => x.RoleName).HasColumnName("role_name");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(x => x.IdUser);
                entity.Property(x => x.IdUser).HasColumnName("id_user");
                entity.Property(x => x.FirstName).HasColumnName("first_name");
                entity.Property(x => x.LastName).HasColumnName("last_name");

                entity.HasOne(x => x.UserRole)
                    .WithMany(x => x.Users)
                    .HasForeignKey(x => x.Role);
            });

            
        }
    }
}