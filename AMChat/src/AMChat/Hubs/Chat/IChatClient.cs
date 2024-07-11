using AMChat.Application.Common.Models.Message;
using AMChat.Hubs.Common.Models.Result;

namespace AMChat.Hubs.Chat;

public interface IChatClient
{
    public Task ReceiveMessage(Guid chatId, MessageDto message);
}
