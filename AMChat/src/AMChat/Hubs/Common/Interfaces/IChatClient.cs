using AMChat.Application.Common.Models.Message;

namespace AMChat.Hubs.Common.Interfaces;

public interface IChatClient
{
    public Task ReceiveMessages(List<MessageDto> messages);
}
