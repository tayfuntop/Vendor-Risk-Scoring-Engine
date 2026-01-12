using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using VendorRisk.Domain.Entities;

namespace VendorRisk.Infrastructure.Persistence.PostgreSql.Configurations;

public class VendorProfileConfiguration : IEntityTypeConfiguration<VendorProfile>
{
    public void Configure(EntityTypeBuilder<VendorProfile> builder)
    {
        builder.ToTable("VendorProfiles");
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id)
            .ValueGeneratedOnAdd();

        builder.Property(v => v.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(v => v.FinancialHealth)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(v => v.SlaUptime)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(v => v.MajorIncidents)
            .IsRequired();

        builder.Property(v => v.SecurityCerts)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<List<string>>(v) ?? new List<string>())
            .IsRequired();

        builder.OwnsOne(v => v.Documents, doc =>
        {
            doc.Property(d => d.ContractValid)
                .HasColumnName("ContractValid")
                .IsRequired();

            doc.Property(d => d.PrivacyPolicyValid)
                .HasColumnName("PrivacyPolicyValid")
                .IsRequired();

            doc.Property(d => d.PentestReportValid)
                .HasColumnName("PentestReportValid")
                .IsRequired();
        });

        builder.HasMany(v => v.RiskAssessments)
            .WithOne(r => r.Vendor)
            .HasForeignKey(r => r.VendorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(v => v.Name);
    }
}
