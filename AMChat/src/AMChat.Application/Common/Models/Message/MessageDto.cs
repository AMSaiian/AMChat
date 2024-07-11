﻿namespace AMChat.Application.Common.Models.Message;

public record MessageDto
{
    public required Guid Id { get; init; }
    public required DateTime PostedTime { get; init; }
    public required string Kind { get; init; }
    public string? AuthorName { get; init; }
}
