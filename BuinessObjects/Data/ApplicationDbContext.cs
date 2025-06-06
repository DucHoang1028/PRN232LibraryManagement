using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace BuinessObjects.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<BookAuthor> BookAuthors { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<BookCategory> BookCategories { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<Fine> Fines { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure composite keys for junction tables
            modelBuilder.Entity<BookAuthor>()
                .HasKey(ba => new { ba.BookId, ba.AuthorId });

            modelBuilder.Entity<BookCategory>()
                .HasKey(bc => new { bc.BookId, bc.CategoryId });

            // Configure relationships
            modelBuilder.Entity<BookAuthor>()
                .HasOne(ba => ba.Book)
                .WithMany(b => b.BookAuthors)
                .HasForeignKey(ba => ba.BookId);

            modelBuilder.Entity<BookAuthor>()
                .HasOne(ba => ba.Author)
                .WithMany(a => a.BookAuthors)
                .HasForeignKey(ba => ba.AuthorId);

            modelBuilder.Entity<BookCategory>()
                .HasOne(bc => bc.Book)
                .WithMany(b => b.BookCategories)
                .HasForeignKey(bc => bc.BookId);

            modelBuilder.Entity<BookCategory>()
                .HasOne(bc => bc.Category)
                .WithMany(c => c.BookCategories)
                .HasForeignKey(bc => bc.CategoryId);

            // Unique constraints
            modelBuilder.Entity<Book>()
                .HasIndex(b => b.Isbn)
                .IsUnique();

            modelBuilder.Entity<Member>()
                .HasIndex(m => m.Email)
                .IsUnique();

            modelBuilder.Entity<Fine>()
                .HasOne(f => f.Loan)
                .WithOne(l => l.Fine)
                .HasForeignKey<Fine>(f => f.LoanId);
        }
    }
}

