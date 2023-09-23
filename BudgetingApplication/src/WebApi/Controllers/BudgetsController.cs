using Application.Budgets.Commands.CreateBudget;
using Application.Budgets.Queries.GetBudgets;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<ActionResult<CreateBudgetResponse>> Create(CreateBudgetCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }
    
    [HttpGet]
    public async Task<ActionResult<GetBudgetsResponse>> Get(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetBudgetsQuery(), cancellationToken);
        return Ok(result);
    }
}