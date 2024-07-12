using AMChat.Application.Common.Interfaces;
using AMChat.Application.Common.Models.Pagination;
using AMChat.Application.Common.Models.User;
using AMChat.Core.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AMChat.Application.Users.Queries.GetUsersPaginated;

public record GetUsersPaginatedQuery : IRequest<Paginated<UserDto>>
{
    public required PaginationContext PaginationContext { get; init; }
    public string SearchPattern { get; init; } = "";
}

public class GetUsersPaginatedHandler(IAppDbContext dbContext,
                                      IMapper mapper,
                                      IPaginationService paginationService)
    : IRequestHandler<GetUsersPaginatedQuery, Paginated<UserDto>>
{
    private readonly IAppDbContext _dbContext = dbContext;
    private readonly IMapper _mapper = mapper;
    private readonly IPaginationService _paginationService = paginationService;

    public async Task<Paginated<UserDto>> Handle(GetUsersPaginatedQuery request,
                                            CancellationToken cancellationToken)
    {
        IQueryable<User> usersQuery = _dbContext.Users
            .AsNoTracking()
            .Include(user => user.Profile)
            .Where(user => user.Name.Contains(request.SearchPattern));


        Paginated<User> paginatedEntities = await _paginationService
            .PaginateAsync(usersQuery,
                           request.PaginationContext,
                           cancellationToken);

        Paginated<UserDto> paginatedDtos = new()
        {
            PaginationInfo = paginatedEntities.PaginationInfo,
            Records = _mapper.Map<List<UserDto>>(paginatedEntities.Records)
        };

        return paginatedDtos;
    }
}
