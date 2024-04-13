using System.Runtime.Caching;
using AiCalendarTelegramBot.Handlers;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AiCalendarTelegramBot;

public class MessageQueueTrigger
{
    private readonly ILogger<MessageQueueTrigger> _logger;

    public MessageQueueTrigger(ILogger<MessageQueueTrigger> logger)
    {
        _logger = logger;
    }

    [Function(nameof(MessageQueueTrigger))]
    public async Task Run([QueueTrigger("chatgpt-tg-queue", Connection = "AzureWebJobsStorage")] QueueMessage queueMessage)
    {
        var tgApKey = Environment.GetEnvironmentVariable("TG_BOT_TOKEN")!;
        var openAiKey = Environment.GetEnvironmentVariable("OPENAI_TOKEN")!;

        var body = queueMessage.MessageText;
        _logger.LogInformation($"Queued update {body}");
        
        if (string.IsNullOrEmpty(body)) return;

        var update = JsonConvert.DeserializeObject<Update>(body);
        _logger.LogInformation($"Deserialized update is '{update}'");

        var message = update?.Message;
        if (message?.Text is null && message?.Caption is null) return;

        _logger.LogInformation($"Message from '{message.Chat.Id}:{message.Chat.Username}'");

        var allowedUsers = new[] { "***REMOVED***" };
        if (!allowedUsers.Contains(message.Chat.Username)) return;

        var botClient = new TelegramBotClient(tgApKey);
        var calendarHandler = new CalendarCommandHandler(botClient, openAiKey);
        
        await calendarHandler.HandleUserPrompt(message);
    }
}