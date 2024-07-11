using AMChat.Application.Common.Interfaces;

namespace AMChat.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor)
    : ICurrentUserService
{
    private static readonly string UserIdHeaderName = CustomHeaders.UserIdHeader;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public Guid? UserId
    {
        get
        {
            bool isParsed = Guid
                .TryParse(_httpContextAccessor.HttpContext?.Request.Headers[UserIdHeaderName],
                          out Guid header);

            return isParsed
                ? header
                : null;
        }
    }

    public Guid GetUserIdOrThrow()
    {
        bool isParsed = Guid
            .TryParse(_httpContextAccessor.HttpContext?.Request.Headers[UserIdHeaderName],
                      out Guid header);

        return isParsed
            ? header
            : throw new UnauthorizedAccessException();
    }
}
