﻿using AMChat.Application.Common.Interfaces;
using AMChat.Application.Common.Models.Message;
using AMChat.Hubs.Chat;
using AMChat.Hubs.Common.Interfaces;
using AMChat.Hubs.Common.Models.HubUser;
using Microsoft.AspNetCore.SignalR;

namespace AMChat.Services;

public class ChatService(IHubContext<ChatHub, IChatClient> hub,
                         IChatCache chatCache)
    : IChatService
{
    private readonly IHubContext<ChatHub, IChatClient> _hub = hub;
    private readonly IChatCache _chatCache = chatCache;

    public async Task SendMessagesAsync(string chatId, List<MessageDto> message)
    {
        await _hub.Clients
            .Groups(chatId)
            .ReceiveMessages(message);
    }

    public async Task ConnectToChatAsync(string chatId, string userId, string connectionId)
    {
        _chatCache.AddUserConnectionToChat(chatId,
                                           userId,
                                           connectionId);

        await _hub.Groups.AddToGroupAsync(connectionId, chatId);
    }

    public async Task DisconnectFromChatAsync(string chatId, string userId, string connectionId)
    {
        _chatCache.DeleteUserConnectionFromChat(chatId,
                                                userId,
                                                connectionId);

        await _hub.Groups.RemoveFromGroupAsync(connectionId, chatId);
    }

    public async Task DisconnectFromAllChatsAsync(string userId)
    {
        List<(string ChatId, HubUser ChatConnections)> userConnections =
            _chatCache.GetAllUserChatConnections(userId);

        foreach (var userConnectionsToChat in userConnections)
        {
            foreach (string connectionId in userConnectionsToChat.ChatConnections.ConnectionIds)
            {
                await _hub.Groups.RemoveFromGroupAsync(connectionId, userConnectionsToChat.ChatId);
            }
        }

        _chatCache.DeleteAllUserConnections(userId);
    }

    public async Task DisconnectAllUsersFromChat(string chatId)
    {
        List<HubUser> usersConnections = _chatCache.GetChatConnections(chatId);

        foreach (string connectionId in usersConnections.SelectMany(
                     userConnections => userConnections.ConnectionIds))
        {
            await _hub.Groups.RemoveFromGroupAsync(connectionId, chatId);
        }

        _chatCache.DeleteChatConnections(chatId);
    }
}
