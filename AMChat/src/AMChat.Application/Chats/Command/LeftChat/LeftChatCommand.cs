using AMChat.Application.Common.Exceptions;
using AMChat.Application.Common.Interfaces;
using AMChat.Application.Common.Templates;
using AMChat.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AMChat.Application.Chats.Command.LeftChat;

public record LeftChatCommand : IRequest
{
    public required Guid UserId { get; init; }
    public required Guid ChatId { get; init; }
}

public class LeftChatHandler(IAppDbContext dbContext,
                             ICurrentUserService currentUser)
    : IRequestHandler<LeftChatCommand>
{
    private readonly IAppDbContext _dbContext = dbContext;
    private readonly ICurrentUserService _currentUser = currentUser;

    public async Task Handle(LeftChatCommand request, CancellationToken cancellationToken)
    {
        cancellationToken = CancellationToken.None;

        Guid currentUserId = _currentUser.GetUserIdOrThrow();

        bool isChatExists = await _dbContext.Chats
            .AnyAsync(chat => chat.Id == request.ChatId,
                      cancellationToken);

        if (!isChatExists)
        {
            throw new NotFoundException(
                string.Format(ErrorTemplates.EntityNotFoundFormat,
                              nameof(Chat)));

        }

        User userToLeft = await _dbContext.Users
                             .FindAsync(request.UserId,
                                        cancellationToken)
                      ?? throw new NotFoundException(
                             string.Format(ErrorTemplates.EntityNotFoundFormat,
                                           nameof(User)));

        if (currentUserId != userToLeft.Id)
        {
            throw new ForbiddenAccessException(
                string.Format(ErrorTemplates.ForbiddenManipulateEntitiesNotOwnedFormat,
                              nameof(User)));
        }

        Chat chatToLeft = await _dbContext.Users
                              .Entry(userToLeft)
                              .Collection(user => user.JoinedChats)
                              .Query()
                              .FirstOrDefaultAsync(chat => chat.Id == request.ChatId,
                                                           cancellationToken)
                       ?? await _dbContext.Users
                              .Entry(userToLeft)
                              .Collection(user => user.OwnedChats)
                              .Query()
                              .FirstOrDefaultAsync(chat => chat.Id == request.ChatId,
                                                   cancellationToken)
                       ?? throw new ConflictException(ErrorTemplates.LeftFromNotJoinedChatError);

        Message userLeftChat = new()
        {
            Text = string.Format(SystemMessageTemplates.LeftTheChatMessage, userToLeft.Name)
        };

        if (chatToLeft.OwnerId == userToLeft.Id)
        {
            User? newOwner = await _dbContext.Chats
                .Entry(chatToLeft)
                .Collection(chat => chat.JoinedUsers)
                .Query()
                .OrderBy(user => user.Name)
                .FirstOrDefaultAsync(user => user.Id != userToLeft.Id,
                                     cancellationToken);

            if (newOwner is not null)
            {
                chatToLeft.Owner = newOwner;
                chatToLeft.JoinedUsers.Remove(newOwner);
                chatToLeft.Messages.Add(userLeftChat);
            }
            else
            {
                _dbContext.Chats.Remove(chatToLeft);
            }
        }
        else
        {
            userToLeft.JoinedChats.Remove(chatToLeft);
            chatToLeft.Messages.Add(userLeftChat);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
