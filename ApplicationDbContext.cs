
using Microsoft.EntityFrameworkCore;
using vue_ts.Models;
using vue_ts.Helpers.Utils;
using vue_ts.Helpers.Utils.GlobalAttributes;


namespace vue_ts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<DetailModel> Customers { get; set; }
        public virtual DbSet<ImageModel> images { get; set; }
        public virtual DbSet<Otp> Otps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Otp entity
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

            // Configure DetailModel
            modelBuilder.Entity<DetailModel>(entity =>
            {
                entity.ToTable("Customers");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id)
                      .HasColumnType("int")
                      .ValueGeneratedOnAdd();
                entity.Property(e => e.email)
                      .HasMaxLength(255)
                      .IsRequired();

            });


            modelBuilder.Entity<ImageModel>(entity =>
            {
                entity.ToTable("Images");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id)
                      .HasColumnType("int")
                      .ValueGeneratedOnAdd();

            });
        }
    }
}
