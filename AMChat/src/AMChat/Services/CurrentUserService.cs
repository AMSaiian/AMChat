using AMChat.Application.Common.Interfaces;

namespace AMChat.Services;

public class CurrentUserService : ICurrentUserService
{
    public Guid? UserId
    {
        get
        {
            return Guid.Empty;
        }
    }

    public Guid GetUserIdOrThrow()
    {
        return Guid.Empty;
    }
}
