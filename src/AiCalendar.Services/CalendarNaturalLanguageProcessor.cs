#pragma warning disable OPENAI001
using System.Text;
using AiCalendarTelegramBot;
using Ardalis.GuardClauses;
using Newtonsoft.Json;
using NJsonSchema;
using OpenAI;
using OpenAI.Responses;

namespace AiCalendar.Services;

public class CalendarNaturalLanguageProcessor
{
    private readonly OpenAIClient _openAiClient;

    public CalendarNaturalLanguageProcessor(OpenAIClient openAiClient)
    {
        _openAiClient = openAiClient;
    }

    public async Task<CalendarResponse> GetCalendarResponseAsync(string eventsDescription)
    {
        Guard.Against.NullOrWhiteSpace(eventsDescription);

        var responseClient = _openAiClient.GetOpenAIResponseClient(Constants.GptModelVersion);
        Guard.Against.Null(responseClient, message: "Failed to create OpenAI response client");

        var jsonSchema = GetCalendarResponseSchema();
        var creationOptions = CreateResponseOptions(jsonSchema);

        var response = await responseClient.CreateResponseAsync(eventsDescription, creationOptions);
        Guard.Against.Null(response?.Value, message: "OpenAI response is null");

        var outputText = response.Value.GetOutputText();
        Guard.Against.NullOrWhiteSpace(outputText, message: "OpenAI response output text is null or empty");

        var calendarResponse = JsonConvert.DeserializeObject<CalendarResponse>(outputText);
        return Guard.Against.Null(calendarResponse, message: "Failed to deserialize calendar response");
    }

    private static ResponseCreationOptions CreateResponseOptions(string jsonSchema)
    {
        Guard.Against.NullOrWhiteSpace(jsonSchema);

        return new ResponseCreationOptions()
        {
            Instructions = string.Format(Constants.Instructions, GetWeekCalendar()),
            ReasoningOptions = new ResponseReasoningOptions()
            {
                ReasoningEffortLevel = ResponseReasoningEffortLevel.Low,
            },
            TextOptions = new ResponseTextOptions()
            {
                TextFormat = ResponseTextFormat.CreateJsonSchemaFormat(
                    "ev",
                    BinaryData.FromString(jsonSchema),
                    "Events schema",
                    jsonSchemaIsStrict: true),
            }
        };
    }

    private static string GetWeekCalendar()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"CURRENT DATE: {DateTime.UtcNow.Date.ToLongDateString()}");
        sb.AppendLine($"NEXT DAYS:");

        for (int i = 1; i < 7; i++)
        {
            sb.AppendLine($"{DateTime.UtcNow.Date.AddDays(i).ToLongDateString()}");
        }

        return sb.ToString();
    }

    private static string GetCalendarResponseSchema()
    {
        var schema = JsonSchema.FromType<CalendarResponse>();
        var jsonSchema = schema.ToJson();

        return Guard.Against.NullOrWhiteSpace(jsonSchema, message: "Failed to generate JSON schema for CalendarResponse");
    }
}