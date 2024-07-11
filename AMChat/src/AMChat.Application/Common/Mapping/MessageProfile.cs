using AMChat.Application.Common.Models.Message;
using AMChat.Core.Entities;
using Profile = AutoMapper.Profile;

namespace AMChat.Application.Common.Mapping;

public sealed class MessageProfile : Profile
{
    public MessageProfile()
    {
        CreateMap<Message, MessageDto>()
            .ReverseMap();
    }
}
