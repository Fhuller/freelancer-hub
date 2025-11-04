using freelancer_hub_backend.Models;
using Microsoft.EntityFrameworkCore;
using File = freelancer_hub_backend.Models.File;


namespace freelancer_hub_backend
{
    public class FreelancerContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<ProjectFile> ProjectFiles { get; set; }

        public FreelancerContext(DbContextOptions<FreelancerContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Expense>()
                .Property(e => e.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Invoice>()
                .Property(i => i.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<File>()
                .Property(p => p.FileSize)
                .HasPrecision(18, 2);

            base.OnModelCreating(modelBuilder);
        }
    }
}