using AMChat.Application.Common.Exceptions;
using AMChat.Application.Common.Interfaces;
using AMChat.Application.Common.Templates;
using AMChat.Core.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AMChat.Application.Chats.Command.UpdateChat;

public record UpdateChatCommand : IRequest
{
    public required Guid Id { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public bool HasDescription { get; init; } = false;
    public Guid? OwnerId { get; init; }
}

public class UpdateChatHandler(IAppDbContext dbContext,
                               IMapper mapper,
                               ICurrentUserService currentUser) : IRequestHandler<UpdateChatCommand>
{
    private readonly IAppDbContext _dbContext = dbContext;
    private readonly IMapper _mapper = mapper;
    private readonly ICurrentUserService _currentUser = currentUser;

    public async Task Handle(UpdateChatCommand request, CancellationToken cancellationToken)
    {
        cancellationToken = CancellationToken.None;

        Guid currentUserId = _currentUser.GetUserIdOrThrow();

        Chat updatingChat = await _dbContext.Chats
                                .FindAsync(request.Id, cancellationToken)
                         ?? throw new NotFoundException(
                                string.Format(ErrorTemplates.EntityNotFoundFormat,
                                              nameof(Chat)));

        if (currentUserId != updatingChat.OwnerId)
        {
            throw new ForbiddenAccessException(
                string.Format(ErrorTemplates.ForbiddenUpdateNotOwnedResourceFormat,
                              nameof(Chat)));
        }

        bool isNewOwnerExists = await _dbContext.Users
            .AnyAsync(user => user.Id == request.OwnerId,
                      cancellationToken);

        if (!isNewOwnerExists)
        {
            throw new NotFoundException(
                string.Format(ErrorTemplates.EntityNotFoundFormat,
                              nameof(User)));
        }

        if (request.OwnerId == updatingChat.OwnerId)
        {
            throw new ConflictException(ErrorTemplates.SameChatOwnerChange);
        }

        _mapper.Map(request, updatingChat);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
