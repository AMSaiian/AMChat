using System.Security.Claims;
using AMChat.Application.Common.Interfaces;
using UnauthorizedAccessException = AMChat.Application.Common.Exceptions.UnauthorizedAccessException;

namespace AMChat.Common.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor)
    : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public Guid? UserId
    {
        get
        {
            bool isParsed = Guid
                .TryParse(_httpContextAccessor.HttpContext?.User
                              .Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value,
                          out Guid userId);

            return isParsed
                ? userId
                : null;
        }
    }

    public Guid GetUserIdOrThrow()
    {
        bool isParsed = Guid
            .TryParse(_httpContextAccessor.HttpContext?.User
                          .Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value,
                      out Guid userId);

        return isParsed
            ? userId
            : throw new UnauthorizedAccessException();
    }
}
