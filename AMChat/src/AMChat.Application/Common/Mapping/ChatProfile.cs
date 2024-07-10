using AMChat.Application.Chats.Queries.GetPaginatedChats;
using AMChat.Application.Common.Models.Chat;
using AMChat.Core.Entities;
using Profile = AutoMapper.Profile;

namespace AMChat.Application.Common.Mapping;

public sealed class ChatProfile : Profile
{
    public ChatProfile()
    {
        CreateMap<Chat, ChatDto>();
    }
}
