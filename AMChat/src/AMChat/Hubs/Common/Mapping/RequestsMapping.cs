
using AMChat.Application.Chats.Command.SendMessage;
using AMChat.Hubs.Common.Requests.Chats;
using AutoMapper;

namespace AMChat.Hubs.Common.Mapping;

public sealed class RequestsMapping : Profile
{
    public RequestsMapping()
    {
        CreateMap<SendMessageRequest, SendMessageCommand>();
    }
}
