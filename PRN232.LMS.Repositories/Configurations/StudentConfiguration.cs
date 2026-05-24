using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Student");
        builder.HasKey(s => s.StudentId);
        builder.Property(s => s.FullName).HasMaxLength(100).IsRequired();
        builder.Property(s => s.Email).HasMaxLength(100).IsRequired();
        builder.Property(s => s.DateOfBirth).HasColumnType("datetime");
        builder.HasIndex(s => s.Email).IsUnique();
    }
}

