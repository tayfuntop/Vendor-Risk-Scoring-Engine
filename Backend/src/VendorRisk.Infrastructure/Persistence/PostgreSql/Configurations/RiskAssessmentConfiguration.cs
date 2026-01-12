using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VendorRisk.Domain.Entities;

namespace VendorRisk.Infrastructure.Persistence.PostgreSql.Configurations;

public class RiskAssessmentConfiguration : IEntityTypeConfiguration<RiskAssessment>
{
    public void Configure(EntityTypeBuilder<RiskAssessment> builder)
    {
        builder.ToTable("RiskAssessments");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .ValueGeneratedOnAdd();

        builder.Property(r => r.VendorId)
            .IsRequired();

        builder.Property(r => r.RiskScore)
            .HasPrecision(5, 4)
            .IsRequired();

        builder.Property(r => r.RiskLevel)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(r => r.Reason)
            .IsRequired();

        builder.Property(r => r.AssessedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(r => r.VendorId);
        builder.HasIndex(r => r.RiskLevel);
        builder.HasIndex(r => r.AssessedAt);
        builder.HasIndex(r => new { r.VendorId, r.AssessedAt });
    }
}
