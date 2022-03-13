using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Lab4
{
    public partial class DBUniWorkersContext : DbContext
    {
        public DBUniWorkersContext()
        {
        }

        public DBUniWorkersContext(DbContextOptions<DBUniWorkersContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Department> Departments { get; set; } = null!;
        public virtual DbSet<Faculty> Faculties { get; set; } = null!;
        public virtual DbSet<Paycheck> Paychecks { get; set; } = null!;
        public virtual DbSet<PeoplePosition> PeoplePositions { get; set; } = null!;
        public virtual DbSet<Person> People { get; set; } = null!;
        public virtual DbSet<Position> Positions { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-5NBPC5J; Database=DBUniWorkers;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DepartmentName).HasColumnName("department_name");

                entity.Property(e => e.FacultyId).HasColumnName("faculty_id");

                entity.HasOne(d => d.Faculty)
                    .WithMany(p => p.Departments)
                    .HasForeignKey(d => d.FacultyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Departments_Faculties");
            });

            modelBuilder.Entity<Faculty>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.FacultyName).HasColumnName("faculty_name");
            });

            modelBuilder.Entity<Paycheck>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.MonthDate)
                    .HasColumnType("date")
                    .HasColumnName("month_date");

                entity.Property(e => e.PersonId).HasColumnName("person_id");

                entity.Property(e => e.Salary)
                    .HasColumnType("money")
                    .HasColumnName("salary");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Paychecks)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Paychecks_People");
            });

            modelBuilder.Entity<PeoplePosition>(entity =>
            {
                entity.HasKey(e => new { e.PersonId, e.PositionId });

                entity.ToTable("People_Positions");

                entity.Property(e => e.PersonId).HasColumnName("person_id");

                entity.Property(e => e.PositionId).HasColumnName("position_id");

                entity.Property(e => e.Finish)
                    .HasColumnType("date")
                    .HasColumnName("finish");

                entity.Property(e => e.Start)
                    .HasColumnType("date")
                    .HasColumnName("start");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.PeoplePositions)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_People_Positions_People");

                entity.HasOne(d => d.Position)
                    .WithMany(p => p.PeoplePositions)
                    .HasForeignKey(d => d.PositionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_People_Positions_Positions");
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DepartmentId).HasColumnName("department_id");

                entity.Property(e => e.PersonName).HasColumnName("person_name");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.People)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_People_Departments");
            });

            modelBuilder.Entity<Position>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.PositionName).HasColumnName("position_name");

                entity.Property(e => e.Salary)
                    .HasColumnType("money")
                    .HasColumnName("salary");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
