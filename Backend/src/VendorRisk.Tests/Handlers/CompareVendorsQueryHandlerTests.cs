using FluentAssertions;
using Moq;
using VendorRisk.Application.Interfaces;
using VendorRisk.Application.Queries.CompareVendors;
using VendorRisk.Domain.Common;
using VendorRisk.Domain.Entities;
using VendorRisk.Domain.Enums;

namespace VendorRisk.Tests.Handlers;

public class CompareVendorsQueryHandlerTests
{
    private readonly Mock<IVendorRepository> _mockVendorRepo;
    private readonly Mock<IRiskCalculationService> _mockRiskCalc;
    private readonly Mock<IRiskAssessmentRepository> _mockAssessmentRepo;
    private readonly CompareVendorsQueryHandler _sut;

    public CompareVendorsQueryHandlerTests()
    {
        _mockVendorRepo = new Mock<IVendorRepository>();
        _mockRiskCalc = new Mock<IRiskCalculationService>();
        _mockAssessmentRepo = new Mock<IRiskAssessmentRepository>();
        _sut = new CompareVendorsQueryHandler(
            _mockVendorRepo.Object,
            _mockRiskCalc.Object,
            _mockAssessmentRepo.Object
        );
    }

    [Fact]
    public async Task Handle_ComparesMultipleVendors_ReturnsRankedComparison()
    {
        var vendor1 = CreateVendor(1, "Low Risk Vendor", 90, 99, 0, new List<string> { "ISO27001", "SOC2" }, true);
        var vendor2 = CreateVendor(2, "High Risk Vendor", 40, 85, 5, new List<string>(), false);

        var assessment1 = CreateAssessment(vendor1.Id, 0.15m, RiskLevel.Low);
        var assessment2 = CreateAssessment(vendor2.Id, 0.85m, RiskLevel.Critical);

        _mockVendorRepo.Setup(x => x.GetByIdAsync(vendor1.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vendor1);
        _mockVendorRepo.Setup(x => x.GetByIdAsync(vendor2.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vendor2);

        _mockRiskCalc.Setup(x => x.CalculateRiskAsync(vendor1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assessment1);
        _mockRiskCalc.Setup(x => x.CalculateRiskAsync(vendor2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assessment2);

        var request = new CompareVendorsQueryRequest
        {
            VendorIds = new List<int> { vendor1.Id, vendor2.Id }
        };

        var response = await _sut.Handle(request, CancellationToken.None);

        response.Should().NotBeNull();
        response.Comparison.Vendors.Should().HaveCount(2);

        var rankedVendors = response.Comparison.Vendors.OrderBy(v => v.Rank).ToList();
        rankedVendors[0].Id.Should().Be(vendor1.Id);
        rankedVendors[0].Rank.Should().Be(1);
        rankedVendors[1].Id.Should().Be(vendor2.Id);
        rankedVendors[1].Rank.Should().Be(2);
    }

    [Fact]
    public async Task Handle_IdentifiesStrengthsAndWeaknesses()
    {
        var vendor = CreateVendor(1, "Test Vendor", 85, 99.5m, 0, new List<string> { "ISO27001", "SOC2" }, true);
        var assessment = CreateAssessment(vendor.Id, 0.12m, RiskLevel.Low);

        _mockVendorRepo.Setup(x => x.GetByIdAsync(vendor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vendor);
        _mockRiskCalc.Setup(x => x.CalculateRiskAsync(vendor, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assessment);

        var request = new CompareVendorsQueryRequest
        {
            VendorIds = new List<int> { vendor.Id }
        };

        var response = await _sut.Handle(request, CancellationToken.None);

        var comparedVendor = response.Comparison.Vendors.First(v => v.Id == vendor.Id);
        comparedVendor.Strengths.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_GeneratesSummaryWithLowestAndHighestRisk()
    {
        var vendor1 = CreateVendor(1, "Best Vendor", 95, 99.9m, 0, new List<string> { "ISO27001", "SOC2", "PCI-DSS" }, true);
        var vendor2 = CreateVendor(2, "Worst Vendor", 30, 80m, 10, new List<string>(), false);
        var vendor3 = CreateVendor(3, "Average Vendor", 70, 95m, 2, new List<string> { "ISO27001" }, true);

        var assessment1 = CreateAssessment(vendor1.Id, 0.05m, RiskLevel.Low);
        var assessment2 = CreateAssessment(vendor2.Id, 0.95m, RiskLevel.Critical);
        var assessment3 = CreateAssessment(vendor3.Id, 0.35m, RiskLevel.Medium);

        _mockVendorRepo.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(vendor1);
        _mockVendorRepo.Setup(x => x.GetByIdAsync(2, It.IsAny<CancellationToken>())).ReturnsAsync(vendor2);
        _mockVendorRepo.Setup(x => x.GetByIdAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync(vendor3);

        _mockRiskCalc.Setup(x => x.CalculateRiskAsync(vendor1, It.IsAny<CancellationToken>())).ReturnsAsync(assessment1);
        _mockRiskCalc.Setup(x => x.CalculateRiskAsync(vendor2, It.IsAny<CancellationToken>())).ReturnsAsync(assessment2);
        _mockRiskCalc.Setup(x => x.CalculateRiskAsync(vendor3, It.IsAny<CancellationToken>())).ReturnsAsync(assessment3);

        var request = new CompareVendorsQueryRequest
        {
            VendorIds = new List<int> { 1, 2, 3 }
        };

        var response = await _sut.Handle(request, CancellationToken.None);

        response.Comparison.Summary.Should().NotBeNull();
        response.Comparison.Summary.LowestRiskVendor.Should().Be("Best Vendor");
        response.Comparison.Summary.HighestRiskVendor.Should().Be("Worst Vendor");
        response.Comparison.Summary.AverageRiskScore.Should().BeApproximately(0.45m, 0.01m);
    }

    private VendorProfile CreateVendor(
        int id,
        string name,
        decimal financialHealth,
        decimal slaUptime,
        int incidents,
        List<string> certs,
        bool allDocsValid)
    {
        var documentStatus = new DocumentStatus(allDocsValid, allDocsValid, allDocsValid);
        var vendor = VendorProfile.Create(name, financialHealth, slaUptime, incidents, certs, documentStatus);

        typeof(VendorProfile).GetProperty("Id")!.SetValue(vendor, id);

        return vendor;
    }

    private RiskAssessment CreateAssessment(int vendorId, decimal riskScore, RiskLevel riskLevel)
    {
        return RiskAssessment.Create(vendorId, riskScore, riskLevel, "Test reason");
    }
}
