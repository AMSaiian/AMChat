using AMChat.Application.Chats.Command.CreateChat;
using AMChat.Application.Chats.Command.UpdateChat;
using AMChat.Application.Users.Commands.CreateUser;
using AMChat.Application.Users.Commands.UpdateUser;
using AMChat.Contract.Requests.Chats;
using AMChat.Contract.Requests.Users;
using AutoMapper;

namespace AMChat.Mapping;

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
