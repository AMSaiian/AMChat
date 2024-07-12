using AMChat.Hubs.Common.Models.HubUser;

namespace AMChat.Hubs.Common.Interfaces;

public interface IChatCache
{
    public void AddUserConnectionToChat(string chatId, string userId, string connectionId);

    public void DeleteUserConnectionFromChat(string chatId, string userId, string connectionId);

    public List<HubUser> GetChatConnections(string chatId);

    public void DeleteChatConnections(string chatId);

    public List<(string ChatId, HubUser ChatConnections)> GetAllUserChatConnections(string userId);

    public void DeleteAllUserConnections(string userId);
}
