using AMChat.Application.Chats.Command.CreateChat;
using AMChat.Application.Chats.Command.UpdateChat;
using AMChat.Application.Common.Models.Chat;
using AMChat.Core.Entities;
using Profile = AutoMapper.Profile;

namespace AMChat.Application.Common.Mapping;

public sealed class ChatProfile : Profile
{
    public ChatProfile()
    {
        CreateMap<Chat, ChatDto>();
        CreateMap<CreateChatCommand, Chat>();
        CreateMap<Chat, UpdateChatCommand>()
            .ReverseMap()
            .ForMember(entity => entity.Description,
                       options => options.Condition(command => command.HasDescription))
            .ForMember(entity => entity.Id,
                       options => options.Ignore());
    }
}
