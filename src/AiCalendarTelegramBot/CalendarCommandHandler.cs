using System.Text;
using AiCalendar.Services;
using Ardalis.GuardClauses;
using OpenAI;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

// ReSharper disable MethodSupportsCancellation

namespace AiCalendarTelegramBot;

public class CalendarCommandHandler(ITelegramBotClient botClient, string openAiKey)
{
    private readonly OpenAIClient _openAiClient = new(openAiKey);

    public async ValueTask HandleUserPrompt(Message message)
    {
        var messageText = message.Text ?? message.Caption;
        Guard.Against.NullOrWhiteSpace(messageText, message: "Message text cannot be null or empty");

        var processor = new CalendarNaturalLanguageProcessor(_openAiClient);

        var cts = new CancellationTokenSource();
        var calendarTask = processor.GetCalendarResponseAsync(messageText);
        _ = calendarTask.ContinueWith(_ => cts.Cancel(), TaskContinuationOptions.ExecuteSynchronously);

        _ = Task.Run(async () =>
        {
            while (!cts.IsCancellationRequested)
            {
                await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing, cancellationToken: cts.Token);
                await Task.Delay(3000, cts.Token);
            }
        });

        var serializedCalendar = CalendarConverter.Serialize(await calendarTask);
        Guard.Against.NullOrWhiteSpace(serializedCalendar, message: "Serialized calendar cannot be null or empty");

        await botClient.SendDocumentAsync(
            message.Chat.Id,
            InputFile.FromStream(new MemoryStream(Encoding.UTF8.GetBytes(serializedCalendar)), fileName: $"calendar.ics"));
    }


}