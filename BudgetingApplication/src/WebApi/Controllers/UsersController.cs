using Application.Users.Commands.CreateUser;
using Application.Users.Queries.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [SwaggerOperation("Create new user")]
    public async Task<ActionResult> Create(CreateUserCommand createUserCommand, CancellationToken cancellationToken = default)
    {
        await _sender.Send(createUserCommand, cancellationToken);
        return Ok();
    }
    
    [HttpGet]
    [SwaggerOperation("Get filtered users")]
    public async Task<ActionResult<GetUsersResponse>> Get([FromQuery] GetUsersQuery getUsersQuery, CancellationToken cancellationToken = default)
    {
        var response = await _sender.Send(getUsersQuery, cancellationToken);
        return Ok(response);
    }
}