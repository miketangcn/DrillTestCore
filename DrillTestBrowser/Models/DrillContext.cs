using System;
using Microsoft.EntityFrameworkCore;
using DrillTestBrowser.Lib;

namespace DrillTestBrowser.Models
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

        public virtual DbSet<Efmigrationshistory> Efmigrationshistory { get; set; }
        public virtual DbSet<Holerec> Holerec { get; set; }
        public virtual DbSet<Workrec> Workrec { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //optionsBuilder.UseMySql("server=rm-bp15xf4wi0dk028js2o.mysql.rds.aliyuncs.com;database=Drill;" +
                //                        "user id=miketangcn;pwd=txg3115gwx;port=3306;sslmode=none",
                //    x => x.ServerVersion("8.0.18-mysql"));
                optionsBuilder.UseMySql(Global.ConnectionStrings,
                    x => x.ServerVersion("8.0.18-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Efmigrationshistory>(entity =>
            {
                entity.HasKey(e => e.MigrationId)
                    .HasName("PRIMARY");

                entity.ToTable("__efmigrationshistory");

                entity.Property(e => e.MigrationId)
                    .HasColumnType("varchar(95)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ProductVersion)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Holerec>(entity =>
            {
                entity.ToTable("holerecs");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Data)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.HoleNumber).HasColumnType("smallint(6)");

                entity.Property(e => e.LayerNo).HasColumnType("smallint(6)");

                entity.Property(e => e.MacId).HasColumnType("smallint(6)");

                entity.Property(e => e.SerialNo)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            modelBuilder.Entity<Workrec>(entity =>
            {
                entity.ToTable("workrecs");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.HoleCount).HasColumnType("smallint(6)");

                entity.Property(e => e.Layer).HasColumnType("smallint(6)");

                entity.Property(e => e.SerialNo)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
