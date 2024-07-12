using AMChat.Application.Common.Exceptions;
using AMChat.Application.Common.Interfaces;
using AMChat.Application.Common.Templates;
using AMChat.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AMChat.Application.Users.Commands.DeleteUser;

public record DeleteUserCommand : IRequest
{
    public Guid Id { get; init; }
}

public record DeleteUserHandler(IAppDbContext dbContext,
                                ICurrentUserService currentUser,
                                IChatService chatService)
    : IRequestHandler<DeleteUserCommand>
{
    private readonly IAppDbContext _dbContext = dbContext;
    private readonly ICurrentUserService _currentUser = currentUser;
    private readonly IChatService _chatService = chatService;

    public async Task Handle(DeleteUserCommand request,
                             CancellationToken cancellationToken)
    {
        cancellationToken = CancellationToken.None;

        Guid currentUserId = _currentUser.GetUserIdOrThrow();

        User deletingUser = await _dbContext.Users
                                .Include(user => user.OwnedChats)
                                .Include(user => user.JoinedChats)
                                .FirstOrDefaultAsync(user => user.Id == request.Id,
                                                     cancellationToken)
                         ?? throw new NotFoundException(
                                string.Format(ErrorTemplates.EntityNotFoundFormat,
                                              nameof(User)));

        if (currentUserId != deletingUser.Id)
        {
            throw new ForbiddenAccessException(
                string.Format(ErrorTemplates.ForbiddenDeleteNotOwnedResourceFormat,
                              nameof(User)));
        }

        deletingUser.JoinedChats.Clear();

        foreach (Chat ownedChat in deletingUser.OwnedChats)
        {
            User? newOwner = await _dbContext.Chats
                .Entry(ownedChat)
                .Collection(chat => chat.JoinedUsers)
                .Query()
                .OrderBy(user => user.Name)
                .FirstOrDefaultAsync(user => user.Id != deletingUser.Id,
                                     cancellationToken);

            if (newOwner is not null)
            {
                ownedChat.Owner = newOwner;
                ownedChat.JoinedUsers.Remove(newOwner);
            }
            else
            {
                _dbContext.Chats.Remove(ownedChat);
            }
        }

        _dbContext.Users.Remove(deletingUser);

        await _dbContext.SaveChangesAsync(cancellationToken);

        await _chatService.DisconnectFromAllChatsAsync(request.Id.ToString());
    }
}
