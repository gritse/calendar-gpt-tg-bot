using AiCalendarTelegramBot.Abstractions;
using OpenAI;
using OpenAI.Chat;

namespace AiCalendarTelegramBot.ChatGPT;

public class AiChatThread(string openAiKey) : IAiThread
{
    private const string GptModelVersion = "gpt-3.5-turbo-0125";
    
    private readonly OpenAIClient _openAiClient = new(openAiKey);
    private IReadOnlyCollection<Message> _chatMessages = Array.Empty<Message>();

    public async Task<IAiMessage> WriteMessage(string text, Role role)
    {
        _chatMessages = await ExecuteCompletion(_chatMessages.Append(new Message(role, text)).ToArray());
        
        var lastMessage = _chatMessages.Last();
        string responseText = lastMessage.Content.ToString();

        return new AiMessage(responseText);
    }

    public async ValueTask AppendMessage(string text, Role role)
    {
        _chatMessages = _chatMessages.Append(new Message(role, text)).ToArray();
    }

    private async Task<IReadOnlyCollection<Message>> ExecuteCompletion(IReadOnlyCollection<Message> chatMessages)
    {
        var chatResponse = await _openAiClient.ChatEndpoint.GetCompletionAsync(new ChatRequest(
            chatMessages,
            model: GptModelVersion,
            temperature: 0, // more deterministic
            number: 1)); // one response

        return chatMessages.Append(chatResponse.FirstChoice.Message).ToArray();
    }
}

