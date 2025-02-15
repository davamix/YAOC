using Microsoft.Extensions.Configuration;
using OllamaSharp;
using System.Net.Http;

namespace Yaoc.Services;

public interface IOllamaService {
    IAsyncEnumerable<string> SendMessage(Chat chat, string message);
    Task<List<string>> GetModelsAsync();
    Chat CreateChat();

    Task<bool> TestConnection(string ollamaServerUrl);
}

internal class OllamaService : IOllamaService {
    private IOllamaApiClient _ollamaApiClient;
    private readonly IConfiguration _configuration;

    public OllamaService(IConfiguration configuration) {

        _configuration = configuration;

        ConfigureOllamaClient();
    }

    private void ConfigureOllamaClient() {
        _ollamaApiClient = new OllamaApiClient(new Uri(_configuration["AppSettings:OllamaServer"]));
    }

    public Chat CreateChat() => new(_ollamaApiClient);

    public async IAsyncEnumerable<string> SendMessage(Chat chat, string message) {
        //var sb = new StringBuilder();

        //for (var x = 0; x < 10; x++) {
        //    await Task.Delay(300);
        //    sb.Append($"Message {x} ");
        //    //chat.Messages.Add(new Message(ChatRole.Assistant, $"Message {x}"));
        //    yield return sb.ToString();
        //}

        //chat.Messages.Add(new(ChatRole.Assistant, sb.ToString()));

        //yield break;

        await foreach (var response in chat.SendAsync(message)) { 
            yield return response;
        }
    }

    public async Task<List<string>> GetModelsAsync() {
        var models = await _ollamaApiClient.ListLocalModelsAsync();

        return models.Select(model => model.Name).ToList();
    }

    public async Task<bool> TestConnection(string ollamaServerUrl) {
        _configuration["AppSettings:OllamaServer"] = ollamaServerUrl;
        ConfigureOllamaClient();

        try {
            return await _ollamaApiClient.IsRunningAsync();
        } catch (HttpRequestException ex) {
            return false;
        } catch (Exception ex) {
            return false;
        }
    }
}
