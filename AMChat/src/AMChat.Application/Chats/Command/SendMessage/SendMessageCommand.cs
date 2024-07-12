using AMChat.Application.Common.Exceptions;
using AMChat.Application.Common.Interfaces;
using AMChat.Application.Common.Models.Message;
using AMChat.Application.Common.Templates;
using AMChat.Core.Entities;
using AMChat.Core.Enums;
using AutoMapper;
using MediatR;

namespace AMChat.Application.Chats.Command.SendMessage;

public record SendMessageCommand : IRequest<Guid>
{
    public required Guid ChatId { get; init; }
    public required Guid AuthorId { get; init; }
    public required string Text { get; init; }
}

public class SendMessageHandler(IAppDbContext dbContext,
                                IChatService chatService,
                                ICurrentUserService currentUser,
                                IMapper mapper)
    : IRequestHandler<SendMessageCommand, Guid>
{
    private readonly IChatService _chatService = chatService;
    private readonly IAppDbContext _dbContext = dbContext;
    private readonly IMapper _mapper = mapper;
    private readonly ICurrentUserService _currentUser = currentUser;

    public async Task<Guid> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        cancellationToken = CancellationToken.None;

        Guid currentUserId = _currentUser.GetUserIdOrThrow();

        Chat sendingChat = await _dbContext.Chats
            .FindAsync(request.ChatId, cancellationToken)
                        ?? throw new NotFoundException(
                               string.Format(ErrorTemplates.EntityNotFoundFormat,
                                             nameof(Chat)));

        User sender = await _dbContext.Users
            .FindAsync(request.AuthorId, cancellationToken)
                   ?? throw new NotFoundException(
                          string.Format(ErrorTemplates.EntityNotFoundFormat,
                                        nameof(User)));

        if (currentUserId != sender.Id)
        {
            throw new ForbiddenAccessException(
                string.Format(ErrorTemplates.ForbiddenManipulateEntitiesNotOwnedFormat,
                              nameof(User)));
        }

        Message newMessage = _mapper.Map<Message>(request);
        newMessage.Kind = MessageKind.User;

        sendingChat.Messages.Add(newMessage);

        await _dbContext.SaveChangesAsync(cancellationToken);

        MessageDto newMessageDto = _mapper.Map<MessageDto>(newMessage);

        await _chatService.SendMessagesAsync(request.ChatId.ToString(), [newMessageDto]);

        return newMessage.Id;
    }
}

