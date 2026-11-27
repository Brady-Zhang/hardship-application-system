using HardshipApp.Api.Contracts;
using HardshipApp.Models;
using HardshipApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace HardshipApp.Api.Controllers;

[ApiController]
[Route("api/applications")]
public sealed class HardshipApplicationsController(IHardshipApplicationService applications)
    : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<HardshipApplicationResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        HardshipApplicationRequest request,
        CancellationToken cancellationToken)
    {
        HardshipApplication created = await applications.CreateAsync(request.ToEntity(), cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToResponse());
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<HardshipApplicationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        HardshipApplication? application = await applications.FindAsync(id, cancellationToken);

        return application is null
            ? Problem(statusCode: StatusCodes.Status404NotFound, title: "Application not found")
            : Ok(application.ToResponse());
    }
}
