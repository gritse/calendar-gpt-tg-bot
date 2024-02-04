namespace AiCalendarTelegramBot.Abstractions;

public interface IAiAssistant
{
    public ValueTask<IAiThread> CreateThread();
}