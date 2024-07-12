using AMChat.Application.Common.Exceptions;
using AMChat.Application.Common.Interfaces;
using AMChat.Application.Common.Templates;
using AMChat.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AMChat.Application.Chats.Queries.IsUserJoined;

public record IsUserJoinedChatQuery : IRequest<bool>
{
    public required Guid UserId { get; init; }
    public required Guid ChatId { get; init; }
}

public class IsUserJoinedChatHandler(IAppDbContext dbContext,
                                     ICurrentUserService currentUser)
    : IRequestHandler<IsUserJoinedChatQuery, bool>
{
    private readonly IAppDbContext _dbContext = dbContext;
    private readonly ICurrentUserService _currentUser = currentUser;

    public async Task<bool> Handle(IsUserJoinedChatQuery request,
                                   CancellationToken cancellationToken)
    {
        cancellationToken = CancellationToken.None;

        Guid currentUserId = _currentUser.GetUserIdOrThrow();

        User challengingUser = await _dbContext.Users
                                .Include(user => user.OwnedChats)
                                .Include(user => user.JoinedChats)
                                .AsNoTracking()
                                .FirstOrDefaultAsync(user => user.Id == request.UserId,
                                                     cancellationToken)
                         ?? throw new NotFoundException(
                                string.Format(ErrorTemplates.EntityNotFoundFormat,
                                              nameof(User)));

        if (currentUserId != challengingUser.Id)
        {
            throw new ForbiddenAccessException(
                string.Format(ErrorTemplates.ForbiddenDeleteNotOwnedResourceFormat,
                              nameof(User)));
        }

        bool isChatExists = await _dbContext.Chats
            .AsNoTracking()
            .AnyAsync(chat => chat.Id == request.ChatId,
                      cancellationToken);

        if (!isChatExists)
        {
            throw new NotFoundException(
                string.Format(ErrorTemplates.EntityNotFoundFormat,
                              nameof(Chat)));

        }

        bool isJoined = await _dbContext.Users
                            .Entry(challengingUser)
                            .Collection(user => user.JoinedChats)
                            .Query()
                            .AsNoTracking()
                            .AnyAsync(chat => chat.Id == request.ChatId,
                                      cancellationToken)
                     || await _dbContext.Users
                            .Entry(challengingUser)
                            .Collection(user => user.OwnedChats)
                            .Query()
                            .AsNoTracking()
                            .AnyAsync(chat => chat.Id == request.ChatId,
                                      cancellationToken);

        return isJoined;
    }
}
