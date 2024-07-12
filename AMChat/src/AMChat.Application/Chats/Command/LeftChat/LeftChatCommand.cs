using AMChat.Application.Common.Exceptions;
using AMChat.Application.Common.Interfaces;
using AMChat.Application.Common.Models.Message;
using AMChat.Application.Common.Templates;
using AMChat.Core.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AMChat.Application.Chats.Command.LeftChat;

public record LeftChatCommand : IRequest
{
    public required Guid UserId { get; init; }
    public required Guid ChatId { get; init; }
}

public class LeftChatHandler(IAppDbContext dbContext,
                             ICurrentUserService currentUser,
                             IMapper mapper,
                             IChatService chatService)
    : IRequestHandler<LeftChatCommand>
{
    private readonly IAppDbContext _dbContext = dbContext;
    private readonly ICurrentUserService _currentUser = currentUser;
    private readonly IChatService _chatService = chatService;
    private readonly IMapper _mapper = mapper;

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

        bool isChatDeleted = false;

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
            }
            else
            {
                _dbContext.Chats.Remove(chatToLeft);
                isChatDeleted = true;
            }
        }
        else
        {
            userToLeft.JoinedChats.Remove(chatToLeft);
        }

        if (!isChatDeleted)
        {
            chatToLeft.Messages.Add(userLeftChat);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        if (!isChatDeleted)
        {
            MessageDto userLeftChatDto = _mapper.Map<MessageDto>(userLeftChat);
            await _chatService.SendMessagesAsync(request.ChatId.ToString(), [userLeftChatDto]);
        }
    }
}
