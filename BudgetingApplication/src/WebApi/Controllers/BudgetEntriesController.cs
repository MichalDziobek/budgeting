using Application.BudgetEntries.Commands.CreateBudgetEntry;
using Application.BudgetEntries.Commands.UpdateBudgetEntry;
using Application.BudgetEntries.Queries.GetBudgetEntries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers;

[ApiController]
[Route("budgets/{budgetId:int}/[controller]")]
[Authorize]
public class BudgetEntriesController : ControllerBase
{
    private readonly ISender _sender;

    public BudgetEntriesController(ISender sender)
    {
        _sender = sender;
    }
    
    [HttpPost]
    [SwaggerOperation("Create new budget entry")]
    public async Task<ActionResult<CreateBudgetEntryResponse>> Create(int budgetId, CreateBudgetEntryCommand createBudgetEntryCommand,
        CancellationToken cancellationToken = default)
    {
        createBudgetEntryCommand.BudgetId = budgetId;
        var result = await _sender.Send(createBudgetEntryCommand, cancellationToken);
        return Ok(result);
    }
    
    [HttpPut("{budgetEntryId:int}")]
    [SwaggerOperation("Update budget entry")]
    public async Task<ActionResult<UpdateBudgetEntryResponse>> Update(int budgetEntryId, UpdateBudgetEntryCommand createBudgetEntryCommand,
        CancellationToken cancellationToken = default)
    {
        createBudgetEntryCommand.BudgetEntryId = budgetEntryId;
        var result = await _sender.Send(createBudgetEntryCommand, cancellationToken);
        return Ok(result);
    }
    
    [HttpGet]
    [SwaggerOperation("Get filtered and paginated budget entries")]
    public async Task<ActionResult<GetBudgetEntriesResponse>> Get(int budgetId, [FromQuery]GetBudgetEntriesQuery createBudgetEntryCommand,
        CancellationToken cancellationToken = default)
    {
        createBudgetEntryCommand.BudgetId = budgetId;
        var result = await _sender.Send(createBudgetEntryCommand, cancellationToken);
        return Ok(result);
    }
}