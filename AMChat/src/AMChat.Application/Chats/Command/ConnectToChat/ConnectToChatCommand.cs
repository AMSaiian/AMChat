using AMChat.Application.Common.Exceptions;
using AMChat.Application.Common.Interfaces;
using AMChat.Application.Common.Models.Message;
using AMChat.Application.Common.Templates;
using AMChat.Core.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AMChat.Application.Chats.Command.ConnectToChat;

public record ConnectToChatCommand : IRequest
{
    public required Guid UserId { get; init; }
    public required Guid ChatId { get; init; }
    public required string ConnectionId { get; init; }
}

public class ConnectToChatHandler(IAppDbContext dbContext,
                                  IChatService chatService,
                                  IMapper mapper)
    : IRequestHandler<ConnectToChatCommand>
{
    private readonly IChatService _chatService = chatService;
    private readonly IAppDbContext _dbContext = dbContext;
    private readonly IMapper _mapper = mapper;

    public async Task Handle(ConnectToChatCommand request,
                             CancellationToken cancellationToken)
    {
        cancellationToken = CancellationToken.None;

        Chat connectingChat = await _dbContext.Chats
                        .AsNoTracking()
                        .FirstOrDefaultAsync(cancellationToken)
                 ?? throw new NotFoundException(
                        string.Format(ErrorTemplates.EntityNotFoundFormat,
                                      nameof(Chat)));

        await _chatService.ConnectToChatAsync(request.ChatId.ToString(),
                                              request.UserId.ToString(),
                                              request.ConnectionId);

        List<Message> previousMessages = await _dbContext.Chats
            .Entry(connectingChat)
            .Collection(chat => chat.Messages)
            .Query()
            .Include(message => message.Author)
            .AsNoTracking()
            .OrderBy(message => message.PostedTime)
            .ToListAsync(cancellationToken);

        List<MessageDto> previousMessagesDto = _mapper.Map<List<MessageDto>>(previousMessages);

        await _chatService.SendMessagesToConnectionAsync(request.ConnectionId,
                                                         previousMessagesDto);
    }
}
