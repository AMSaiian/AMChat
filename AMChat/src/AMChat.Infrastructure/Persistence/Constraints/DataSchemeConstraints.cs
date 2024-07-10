namespace AMChat.Infrastructure.Persistence.Constraints;

public static class DataSchemeConstraints
{
    public static readonly string SchemeName = "amchat";

    public static readonly string KeyGenerationExtensionName = "uuid-ossp";
    public static readonly string KeyType = "uuid";
    public static readonly string KeyTypeDefaultValue = "uuid_generate_v4()";

    public static readonly int UsernameLength = 100;
    public static readonly int FullnameLength = 200;
    public static readonly int ProfileDescription = 1500;

    public static readonly int ChatNameLength = 100;
    public static readonly int ChatDescriptionLength = 1000;

    public static readonly int MessageTextLength = 500;

    public static readonly int MinimalUserAge = 14;
}
