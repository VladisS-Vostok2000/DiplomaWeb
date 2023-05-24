using DiplomaWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace DiplomaWeb.Db {
    public class ApplicationContext : DbContext {
        public DbSet<User> Users { get; set; }
        public DbSet<Note> Notes { get; set; }



        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) {
            Database.EnsureCreated();
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<User>().Property(p => p.Login).IsRequired();
            modelBuilder.Entity<User>().Property(p => p.Password).IsRequired();
            modelBuilder.Entity<Note>().Property(p => p.UserName).IsRequired();
            modelBuilder.Entity<Note>().Property(p => p.Name).IsRequired();
            modelBuilder.Entity<Note>().Property(p => p.BranchName).IsRequired();
            modelBuilder.Entity<Note>().Property(p => p.Version).IsRequired();
            modelBuilder.Entity<Note>().Property(p => p.Text).IsRequired();
            modelBuilder.Entity<Note>().Property(p => p.CreatingDate).IsRequired();
            base.OnModelCreating(modelBuilder);
        }
    }
}
