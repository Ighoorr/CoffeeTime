using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace WebApiCoffee.Models
{
    public partial class CoffeeContext : DbContext
    {
        public CoffeeContext()
        {
        }

        public CoffeeContext(DbContextOptions<CoffeeContext> options)
            : base(options)
        {
        }

        public virtual DbSet<FreeTime> FreeTimes { get; set; }
        public virtual DbSet<Hobby> Hobbies { get; set; }
        public virtual DbSet<Photo> Photos { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-HTNFDBR;Database=Coffee;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Ukrainian_CI_AS");

            modelBuilder.Entity<FreeTime>(entity =>
            {
                entity.HasKey(e => e.TimeId);

                entity.ToTable("FreeTime");

                entity.Property(e => e.TimeId).HasColumnName("timeId");

                entity.Property(e => e.Day)
                    .HasMaxLength(100)
                    .HasColumnName("day");

                entity.Property(e => e.Time)
                    .HasColumnType("datetime")
                    .HasColumnName("time");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.FreeTimes)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_FreeTime_User");
            });

            modelBuilder.Entity<Hobby>(entity =>
            {
                entity.ToTable("Hobby");

                entity.Property(e => e.HobbyId).HasColumnName("hobbyId");

                entity.Property(e => e.Desc)
                    .HasColumnType("text")
                    .HasColumnName("desc");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Hobbies)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Hobby_User");
               
            });

            modelBuilder.Entity<Photo>(entity =>
            {
                entity.ToTable("Photo");

                entity.Property(e => e.PhotoId).HasColumnName("photoId");

                entity.Property(e => e.PublicId)
                    .HasMaxLength(100)
                    .HasColumnName("publicId");

                entity.Property(e => e.Url)
                    .HasColumnType("text")
                    .HasColumnName("url");

                entity.Property(e => e.UserId).HasColumnName("userId");

               /* entity.HasOne(d => d.User)
                    .WithMany(p => p.Photos)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Photo_User");*/
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.Property(e => e.Age).HasColumnName("age");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("email");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(100)
                    .HasColumnName("firstName");

                entity.Property(e => e.Pass)
                    .HasMaxLength(100)
                    .HasColumnName("pass");

                entity.Property(e => e.Role)
                    .HasMaxLength(100)
                    .HasColumnName("role");

                entity.Property(e => e.SecondName)
                    .HasMaxLength(100)
                    .HasColumnName("secondName");
                entity
                .HasMany(u => u.Hobbies)
                   .WithOne(h => h.User)
                   .OnDelete(DeleteBehavior.Cascade);
            });
          

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
