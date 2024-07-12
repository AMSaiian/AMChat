using AMChat.Hubs.Common.Models.HubUser;

namespace AMChat.Hubs.Common.Interfaces;

public interface IChatCache
{
    public void AddUserConnectionToChat(string chatId, string userId, string connectionId);

    public void DeleteUserConnectionFromChat(string chatId, string userId, string connectionId);

    public List<HubUser>? GetChatConnections(string chatId);

    public void DeleteChatConnections(string chatId);

    public void DeleteUserConnectionFromAllChats(string userId, string connectionId);

    public List<(string ChatId, HubUser ChatConnections)> GetAllUserConnections(string userId);

    public void DeleteAllUserConnections(string userId);

    public HubUser? GetAllUserChatConnections(string userId, string chatId);

    public void DeleteAllUserChatConnections(string userId, string chatId);
}
