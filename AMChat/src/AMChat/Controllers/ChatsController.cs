using AMChat.Application.Chats.Command.CreateChat;
using AMChat.Application.Chats.Command.DeleteChat;
using AMChat.Application.Chats.Command.JoinChat;
using AMChat.Application.Chats.Command.LeftChat;
using AMChat.Application.Chats.Command.UpdateChat;
using AMChat.Application.Chats.Queries.GetChatById;
using AMChat.Application.Chats.Queries.GetChats;
using AMChat.Application.Chats.Queries.GetPaginatedChats;
using AMChat.Application.Common.Models.Chat;
using AMChat.Application.Common.Models.Pagination;
using AMChat.Common.Constants;
using AMChat.Common.Contract.Queries;
using AMChat.Common.Contract.Requests.Chats;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AMChat.Controllers;

public class ChatsController(IMediator mediator,
                             IMapper mapper)
    : ApiControllerBase(mediator, mapper)
{
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChatDetailedDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetChatById([FromRoute] Guid id,
                                                 CancellationToken cancellationToken = default)
    {
        GetChatByIdQuery query = new()
        {
            Id = id
        };

        ChatDetailedDto result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateChat([FromBody] CreateChatRequest newChat,
                                                CancellationToken cancellationToken = default)
    {
        CreateChatCommand command = _mapper.Map<CreateChatCommand>(newChat);

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
    public async Task<IActionResult> UpdateChat([FromRoute] Guid id,
                                                [FromBody] UpdateChatRequest updatedChat,
                                                CancellationToken cancellationToken = default)
    {
        UpdateChatCommand command = _mapper.Map<UpdateChatCommand>(updatedChat);

        command = command with { Id = id };

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
    public async Task<IActionResult> DeleteChat([FromRoute] Guid id,
                                                CancellationToken cancellationToken = default)
    {
        DeleteChatCommand command = new()
        {
            Id = id
        };

        await _mediator.Send(command, cancellationToken);

        return Ok();
    }

    [HttpPost("{id}/users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> JoinChat([FromRoute] Guid id,
                                              [FromBody] Guid userId,
                                              CancellationToken cancellationToken = default)
    {
        JoinChatCommand command = new()
        {
            ChatId = id,
            UserId = userId
        };

        await _mediator.Send(command, cancellationToken);

        return Ok();
    }

    [HttpDelete("{id}/users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> LeftChat([FromRoute] Guid id,
                                              [FromBody] Guid userId,
                                              CancellationToken cancellationToken = default)
    {
        LeftChatCommand command = new()
        {
            ChatId = id,
            UserId = userId
        };

        await _mediator.Send(command, cancellationToken);

        return Ok();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Paginated<ChatDto>))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ChatDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetChats([FromQuery] PaginationQuery page,
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
                        PropertyName = DefaultPaginationOrderProperties.ChatOrder,
                        IsDescending = false
                    }
            };

            GetPaginatedChatsQuery query = new()
            {
                PaginationContext = context,
                SearchPattern = search
            };

            Paginated<ChatDto> result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
        else
        {
            OrderContext? context = order.PropertyName is not null
                ? _mapper.Map<OrderContext>(order)
                : null;

            GetChatsQuery query = new()
            {
                OrderContext = context,
                SearchPattern = search
            };

            List<ChatDto> result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
    }
}
