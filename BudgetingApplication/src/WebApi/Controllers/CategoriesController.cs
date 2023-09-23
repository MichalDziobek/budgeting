using Application.Categories.Commands.CreateCategoryCommand;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly ISender _sender;

    public CategoriesController(ISender sender)
    {
        _sender = sender;
    }
    
    [HttpPost]
    public async Task<ActionResult<CreateCategoryResponse>> Create(CreateCategoryCommand createCategoryCommand, CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(createCategoryCommand, cancellationToken);
        return Ok(result);
    }
}