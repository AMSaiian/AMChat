using AMChat.Application.Common.Interfaces;
using AMChat.Application.Common.Mapping;
using AMChat.Infrastructure.Persistence;
using AMChat.Infrastructure.Persistence.Interceptors;
using AMChat.Infrastructure.Persistence.Seeding.Fakers;
using AMChat.Infrastructure.Persistence.Seeding.Initializers;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace AMChat.UnitTests.Common;

public abstract class UnitTestBase
{
    protected AppDbContext DbContext { get; private set;  }

    protected IAppDbContextInitializer AppDbContextInitializer { get; private set; }

    protected IMapper Mapper { get; }

    protected Mock<ICurrentUserService> CurrentUserServiceMoq { get; }

    protected Mock<IChatService> ChatServiceMoq { get; }

    public UnitTestBase()
    {
        DbContext = BuildDbContext();

        AppDbContextInitializer = BuildAppDbContextInitializer();

        var config = new MapperConfiguration(
            cfg =>
                cfg.AddProfiles(
                [
                    new UserProfile(),
                    new ChatProfile(),
                    new MessageProfile(),
                ]));
        Mapper = config.CreateMapper();

        CurrentUserServiceMoq = new Mock<ICurrentUserService>();

        ChatServiceMoq = new Mock<IChatService>();
    }

    protected virtual async Task SetupDbContext()
    {
        DbContext = BuildDbContext();
        AppDbContextInitializer = BuildAppDbContextInitializer();
        await AppDbContextInitializer.SeedAsync();
    }

    private AppDbContext BuildDbContext()
    {
        return new AppDbContext(
            new DbContextOptionsBuilder<AppDbContext>()
                .AddInterceptors(new SoftDeleteInterceptor())
                .UseSnakeCaseNamingConvention()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);
    }

    private AppDbContextInitializer BuildAppDbContextInitializer()
    {
        return new AppDbContextInitializer(Mock.Of<ILogger<AppDbContextInitializer>>(),
                                           DbContext,
                                           new ChatFaker(),
                                           new MessageFaker(),
                                           new ProfileFaker(),
                                           new UserFaker());
    }
}
