namespace AMChat.Infrastructure.Persistence.Constraints;

public static class DataSchemeConstraints
{
    public static readonly string SchemeName = "amchat";

    public static readonly string KeyGenerationExtensionName = "uuid-ossp";
    public static readonly string KeyType = "uuid";
    public static readonly string KeyTypeDefaultValue = "uuid_generate_v4()";
}
