using AMChat.Application.Chats.Queries.GetChats;
using AMChat.Application.Chats.Queries.GetPaginatedChats;
using AMChat.Application.Common.Models.Chat;
using AMChat.Application.Common.Models.Pagination;
using AMChat.Contract.Queries;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AMChat.Controllers;

public class ChatsController(IMediator mediator, IMapper mapper) : ApiControllerBase(mediator)
{
    private readonly IMapper _mapper = mapper;

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
                    : new() { PropertyName = "name", IsDescending = false }
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
