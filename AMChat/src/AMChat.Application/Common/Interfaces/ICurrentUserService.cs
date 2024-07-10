namespace AMChat.Application.Common.Interfaces;

public interface ICurrentUserService
{
    public Guid? UserId { get; }

    public Guid GetUserIdOrThrow();
}
