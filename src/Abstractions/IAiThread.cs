using OpenAI;

namespace AiCalendarTelegramBot.Abstractions;

public interface IAiThread
{
    public Task<IAiMessage> WriteMessage(string text, Role role);
    ValueTask AppendMessage(string text, Role role);
}