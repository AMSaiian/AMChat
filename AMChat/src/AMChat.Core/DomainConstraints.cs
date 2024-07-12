namespace AMChat.Core;

public static class DomainConstraints
{

    public static readonly int UsernameLength = 100;
    public static readonly int FullnameLength = 200;
    public static readonly int ProfileDescription = 1500;

    public static readonly int ChatNameLength = 100;
    public static readonly int ChatDescriptionLength = 1000;

    public static readonly int MessageTextLength = 500;

    public static readonly int MinimalUserAge = 14;
}
