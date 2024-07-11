using AMChat.Application.Common.Exceptions;
using AMChat.Application.Common.Interfaces;
using AMChat.Application.Common.Templates;
using AMChat.Core.Entities;
using MediatR;

namespace AMChat.Application.Chats.Command.DeleteChat;

public record DeleteChatCommand : IRequest
{
    public required Guid Id { get; init; }
}

public class DeleteChatHandler(IAppDbContext dbContext,
                               ICurrentUserService currentUser) : IRequestHandler<DeleteChatCommand>
{
    private readonly IAppDbContext _dbContext = dbContext;
    private readonly ICurrentUserService _currentUser = currentUser;

    public async Task Handle(DeleteChatCommand request,
                             CancellationToken cancellationToken)
    {
        cancellationToken = CancellationToken.None;

        Guid currentUserId = _currentUser.GetUserIdOrThrow();

        Chat deletingChat = await _dbContext.Chats
                                .FindAsync(request.Id,
                                           cancellationToken)
                         ?? throw new NotFoundException(
                                string.Format(ErrorTemplates.EntityNotFoundFormat,
                                              nameof(Chat)));

        if (currentUserId != deletingChat.OwnerId)
        {
            throw new ForbiddenAccessException(
                string.Format(ErrorTemplates.ForbiddenDeleteNotOwnedResourceFormat,
                              nameof(Chat)));
        }

        _dbContext.Chats.Remove(deletingChat);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
