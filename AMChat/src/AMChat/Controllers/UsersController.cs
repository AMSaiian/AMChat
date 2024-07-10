using AMChat.Application.Users.Commands.CreateUser;
using AMChat.Application.Users.Commands.UpdateUser;
using AMChat.Application.Users.Commands.UpdateUserProfile;
using AMChat.Contract.Requests.Users;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AMChat.Controllers;

public class UsersController(IMediator mediator,
                             IMapper mapper)
    : ApiControllerBase(mediator, mapper)
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest newUser,
                                                CancellationToken cancellationToken = default)
    {
        CreateUserCommand command = _mapper.Map<CreateUserCommand>(newUser);

        Guid result = await _mediator.Send(command, cancellationToken);

        return Ok(result);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateUser([FromRoute] Guid id,
                                                [FromBody] UpdateUserRequest updatedUser,
                                                CancellationToken cancellationToken = default)
    {
        UpdateUserCommand command = _mapper.Map<UpdateUserCommand>(updatedUser);

        command = command with { Id = id };

        await _mediator.Send(command, cancellationToken);

        return Ok();
    }

    [HttpPut("{id}/profile")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateUserProfile([FromRoute] Guid id,
                                                       [FromBody] UpdateUserProfileRequest updatedProfile,
                                                       CancellationToken cancellationToken = default)
    {
        UpdateUserProfileCommand command = _mapper.Map<UpdateUserProfileCommand>(updatedProfile);

        command = command with { UserId = id };

        await _mediator.Send(command, cancellationToken);

        return Ok();
    }
}
