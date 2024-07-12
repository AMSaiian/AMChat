using AMChat.Application.Common.Interfaces;
using AMChat.Application.Common.Models.Chat;
using AMChat.Application.Common.Models.Pagination;
using AMChat.Core.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AMChat.Application.Chats.Queries.GetChats;

public record GetChatsQuery : IRequest<List<ChatDto>>
{
    public required string SearchPattern { get; init; } = "";
    public OrderContext? OrderContext { get; init; }
}

public class GetChatsHandler(IAppDbContext dbContext,
                             IMapper mapper,
                             IPaginationService paginationService)
    : IRequestHandler<GetChatsQuery, List<ChatDto>>
{
    private readonly IAppDbContext _dbContext = dbContext;
    private readonly IMapper _mapper = mapper;
    private readonly IPaginationService _paginationService = paginationService;

    public async Task<List<ChatDto>> Handle(GetChatsQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Chat> chatsQuery = _dbContext.Chats
            .AsNoTracking()
            .Include(chat => chat.Owner)
            .Where(chat => chat.Name.Contains(request.SearchPattern));

        if (request.OrderContext is not null)
        {
            chatsQuery = _paginationService
                .TryOrderDynamically(chatsQuery, request.OrderContext);
        }

        List<Chat> entities = await chatsQuery.ToListAsync(cancellationToken);
        List<ChatDto> dtos = _mapper.Map<List<ChatDto>>(entities);

        return dtos;
    }
}
