using System.Collections.Concurrent;
using AMChat.Hubs.Common.Interfaces;
using AMChat.Hubs.Common.Models.HubUser;

namespace AMChat.Hubs.Common.Cache;

public class ChatCache : IChatCache
{
    private readonly ConcurrentDictionary<string, List<HubUser>> _chatsConnections = new();

    public void AddUserConnectionToChat(string chatId, string userId, string connectionId)
    {
        List<HubUser> chatConnections = _chatsConnections.GetOrAdd(chatId, user => []);

        HubUser? userConnections = chatConnections
            .FirstOrDefault(user => user.Id == userId);

        if (userConnections is null)
        {
            chatConnections.Add(new()
            {
                Id = userId,
                ConnectionIds = [connectionId]
            });
        }
        else
        {
            userConnections.ConnectionIds.Add(connectionId);
        }
    }

    public void DeleteUserConnectionFromChat(string chatId, string userId, string connectionId)
    {
        _chatsConnections.TryGetValue(chatId, out List<HubUser>? chatConnections);

        if (chatConnections is null)
        {
            return;
        }

        HubUser? userConnections = chatConnections
            .FirstOrDefault(user => user.Id == userId);

        if (userConnections is null)
        {
            return;
        }

        userConnections.ConnectionIds.Remove(connectionId);

        if (userConnections.ConnectionIds.Count == 0)
        {
            chatConnections.RemoveAll(user => user.Id == userId);
        }

        if (_chatsConnections[chatId].Count == 0)
        {
            _chatsConnections.Remove(chatId, out _);
        }
    }

    public void DeleteUserConnectionFromAllChats(string userId, string connectionId)
    {
        foreach (string chatId in _chatsConnections.Keys)
        {
            DeleteUserConnectionFromChat(chatId, userId, connectionId);
        }
    }

    public List<HubUser>? GetChatConnections(string chatId)
    {
        return _chatsConnections.TryGetValue(chatId, out List<HubUser>? connections)
            ? connections
            : null;
    }

    public void DeleteChatConnections(string chatId) => _chatsConnections.Remove(chatId, out _);

    public List<(string ChatId, HubUser ChatConnections)> GetAllUserConnections(string userId)
    {
        var result = _chatsConnections
            .Where(pair => pair.Value.Exists(userConnection => userConnection.Id == userId))
            .Select(pair => (ChatId: pair.Key,
                             UserConnection: pair.Value.First(user => user.Id == userId)))
            .ToList();

        return result;
    }

    public void DeleteAllUserConnections(string userId)
    {
        foreach (var chatConnections in _chatsConnections)
        {
            chatConnections.Value.RemoveAll(user => user.Id == userId);

            if (chatConnections.Value.Count == 0)
            {
                _chatsConnections.Remove(chatConnections.Key, out _);
            }
        }
    }

    public HubUser? GetAllUserChatConnections(string userId, string chatId)
    {
        _chatsConnections.TryGetValue(chatId, out List<HubUser>? users);

        var result = users?.FirstOrDefault(user => user.Id == userId);

        return result;
    }

    public void DeleteAllUserChatConnections(string userId, string chatId)
    {
        _chatsConnections.TryGetValue(chatId, out List<HubUser>? chatConnections);

        if (chatConnections is null)
        {
            return;
        }

        chatConnections.RemoveAll(user => user.Id == userId);

        if (_chatsConnections[chatId].Count == 0)
        {
            _chatsConnections.Remove(chatId, out _);
        }
    }
}
