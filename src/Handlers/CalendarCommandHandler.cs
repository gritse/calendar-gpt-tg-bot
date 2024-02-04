using System.Runtime.Caching;
using System.Text;
using System.Text.RegularExpressions;
using AiCalendarTelegramBot.ChatGPT;
using OpenAI;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AiCalendarTelegramBot.Handlers;

public class CalendarCommandHandler(ITelegramBotClient botClient, string openAiKey)
{
    public async ValueTask HandleUserPrompt(Message message)
    {
        var aiAssistant = new AiChat(openAiKey);
        var aiThread = await aiAssistant.CreateThread();

        var systemPrompt = "Ты помощник по календарю. Твоя задача извлекать из текста информацию " +
                           "о событиях и возвращать пользователю события в формате iCalendar.  " +
                           "Пользователь будет отправлять тебе текст на естественном языке, " +
                           "а ты будешь извлекать информацию о событиях. " +
                           "НЕ УКАЗЫВАЙ ЧАСОВОЙ ПОЯС В ДАТЕ. НИ В КОЕМ СЛУЧАЕ НЕ УКАЗЫВАЙ " +
                           "В ДАТЕ ИДЕНТИФИКАТОР ЧАСОВОГО ПОЯСА. " +
                           "Если в тексте указана неполная или относительная дата и время, " +
                           "то учитывай текущую дату и время. " +
                           "При этом SUMMARY и DESCRIPTION должно быть НА ТОМ ЖЕ ЯЗЫКЕ " +
                           "на котором ИЗНАЧАЛЬНЫЙ ТЕКСТ ПОЛЬЗОВАТЕЛЯ. " +
                           "Ответ пользователю ОБЯЗАТЕЛЬНО ДОЛЖЕН ВКЛЮЧАИТЬ ТЕКСТ " +
                           "в формате iCalendar (расширение *.ics)";
        
        await aiThread.AppendMessage(systemPrompt, Role.System);
        var response = await aiThread.WriteMessage(message.Text!, Role.User);

        var vCalendarMatches = Regex.Matches(response.Text!, @"BEGIN:VCALENDAR.+?END:VCALENDAR", RegexOptions.Singleline);
        
        foreach (Match vCal in vCalendarMatches)
        {
            await botClient.SendDocumentAsync(
                message.Chat.Id,
                InputFile.FromStream(new MemoryStream(Encoding.UTF8.GetBytes(vCal.Value)), fileName: $"calendar.ics"));
        }
    }

    public async ValueTask<bool> CanHandle(Message message) => true;
}