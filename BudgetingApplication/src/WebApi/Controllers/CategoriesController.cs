using Application.Categories.Commands.CreateCategory;
using Application.Categories.Commands.DeleteCategory;
using Application.Categories.Queries.GetCategories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebApi.Authorization;

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
    [Authorize(Policy = AuthorizationPolicies.CreateCategory)]
    [SwaggerOperation("Create new category", "Admin only")]
    public async Task<ActionResult<CreateCategoryResponse>> Create(CreateCategoryCommand createCategoryCommand, CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(createCategoryCommand, cancellationToken);
        return Ok(result);
    }
    
    [HttpDelete("{categoryId:int}")]
    [Authorize(Policy = AuthorizationPolicies.DeleteCategory)]
    [SwaggerOperation("Delete existing category", "Admin only, deletes all budget entries from this category")]
    public async Task<ActionResult> Delete(int categoryId, CancellationToken cancellationToken = default)
    {
        var command = new DeleteCategoryCommand() { CategoryId = categoryId };
        await _sender.Send(command, cancellationToken);
        return Ok();
    }
    
    [HttpGet]
    [SwaggerOperation("Get categories filtered by name")]
    public async Task<ActionResult<GetCategoriesResponse>> Get([FromQuery]GetCategoriesQuery createCategoryCommand, CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(createCategoryCommand, cancellationToken);
        return Ok(result);
    }
}