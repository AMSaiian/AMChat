using AMChat.Application.Common.Exceptions;
using AMChat.Application.Common.Interfaces;
using AMChat.Application.Common.Models.Chat;
using AMChat.Application.Common.Templates;
using AMChat.Core.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AMChat.Application.Chats.Queries.GetChatById;

public record GetChatByIdQuery : IRequest<ChatDetailedDto>
{
    public required Guid Id { get; init; }
}

public class GetChatByIdHandler(IAppDbContext dbContext,
                                IMapper mapper)
    : IRequestHandler<GetChatByIdQuery, ChatDetailedDto>
{
    private readonly IAppDbContext _dbContext = dbContext;
    private readonly IMapper _mapper = mapper;

    public async Task<ChatDetailedDto> Handle(GetChatByIdQuery request,
                                              CancellationToken cancellationToken)
    {
        Chat needChat = await _dbContext.Chats
                            .AsNoTracking()
                            .Include(chat => chat.JoinedUsers)
                            .Include(chat => chat.Owner)
                            .FirstOrDefaultAsync(user => user.Id == request.Id,
                                                 cancellationToken)
                     ?? throw new NotFoundException(
                            string.Format(ErrorTemplates.EntityNotFoundFormat,
                                          nameof(Chat)));

        ChatDetailedDto needChatDto = _mapper.Map<ChatDetailedDto>(needChat);

        return needChatDto;
    }
}
