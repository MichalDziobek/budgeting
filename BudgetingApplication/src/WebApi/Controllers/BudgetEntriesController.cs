using Application.BudgetEntries.Commands.CreateBudgetEntry;
using Application.BudgetEntries.Queries.GetBudgetEntries;
using Application.Budgets.Queries.GetBudgets;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<ActionResult<CreateBudgetEntryResponse>> Create(int budgetId, CreateBudgetEntryCommand createBudgetEntryCommand,
        CancellationToken cancellationToken = default)
    {
        createBudgetEntryCommand.BudgetId = budgetId;
        var result = await _sender.Send(createBudgetEntryCommand, cancellationToken);
        return Ok(result);
    }
    
    [HttpGet]
    public async Task<ActionResult<GetBudgetEntriesResponse>> Get(int budgetId, [FromQuery]GetBudgetEntriesQuery createBudgetEntryCommand,
        CancellationToken cancellationToken = default)
    {
        createBudgetEntryCommand.BudgetId = budgetId;
        var result = await _sender.Send(createBudgetEntryCommand, cancellationToken);
        return Ok(result);
    }
}