using AMChat.Application.Common.Exceptions;
using AMChat.Application.Common.Interfaces;
using AMChat.Application.Common.Models.User;
using AMChat.Application.Common.Templates;
using AMChat.Core.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AMChat.Application.Users.Queries.GetUserById;

public record GetUserByIdQuery : IRequest<UserDetailedDto>
{
    public required Guid Id { get; init; }
}

public class GetUserByIdHandler(IAppDbContext dbContext,
                                IMapper mapper)
    : IRequestHandler<GetUserByIdQuery, UserDetailedDto>
{
    private readonly IAppDbContext _dbContext = dbContext;
    private readonly IMapper _mapper = mapper;

    public async Task<UserDetailedDto> Handle(GetUserByIdQuery request,
                                              CancellationToken cancellationToken)
    {
        User needUser = await _dbContext.Users
            .AsNoTracking()
            .Include(user => user.Profile)
            .Include(user => user.JoinedChats)
            .Include(user => user.OwnedChats)
            .FirstOrDefaultAsync(user => user.Id == request.Id,
                                 cancellationToken)
                     ?? throw new NotFoundException(
                            string.Format(ErrorTemplates.EntityNotFoundFormat,
                                          nameof(User)));

        UserDetailedDto needUserDto = _mapper.Map<UserDetailedDto>(needUser);

        return needUserDto;
    }
}
