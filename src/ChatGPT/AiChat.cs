using AiCalendarTelegramBot.Abstractions;

namespace AiCalendarTelegramBot.ChatGPT;

/// <summary>
/// Dumb solution, but cheaper
/// </summary>
/// <param name="openAiKey">Open Ai token</param>
public class AiChat(string openAiKey) : IAiAssistant
{
    public ValueTask<IAiThread> CreateThread()
    {
        var aiChatThread = new AiChatThread(openAiKey);
        return ValueTask.FromResult<IAiThread>(aiChatThread);
    }
}