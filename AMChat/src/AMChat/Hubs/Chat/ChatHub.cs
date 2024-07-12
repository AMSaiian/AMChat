using AMChat.Application.Chats.Command.ConnectToChat;
using AMChat.Application.Chats.Command.DisconnectFromChat;
using AMChat.Application.Chats.Command.SendMessage;
using AMChat.Application.Chats.Queries.IsUserJoined;
using AMChat.Application.Common.Interfaces;
using AMChat.Hubs.Common.Interfaces;
using AMChat.Hubs.Common.Models.Result;
using AMChat.Hubs.Common.Requests.Chats;
using AMChat.Hubs.Common.Templates;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace AMChat.Hubs.Chat;

public class ChatHub(ICurrentUserService currentUser,
                     IChatCache chatCache,
                     IMediator mediator,
                     IMapper mapper)
    : Hub<IChatClient>
{
    private readonly ICurrentUserService _currentUser = currentUser;
    private readonly IChatCache _chatCache = chatCache;
    private readonly IMediator _mediator = mediator;
    private readonly IMapper _mapper  = mapper;

    public async Task<Result> ConnectToChat(ConnectChatRequest request)
    {
        Guid currentUserId = _currentUser.GetUserIdOrThrow();

        IsUserJoinedChatQuery query = new()
        {
            ChatId = request.ChatId,
            UserId = currentUserId
        };

        bool isCanConnectToChat = await _mediator.Send(query);

        if (isCanConnectToChat)
        {
            ConnectToChatCommand command = new()
            {
                ChatId = request.ChatId,
                ConnectionId = Context.ConnectionId,
                UserId = currentUserId
            };

            await _mediator.Send(command);

            return new Result();
        }
        else
        {
            return new Result
            {
                Type = ResultType.ForbiddenError,
                Errors = [ErrorTemplates.ForbiddenConnectNotJoinedChat]
            };
        }
    }

    public async Task<Result> DisconnectFromChat(DisconnectChatRequest request)
    {
        Guid currentUserId = _currentUser.GetUserIdOrThrow();

        IsUserJoinedChatQuery query = new()
        {
            ChatId = request.ChatId,
            UserId = currentUserId
        };

        bool isCanDisconnectFromChat = await _mediator.Send(query);

        if (isCanDisconnectFromChat)
        {
            DisconnectFromChatCommand command = new()
            {
                ChatId = request.ChatId,
                ConnectionId = Context.ConnectionId,
                UserId = currentUserId
            };

            await _mediator.Send(command);

            return new Result();
        }
        else
        {
            return new Result
            {
                Type = ResultType.ForbiddenError,
                Errors = [ErrorTemplates.ForbiddenDisconnectNotJoinedChat]
            };
        }
    }

    public async Task<Result> SendMessage(SendMessageRequest request)
    {
        Guid currentUserId = _currentUser.GetUserIdOrThrow();

        IsUserJoinedChatQuery query = new()
        {
            ChatId = request.ChatId,
            UserId = currentUserId
        };

        bool isCanSendMessagesToChat = await _mediator.Send(query);

        if (isCanSendMessagesToChat)
        {
            SendMessageCommand command = _mapper.Map<SendMessageCommand>(request);

            Guid newMessageId = await _mediator.Send(command);

            return new Result
            {
                Value = newMessageId
            };
        }
        else
        {
            return new Result
            {
                Type = ResultType.ForbiddenError,
                Errors = [ErrorTemplates.ForbiddenWriteToNotJoinedChat]
            };
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string? disconnectingUserId = Context.UserIdentifier;
        string disconnectingConnectionId = Context.ConnectionId;

        if (disconnectingUserId is not null)
        {
            _chatCache.DeleteUserConnectionFromAllChats(disconnectingUserId,
                                                        disconnectingConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }
}
