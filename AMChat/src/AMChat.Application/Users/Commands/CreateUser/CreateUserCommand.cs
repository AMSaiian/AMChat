using AMChat.Application.Common.Exceptions;
using AMChat.Application.Common.Interfaces;
using AMChat.Application.Common.Templates;
using AMChat.Core.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AMChat.Application.Users.Commands.CreateUser;

public record CreateUserCommand : IRequest<Guid>
{
    public required string Name { get; init; }
    public required string ProfileFullname { get; init; }
    public required DateOnly ProfileBirthdate { get; init; }
    public string? ProfileDescription { get; init; }
}

public class CreateUserHandler(IAppDbContext dbContext,
                               IMapper mapper)
    : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IAppDbContext _dbContext = dbContext;
    private readonly IMapper _mapper = mapper;

    public async Task<Guid> Handle(CreateUserCommand request,
                                   CancellationToken cancellationToken)
    {
        cancellationToken = CancellationToken.None;

        bool isUserWithUserNameExists = await _dbContext.Users
            .AnyAsync(user => user.Name.ToLower() == request.Name.ToLower(),
                      cancellationToken);

        if (isUserWithUserNameExists)
        {
            throw new ConflictException(ErrorTemplates.UniqueUsernameViolated);
        }

        User newUser = _mapper.Map<User>(request);

        await _dbContext.Users.AddAsync(newUser,
                                        cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return newUser.Id;
    }
}
