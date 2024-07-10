using AMChat.Application.Common.Exceptions;
using AMChat.Application.Common.Interfaces;
using AMChat.Application.Common.Templates;
using AMChat.Core.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AMChat.Application.Users.Commands.UpdateUser;

public record UpdateUserCommand : IRequest
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}

public class UpdateUserHandler(IAppDbContext dbContext,
                               IMapper mapper,
                               ICurrentUserService currentUser)
    : IRequestHandler<UpdateUserCommand>
{
    private readonly IAppDbContext _dbContext = dbContext;
    private readonly IMapper _mapper = mapper;
    private readonly ICurrentUserService _currentUser = currentUser;

    public async Task Handle(UpdateUserCommand request,
                             CancellationToken cancellationToken)
    {
        cancellationToken = CancellationToken.None;

        Guid currentUserId = _currentUser.GetUserIdOrThrow();

        if (currentUserId != request.Id)
        {
            throw new ForbiddenAccessException(ErrorTemplates.ForbiddenUpdateNotOwnedUser);
        }

        User updatingUser = await _dbContext.Users
            .FindAsync(request.Id, cancellationToken)
                 ?? throw new NotFoundException(
                                string.Format(ErrorTemplates.EntityNotFoundFormat,
                                              nameof(User)));

        bool isUserWithUserNameExists = await _dbContext.Users
            .AnyAsync(user => user.Name.ToLower() == request.Name.ToLower(),
                      cancellationToken);

        if (isUserWithUserNameExists)
        {
            throw new ConflictException(ErrorTemplates.UniqueUsernameViolated);
        }

        _mapper.Map(request, updatingUser);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
