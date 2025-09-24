using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AiCalendarTelegramBot;

public class TelegramBotWebhookTrigger
{
    private readonly ILogger _logger;

    public TelegramBotWebhookTrigger(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<TelegramBotWebhookTrigger>();
    }

    [Function("TelegramBotWebhookTrigger")]
    public async Task<FunctionOutput> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
        FunctionContext executionContext)
    {
        var body = await req.ReadAsStringAsync();
        if (body == null) throw new InvalidOperationException("body is null");
        
        return new FunctionOutput(req.CreateResponse(HttpStatusCode.OK), body);
    }
}