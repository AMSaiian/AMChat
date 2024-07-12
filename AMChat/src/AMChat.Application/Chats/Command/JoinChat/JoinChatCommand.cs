using AMChat.Application.Common.Exceptions;
using AMChat.Application.Common.Interfaces;
using AMChat.Application.Common.Models.Message;
using AMChat.Application.Common.Templates;
using AMChat.Core.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AMChat.Application.Chats.Command.JoinChat;

public record JoinChatCommand : IRequest
{
    public required Guid UserId { get; init; }
    public required Guid ChatId { get; init; }
}

public class JoinChatHandler(IAppDbContext dbContext,
                             ICurrentUserService currentUser,
                             IMapper mapper,
                             IChatService chatService)
    : IRequestHandler<JoinChatCommand>
{
    private readonly IAppDbContext _dbContext = dbContext;
    private readonly ICurrentUserService _currentUser = currentUser;
    private readonly IChatService _chatService = chatService;
    private readonly IMapper _mapper = mapper;

    public async Task Handle(JoinChatCommand request, CancellationToken cancellationToken)
    {
        cancellationToken = CancellationToken.None;

        Guid currentUserId = _currentUser.GetUserIdOrThrow();

        Chat chatToAdd = await _dbContext.Chats
                             .FindAsync(request.ChatId,
                                        cancellationToken)
                      ?? throw new NotFoundException(
                             string.Format(ErrorTemplates.EntityNotFoundFormat,
                                           nameof(Chat)));

        User userToAdd = await _dbContext.Users
                             .FindAsync(request.UserId,
                                        cancellationToken)
                      ?? throw new NotFoundException(
                             string.Format(ErrorTemplates.EntityNotFoundFormat,
                                           nameof(User)));

        if (currentUserId != userToAdd.Id)
        {
            throw new ForbiddenAccessException(
                string.Format(ErrorTemplates.ForbiddenManipulateEntitiesNotOwnedFormat,
                              nameof(User)));
        }

        bool isAlreadyJoined = await _dbContext.Users
                                   .Entry(userToAdd)
                                   .Collection(user => user.JoinedChats)
                                   .Query()
                                   .AnyAsync(chat => chat.Id == request.ChatId,
                                             cancellationToken)
                            || await _dbContext.Users
                                   .Entry(userToAdd)
                                   .Collection(user => user.OwnedChats)
                                   .Query()
                                   .AnyAsync(chat => chat.Id == request.ChatId,
                                             cancellationToken);

        if (isAlreadyJoined)
        {
            throw new ConflictException(ErrorTemplates.JoinSameChatError);
        }

        userToAdd.JoinedChats.Add(chatToAdd);

        Message userJoinChat = new()
        {
            Text = string.Format(SystemMessageTemplates.JoinToChatMessage, userToAdd.Name)
        };

        chatToAdd.Messages.Add(userJoinChat);

        await _dbContext.SaveChangesAsync(cancellationToken);

        MessageDto userJoinChatDto = _mapper.Map<MessageDto>(userJoinChat);
        await _chatService.SendMessagesAsync(request.ChatId.ToString(), [userJoinChatDto]);
    }
}
