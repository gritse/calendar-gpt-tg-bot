using System.Globalization;
using System.Text;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Newtonsoft.Json;
using OpenAI;
using OpenAI.Chat;
using Telegram.Bot;
using Telegram.Bot.Types;
using Message = Telegram.Bot.Types.Message;
using ChatMessage = OpenAI.Chat.Message;
using File = System.IO.File;
using Calendar = Ical.Net.Calendar;

namespace AiCalendarTelegramBot.Handlers;

public record CalendarResponse(Event[] Events);
public record Event(string Title, string Description, string Date, string Time, string Duration, string Location);

public class CalendarCommandHandler(ITelegramBotClient botClient, string openAiKey)
{
    private readonly OpenAIClient _openAiClient = new(openAiKey);
    private const string GptModelVersion = "gpt-3.5-turbo-0125";

    public async ValueTask HandleUserPrompt(Message message)
    {
        var systemPrompt = new ChatMessage(Role.System, await File.ReadAllTextAsync("prompt.txt"));
        var userPrompt = new ChatMessage(Role.User, message.Text);

        var chatResponse = await _openAiClient.ChatEndpoint.GetCompletionAsync(new ChatRequest(
                messages: new[] { systemPrompt, userPrompt },
                model: GptModelVersion,
                responseFormat: ChatResponseFormat.Json,
                temperature: 0, // more deterministic
                number: 1));

        var calendarResponse = JsonConvert.DeserializeObject<CalendarResponse>(chatResponse.FirstChoice.Message);

        var calendar = new Calendar();

        foreach (var responseEvent in calendarResponse.Events)
        {
            var startDate = DateTime.ParseExact($"{responseEvent.Date} {responseEvent.Time}", "yyyy-MM-dd HH:mm", CultureInfo.CurrentCulture);
            var duration = !TimeSpan.TryParseExact($"{responseEvent.Duration}", @"hh\:mm", CultureInfo.CurrentCulture, out var d)
                ? TimeSpan.Zero
                : d;

            var e = new CalendarEvent
            {
                Start = new CalDateTime(startDate),
                Duration = duration,
                Description = responseEvent.Description,
                Summary = responseEvent.Title,
                Location = responseEvent.Location,
            };

            calendar.Events.Add(e);
        }

        var serializer = new CalendarSerializer();
        var serializedCalendar = serializer.SerializeToString(calendar);

        await botClient.SendDocumentAsync(
            message.Chat.Id,
            InputFile.FromStream(new MemoryStream(Encoding.UTF8.GetBytes(serializedCalendar)), fileName: $"calendar.ics"));
    }


}