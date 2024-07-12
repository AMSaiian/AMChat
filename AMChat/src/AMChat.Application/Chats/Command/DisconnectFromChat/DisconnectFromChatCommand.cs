using AMChat.Application.Common.Exceptions;
using AMChat.Application.Common.Interfaces;
using AMChat.Application.Common.Templates;
using AMChat.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AMChat.Application.Chats.Command.DisconnectFromChat;

public record DisconnectFromChatCommand : IRequest
{
    public required Guid UserId { get; init; }
    public required Guid ChatId { get; init; }
    public required string ConnectionId { get; init; }
}

public class DisconnectFromChatHandler(IAppDbContext dbContext,
                                       IChatService chatService)
    : IRequestHandler<DisconnectFromChatCommand>
{
    private readonly IChatService _chatService = chatService;
    private readonly IAppDbContext _dbContext = dbContext;

    public async Task Handle(DisconnectFromChatCommand request, CancellationToken cancellationToken)
    {
        cancellationToken = CancellationToken.None;

        bool disconnectingChatExist = await _dbContext.Chats
            .AsNoTracking()
            .AnyAsync(chat => chat.Id == request.ChatId,
                      cancellationToken);

        if (!disconnectingChatExist)
        {
            throw new NotFoundException(
                string.Format(ErrorTemplates.EntityNotFoundFormat,
                              nameof(Chat)));
        }


        await _chatService.DisconnectFromChatAsync(request.ChatId.ToString(),
                                                request.UserId.ToString(),
                                                request.ConnectionId);
    }
}
