using AiCalendarTelegramBot.Abstractions;

namespace AiCalendarTelegramBot.ChatGPT;

public record AiMessage(string? Text) : IAiMessage;