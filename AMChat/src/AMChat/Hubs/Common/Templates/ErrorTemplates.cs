namespace AMChat.Hubs.Common.Templates;

public static class ErrorTemplates
{
    public static readonly string ForbiddenConnectNotJoinedChat = "Can't connect to not joined chat";
    public static readonly string ForbiddenDisconnectNotJoinedChat = "Can't disconnect to not joined chat";
    public static readonly string ForbiddenWriteToNotJoinedChat = "Can't send messages to not connected chat";
}
