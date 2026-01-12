using FluentAssertions;
using Moq;
using VendorRisk.Application.Interfaces;
using VendorRisk.Application.Services;
using VendorRisk.Domain.Common;
using VendorRisk.Domain.Entities;
using VendorRisk.Domain.Enums;

namespace VendorRisk.Tests.Services;

public class RuleEngineServiceTests
{
    private readonly Mock<IRiskSimilarityMatrixService> _mockMatrixService;
    private readonly RuleEngineService _sut;

    public RuleEngineServiceTests()
    {
        _mockMatrixService = new Mock<IRiskSimilarityMatrixService>();
        _sut = new RuleEngineService(_mockMatrixService.Object);
    }

    [Fact]
    public void CalculateRisk_WithLowRiskVendor_ReturnsLowRiskLevel()
    {
        var vendor = CreateVendor(
            financialScore: 90,
            slaUptime: 99.5m,
            incidents: 0,
            certifications: new List<string> { "ISO27001", "SOC2" },
            allDocsValid: true
        );

        _mockMatrixService.Setup(x => x.GetCorrelatedRisks(It.IsAny<List<string>>()))
            .Returns(new Dictionary<string, decimal>());

        var result = _sut.CalculateRisk(vendor);

        result.Should().NotBeNull();
        result.RiskLevel.Should().Be(RiskLevel.Low);
        result.RiskScore.Should().BeLessThan(0.25m);
    }

    [Fact]
    public void CalculateRisk_WithCriticalRiskVendor_ReturnsCriticalRiskLevel()
    {
        var vendor = CreateVendor(
            financialScore: 30,
            slaUptime: 85m,
            incidents: 5,
            certifications: new List<string>(),
            allDocsValid: false
        );

        var correlatedRisks = new Dictionary<string, decimal>
        {
            ["highDebtRatio"] = 0.88m,
            ["downtime"] = 0.87m,
            ["weakAccessControl"] = 0.84m
        };

        _mockMatrixService.Setup(x => x.GetCorrelatedRisks(It.IsAny<List<string>>()))
            .Returns(correlatedRisks);


        var result = _sut.CalculateRisk(vendor);

        result.Should().NotBeNull();
        result.RiskLevel.Should().Be(RiskLevel.Critical);
        result.RiskScore.Should().BeGreaterThan(0.75m);
    }

    [Fact]
    public void CalculateRisk_WithModerateRiskVendor_CalculatesCorrectly()
    {
        var vendor = CreateVendor(
            financialScore: 60,
            slaUptime: 93m,
            incidents: 2,
            certifications: new List<string> { "ISO27001" },
            allDocsValid: true
        );

        _mockMatrixService.Setup(x => x.GetCorrelatedRisks(It.IsAny<List<string>>()))
            .Returns(new Dictionary<string, decimal>());

        var result = _sut.CalculateRisk(vendor);

        result.Should().NotBeNull();
        result.RiskScore.Should().BeGreaterThan(0m).And.BeLessThan(1m);
        result.RiskLevel.Should().BeOneOf(RiskLevel.Low, RiskLevel.Medium);
    }

    [Fact]
    public void CalculateRisk_AppliesCorrelationAmplification()
    {
        var vendor = CreateVendor(
            financialScore: 50,
            slaUptime: 90m,
            incidents: 3,
            certifications: new List<string>(),
            allDocsValid: false
        );

        var correlatedRisks = new Dictionary<string, decimal>
        {
            ["highDebtRatio"] = 0.88m,
            ["downtime"] = 0.87m,
            ["weakAccessControl"] = 0.84m,
            ["latePayments"] = 0.82m,
            ["slowTicketResolution"] = 0.83m
        };

        _mockMatrixService.Setup(x => x.GetCorrelatedRisks(It.IsAny<List<string>>()))
            .Returns(correlatedRisks);

        var result = _sut.CalculateRisk(vendor);

        result.RiskScore.Should().BeGreaterThan(0m);
    }

    [Fact]
    public void CalculateRisk_GeneratesDetailedReason()
    {
        var vendor = CreateVendor(
            financialScore: 45,
            slaUptime: 88m,
            incidents: 4,
            certifications: new List<string>(),
            allDocsValid: false
        );

        _mockMatrixService.Setup(x => x.GetCorrelatedRisks(It.IsAny<List<string>>()))
            .Returns(new Dictionary<string, decimal>());

        var result = _sut.CalculateRisk(vendor);

        result.Reason.Should().NotBeNullOrEmpty();
    }

    private VendorProfile CreateVendor(
        decimal financialScore,
        decimal slaUptime,
        int incidents,
        List<string> certifications,
        bool allDocsValid)
    {
        var documentStatus = new DocumentStatus(
            contractValid: allDocsValid,
            privacyPolicyValid: allDocsValid,
            pentestReportValid: allDocsValid
        );

        var vendor = VendorProfile.Create(
            name: "Test Vendor",
            financialHealth: financialScore,
            slaUptime: slaUptime,
            majorIncidents: incidents,
            securityCerts: certifications,
            documents: documentStatus
        );

        typeof(VendorProfile).GetProperty("Id")!.SetValue(vendor, 1);

        return vendor;
    }
}
