using MediatR;
using Microsoft.AspNetCore.Mvc;
using VendorRisk.Application.Commands.CreateVendor;
using VendorRisk.Application.Queries.CompareVendors;
using VendorRisk.Application.Queries.GetAllVendors;
using VendorRisk.Application.Queries.GetVendorById;
using VendorRisk.Application.Queries.GetVendorRiskAssessment;

namespace VendorRisk.Api.Controller;

[ApiController]
[Route("api/vendor")]
public class VendorsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<VendorsController> _logger;

    public VendorsController(IMediator mediator, ILogger<VendorsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Create a new vendor profile
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateVendor([FromBody] CreateVendorCommandRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new vendor: {VendorName}", request.Name);
        var response = await _mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(GetVendorById), new { id = response.Id }, response);
    }

    /// <summary>
    /// Get all vendors
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllVendors(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching all vendors");
        var request = new GetAllVendorsQueryRequest();
        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Get vendor by ID
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVendorById(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching vendor: {VendorId}", id);
        var request = new GetVendorByIdQueryRequest { VendorId = id };
        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Get vendor risk assessment - Rule-based scoring engine
    /// </summary>
    [HttpGet("{id:int}/risk")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVendorRiskAssessment(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Calculating risk assessment for vendor: {VendorId}", id);
        var request = new GetVendorRiskAssessmentQueryRequest { VendorId = id };
        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Compare multiple vendors side-by-side with risk analysis
    /// </summary>
    [HttpPost("compare")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CompareVendors([FromBody] CompareVendorsQueryRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Comparing {Count} vendors", request.VendorIds.Count);
        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }
}
