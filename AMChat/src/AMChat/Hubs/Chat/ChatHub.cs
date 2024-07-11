using AMChat.Application.Common.Interfaces;
using AMChat.Hubs.Common.Models.Result;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace AMChat.Hubs.Chat;

public class ChatHub(ICurrentUserService currentUser,
                     IMediator mediator)
    : Hub<IChatClient>
{
    private readonly ICurrentUserService _currentUser = currentUser;
    private readonly IMediator _mediator = mediator;

    public override Task OnConnectedAsync()
    {
        var userId = _currentUser.UserId;

        return base.OnConnectedAsync();
    }

    public async Task<Result> SendMessage()
    {
        return new();
    }
}
