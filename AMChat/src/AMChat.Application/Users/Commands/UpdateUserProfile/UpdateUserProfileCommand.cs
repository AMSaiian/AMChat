using AMChat.Application.Common.Exceptions;
using AMChat.Application.Common.Interfaces;
using AMChat.Application.Common.Templates;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Profile = AMChat.Core.Entities.Profile;

namespace AMChat.Application.Users.Commands.UpdateUserProfile;

public record UpdateUserProfileCommand : IRequest
{
    public Guid UserId { get; init; }
    public string? Fullname { get; init; }
    public DateOnly? Birthdate { get; init; }
    public string? Description { get; init; }
    public bool HasDescription { get; init; } = false;
}

public class UpdateUserProfileHandler(IAppDbContext dbContext,
                                      IMapper mapper,
                                      ICurrentUserService currentUser)
    : IRequestHandler<UpdateUserProfileCommand>
{
    private readonly IAppDbContext _dbContext = dbContext;
    private readonly IMapper _mapper = mapper;
    private readonly ICurrentUserService _currentUser = currentUser;

    public async Task Handle(UpdateUserProfileCommand request,
                             CancellationToken cancellationToken)
    {
        cancellationToken = CancellationToken.None;

        Guid currentUserId = _currentUser.GetUserIdOrThrow();

        if (currentUserId != request.UserId)
        {
            throw new ForbiddenAccessException(ErrorTemplates.ForbiddenUpdateNotOwnedUser);
        }

        Profile updatingProfile = await _dbContext.Profiles
                                     .FirstOrDefaultAsync(entity => entity.UserId == request.UserId,
                                                          cancellationToken)
                              ?? throw new NotFoundException(
                                     string.Format(ErrorTemplates.EntityNotFoundFormat,
                                                   nameof(Profile)));

        _mapper.Map(request, updatingProfile);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
