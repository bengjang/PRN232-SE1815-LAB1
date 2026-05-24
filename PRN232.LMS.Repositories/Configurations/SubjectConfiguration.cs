using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Configurations;

public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.ToTable("Subject");
        builder.HasKey(s => s.SubjectId);
        builder.Property(s => s.SubjectCode).HasMaxLength(20).IsRequired();
        builder.Property(s => s.SubjectName).HasMaxLength(100).IsRequired();
        builder.HasIndex(s => s.SubjectCode).IsUnique();
    }
}

