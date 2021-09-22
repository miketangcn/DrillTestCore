using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DrillTestCore.Models
{
    public partial class DrillContext : DbContext
    {
        public DrillContext()
        {
        }

        public DrillContext(DbContextOptions<DrillContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Holerec> Holerec { get; set; }
        public virtual DbSet<Workrec> Workrec { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                optionsBuilder.UseMySql("server=localhost;database=Drill;user id=root;password=txg3115gwx", x => x.ServerVersion("8.0.20-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Holerec>(entity =>
            {
                entity.ToTable("holerec");

                entity.Property(e => e.Data)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.MaxPressure).HasColumnType("float(24,0)");

                entity.Property(e => e.SerialNo)
                    .IsRequired()
                    .HasColumnType("varchar(30)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            modelBuilder.Entity<Workrec>(entity =>
            {
                entity.ToTable("workrec");

                entity.Property(e => e.SerialNo)
                    .IsRequired()
                    .HasColumnName("SerialNO")
                    .HasColumnType("varchar(30)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
