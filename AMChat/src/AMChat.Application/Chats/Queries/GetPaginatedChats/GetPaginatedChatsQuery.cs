using AMChat.Application.Common.Interfaces;
using AMChat.Application.Common.Models.Chat;
using AMChat.Application.Common.Models.Pagination;
using AMChat.Core.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AMChat.Application.Chats.Queries.GetPaginatedChats;

public record GetPaginatedChatsQuery : IRequest<Paginated<ChatDto>>
{
    public required PaginationContext PaginationContext { get; init; }
    public string SearchPattern { get; init; } = "";
}

public class GetPaginatedChatsHandler(IAppDbContext dbContext,
                                      IMapper mapper,
                                      IPaginationService paginationService)
    : IRequestHandler<GetPaginatedChatsQuery, Paginated<ChatDto>>
{
    private readonly IAppDbContext _dbContext = dbContext;
    private readonly IMapper _mapper = mapper;
    private readonly IPaginationService _paginationService = paginationService;

    public async Task<Paginated<ChatDto>> Handle(GetPaginatedChatsQuery request,
                                                 CancellationToken cancellationToken)
    {
        IQueryable<Chat> chatsQuery = _dbContext.Chats
            .AsNoTracking()
            .Include(chat => chat.Owner)
            .Where(chat => chat.Name.Contains(request.SearchPattern));

        Paginated<Chat> paginatedEntities = await _paginationService
            .PaginateAsync(chatsQuery,
                           request.PaginationContext,
                           cancellationToken);

        Paginated<ChatDto> paginatedDtos = new()
        {
            PaginationInfo = paginatedEntities.PaginationInfo,
            Records = _mapper.Map<List<ChatDto>>(paginatedEntities.Records)
        };

        return paginatedDtos;
    }
}
