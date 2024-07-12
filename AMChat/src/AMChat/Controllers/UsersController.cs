using AMChat.Application.Common.Models.Pagination;
using AMChat.Application.Common.Models.User;
using AMChat.Application.Users.Commands.CreateUser;
using AMChat.Application.Users.Commands.DeleteUser;
using AMChat.Application.Users.Commands.UpdateUser;
using AMChat.Application.Users.Commands.UpdateUserProfile;
using AMChat.Application.Users.Queries.GetUserById;
using AMChat.Application.Users.Queries.GetUsers;
using AMChat.Application.Users.Queries.GetUsersPaginated;
using AMChat.Common.Constants;
using AMChat.Common.Contract.Queries;
using AMChat.Common.Contract.Requests.Users;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AMChat.Controllers;

public class UsersController(IMediator mediator,
                             IMapper mapper)
    : ApiControllerBase(mediator, mapper)
{
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDetailedDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserById([FromRoute] Guid id,
                                                 CancellationToken cancellationToken = default)
    {
        GetUserByIdQuery query = new()
        {
            Id = id
        };

        UserDetailedDto result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

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
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteUser([FromRoute] Guid id,
                                                CancellationToken cancellationToken = default)
    {
        DeleteUserCommand command = new() { Id = id };

        await _mediator.Send(command, cancellationToken);

        return Ok();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Paginated<UserDto>))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUsers([FromQuery] PaginationQuery page,
                                              [FromQuery] OrderQuery order,
                                              [FromQuery] string search = "",
                                              CancellationToken cancellationToken = default)
    {
        if (page.PageNumber is not null
         && page.PageSize is not null)
        {
            PaginationContext context = new()
            {
                PageContext = _mapper.Map<PageContext>(page),
                OrderContext = order.PropertyName is not null
                    ? _mapper.Map<OrderContext>(order)
                    : new()
                    {
                        PropertyName = DefaultPaginationOrderProperties.UserOrder,
                        IsDescending = false
                    }
            };

            GetUsersPaginatedQuery query = new()
            {
                PaginationContext = context,
                SearchPattern = search
            };

            Paginated<UserDto> result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
        else
        {
            OrderContext? context = order.PropertyName is not null
                ? _mapper.Map<OrderContext>(order)
                : null;

            GetUsersQuery query = new()
            {
                OrderContext = context,
                SearchPattern = search
            };

            List<UserDto> result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
    }
}
