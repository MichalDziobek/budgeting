using Application.Budgets.Commands.CreateBudget;
using Application.Budgets.Commands.ShareBudget;
using Application.Budgets.Commands.UpdateBudgetName;
using Application.Budgets.Queries.GetBudgets;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class BudgetsController : ControllerBase
{
    private readonly ISender _sender;

    public BudgetsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [SwaggerOperation("Create new budged")]
    public async Task<ActionResult<CreateBudgetResponse>> Create(CreateBudgetCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }
    
    [HttpGet]
    [SwaggerOperation("Get budgets", "Returns all budgets owned by and shared with current user")]
    public async Task<ActionResult<GetBudgetsResponse>> Get(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetBudgetsQuery(), cancellationToken);
        return Ok(result);
    }
    
    [HttpPatch("{budgetId:int}")]
    [SwaggerOperation("Update budget name")]
    public async Task<ActionResult<GetBudgetsResponse>> UpdateName(int budgetId, UpdateBudgetNameCommand command,CancellationToken cancellationToken)
    {
        command.BudgetId = budgetId;
        await _sender.Send(command, cancellationToken);
        return Ok();
    }
    
    [HttpPut("{budgetId:int}/share")]
    [SwaggerOperation("Share owned budget")]
    public async Task<ActionResult<GetBudgetsResponse>> ShareBudget(int budgetId, ShareBudgetCommand command,CancellationToken cancellationToken)
    {
        command.BudgetId = budgetId;
        await _sender.Send(command, cancellationToken);
        return Ok();
    }
}