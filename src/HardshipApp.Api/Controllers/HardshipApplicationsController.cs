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
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<HardshipApplicationResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        IReadOnlyList<HardshipApplication> applicationList = await applications.ListAsync(cancellationToken);

        return Ok(applicationList.Select(application => application.ToResponse()));
    }

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

        return application is null ? ApplicationNotFound() : Ok(application.ToResponse());
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<HardshipApplicationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        HardshipApplicationRequest request,
        CancellationToken cancellationToken)
    {
        HardshipApplication? existing = await applications.FindAsync(id, cancellationToken);

        if (existing is null)
        {
            return ApplicationNotFound();
        }

        request.ApplyTo(existing);

        HardshipApplication updated = await applications.UpdateAsync(existing, cancellationToken);

        return Ok(updated.ToResponse());
    }

    // One shape for a missing record, whichever endpoint was asked.
    private ObjectResult ApplicationNotFound() => Problem(
        statusCode: StatusCodes.Status404NotFound,
        title: "Application not found",
        detail: "No hardship application exists with that id.");
}
