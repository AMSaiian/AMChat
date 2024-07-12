using System.Security.Claims;
using AMChat.Application.Common.Interfaces;

namespace AMChat.Services;

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
