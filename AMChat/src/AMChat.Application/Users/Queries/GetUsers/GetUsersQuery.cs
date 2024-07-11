using AMChat.Application.Common.Interfaces;
using AMChat.Application.Common.Models.Pagination;
using AMChat.Application.Common.Models.User;
using AMChat.Core.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AMChat.Application.Users.Queries.GetUsers;

public record GetUsersQuery : IRequest<List<UserDto>>
{
    public required string SearchPattern { get; init; } = "";
    public OrderContext? OrderContext { get; init; }
}

public class GetUsersHandler(IAppDbContext dbContext,
                              IMapper mapper,
                              IPaginationService paginationService)
    : IRequestHandler<GetUsersQuery, List<UserDto>>
{
    private readonly IAppDbContext _dbContext = dbContext;
    private readonly IMapper _mapper = mapper;
    private readonly IPaginationService _paginationService = paginationService;

    public async Task<List<UserDto>> Handle(GetUsersQuery request,
                                            CancellationToken cancellationToken)
    {
        IQueryable<User> usersQuery = _dbContext.Users
            .AsNoTracking()
            .Include(user => user.Profile)
            .Include(user => user.JoinedChats)
            .Include(user => user.OwnedChats)
            .Where(user => user.Name.Contains(request.SearchPattern));

        if (request.OrderContext is not null)
        {
            usersQuery = _paginationService
                .TryOrderDynamically(usersQuery, request.OrderContext);
        }

        List<User> entities = await usersQuery.ToListAsync(cancellationToken);
        List<UserDto> dtos = _mapper.Map<List<UserDto>>(entities);

        return dtos;
    }
}
