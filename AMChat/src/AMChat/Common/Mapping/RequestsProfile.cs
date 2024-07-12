using AMChat.Application.Chats.Command.CreateChat;
using AMChat.Application.Chats.Command.UpdateChat;
using AMChat.Application.Users.Commands.CreateUser;
using AMChat.Application.Users.Commands.UpdateUser;
using AMChat.Common.Contract.Requests.Chats;
using AMChat.Common.Contract.Requests.Users;
using AutoMapper;

namespace AMChat.Common.Mapping;

public sealed class RequestsProfile : Profile
{
    public RequestsProfile()
    {
        CreateMap<CreateUserRequest, CreateUserCommand>();
        CreateMap<UpdateUserRequest, UpdateUserCommand>();
        CreateMap<UpdateUserProfileRequest, UpdateUserCommand>();

        CreateMap<CreateChatRequest, CreateChatCommand>();
        CreateMap<UpdateChatRequest, UpdateChatCommand>();
    }
}
