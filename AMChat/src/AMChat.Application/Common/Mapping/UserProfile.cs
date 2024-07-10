using AMChat.Application.Users.Commands.CreateUser;
using AMChat.Application.Users.Commands.UpdateUser;
using AMChat.Application.Users.Commands.UpdateUserProfile;
using AMChat.Core.Entities;

namespace AMChat.Application.Common.Mapping;

public sealed class UserProfile : AutoMapper.Profile
{
    public UserProfile()
    {
        CreateMap<User, CreateUserCommand>()
            .ReverseMap();
        CreateMap<User, UpdateUserCommand>()
            .ReverseMap();

        CreateMap<Profile, UpdateUserProfileCommand>()
            .ReverseMap()
            .ForMember(entity => entity.Description,
                       options => options.Condition(command => command.HasDescription))
            .ForMember(entity => entity.UserId,
                       options => options.Ignore());
    }
}
