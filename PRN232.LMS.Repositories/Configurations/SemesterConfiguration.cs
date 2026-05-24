using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Configurations;

public class SemesterConfiguration : IEntityTypeConfiguration<Semester>
{
    public void Configure(EntityTypeBuilder<Semester> builder)
    {
        builder.ToTable("Semester");
        builder.HasKey(s => s.SemesterId);
        builder.Property(s => s.SemesterName).HasMaxLength(100).IsRequired();
        builder.Property(s => s.StartDate).HasColumnType("datetime");
        builder.Property(s => s.EndDate).HasColumnType("datetime");
    }
}

