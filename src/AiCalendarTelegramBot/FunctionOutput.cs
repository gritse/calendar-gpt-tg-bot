using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace AiCalendarTelegramBot;

public record FunctionOutput(
    HttpResponseData HttpResponse, 
    [property: QueueOutput("chatgpt-tg-queue", Connection = "AzureWebJobsStorage")] string Body);