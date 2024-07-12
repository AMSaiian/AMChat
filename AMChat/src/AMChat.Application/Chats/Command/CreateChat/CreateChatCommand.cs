using AMChat.Application.Common.Exceptions;
using AMChat.Application.Common.Interfaces;
using AMChat.Application.Common.Templates;
using AMChat.Core.Entities;
using AutoMapper;
using MediatR;

namespace AMChat.Application.Chats.Command.CreateChat;

public record CreateChatCommand : IRequest<Guid>
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required Guid OwnerId { get; init; }
}

public class CreateChatHandler(IAppDbContext dbContext,
                               IMapper mapper,
                               ICurrentUserService currentUser)
    : IRequestHandler<CreateChatCommand, Guid>
{
    private readonly IAppDbContext _dbContext = dbContext;
    private readonly IMapper _mapper = mapper;
    private readonly ICurrentUserService _currentUser = currentUser;

    public async Task<Guid> Handle(CreateChatCommand request,
                                   CancellationToken cancellationToken)
    {
        cancellationToken = CancellationToken.None;

        Guid currentUserId = _currentUser.GetUserIdOrThrow();

        User ownerUser = await _dbContext.Users
                                .FindAsync(request.OwnerId, cancellationToken)
                         ?? throw new NotFoundException(
                                string.Format(ErrorTemplates.EntityNotFoundFormat,
                                              nameof(User)));

        if (currentUserId != ownerUser.Id)
        {
            throw new ForbiddenAccessException(
                string.Format(ErrorTemplates.ForbiddenManipulateEntitiesNotOwnedFormat,
                              nameof(User)));
        }

        Chat newChat = _mapper.Map<Chat>(request);

        await _dbContext.Chats.AddAsync(newChat, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return newChat.Id;
    }
}
