using Microsoft.EntityFrameworkCore;
using vue_ts.Models;

namespace vue_ts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<DetailModel> Customers { get; set; }
        public virtual DbSet<ImageModel> images { get; set; } // Changed to PascalCase
        public virtual DbSet<Otp> Otps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure DetailModel (Customer)
            modelBuilder.Entity<DetailModel>(entity =>
            {
                entity.ToTable("Customers"); // Match your table name attribute
                entity.HasKey(e => e.id);

                entity.Property(e => e.id)
                    .HasColumnType("int")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.email)
                    .HasMaxLength(255)
                    .IsRequired();

                // Navigation property configuration
                entity.HasOne(d => d.images) 
                    .WithOne(i => i.customer)
                    .HasForeignKey<ImageModel>(i => i.id)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure ImageModel
            modelBuilder.Entity<ImageModel>(entity =>
            {
                entity.ToTable("Images"); 

                // Primary key configuration
                entity.HasKey(e => e.customerid);
                entity.Property(e => e.customerid)
                    .ValueGeneratedOnAdd();

                // Foreign key configuration
                entity.Property(e => e.id)
                    .HasColumnType("int")
                    .IsRequired();

                // Navigation property configuration
                entity.HasOne(i => i.customer)
                    .WithOne(c => c.images) 
                    .HasForeignKey<ImageModel>(i => i.id);
            });

            // Configure Otp entity (unchanged)
            modelBuilder.Entity<Otp>(entity =>
            {
                entity.ToTable("Otps");
                entity.HasKey(o => o.Id);
                entity.Property(o => o.Id)
                    .HasColumnType("int")
                    .ValueGeneratedOnAdd();
                entity.Property(o => o.PhoneNumber)
                    .HasMaxLength(15)
                    .IsRequired();
                entity.Property(o => o.OtpCode)
                    .HasMaxLength(6)
                    .IsRequired();
                entity.Property(o => o.CreatedAt)
                    .IsRequired()
                    .HasColumnType("datetime");
                entity.Property(o => o.ExpiresAt)
                    .IsRequired()
                    .HasColumnType("datetime");
                entity.Property(o => o.IsUsed)
                    .IsRequired()
                    .HasDefaultValue(false);
            });
        }
    }
}