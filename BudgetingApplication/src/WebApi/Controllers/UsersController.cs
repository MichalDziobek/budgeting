using Application.Users.Commands;
using Application.Users.Queries.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<ActionResult> Create(CreateUserCommand createUserCommand, CancellationToken cancellationToken = default)
    {
        await _sender.Send(createUserCommand, cancellationToken);
        return Ok();
    }
    
    [HttpGet]
    public async Task<ActionResult<GetUsersResponse>> Get([FromQuery] GetUsersQuery getUsersQuery, CancellationToken cancellationToken = default)
    {
        var response = await _sender.Send(getUsersQuery, cancellationToken);
        return Ok(response);
    }
}