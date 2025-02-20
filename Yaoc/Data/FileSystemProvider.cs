using Yaoc.Models;
using Yaoc.Services;
using OllamaSharp;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows;
using Yaoc.Messages.Snackbar;
using System.Windows.Data;

namespace Yaoc.Data;

public class ChatConverter : JsonConverter<Chat> {
    private readonly IOllamaService _ollamaService;
    public ChatConverter(IOllamaService ollamaService) {
        _ollamaService = ollamaService;
    }
    public override Chat? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        if (!JsonDocument.TryParseValue(ref reader, out JsonDocument jsonDoc)) {
            throw new JsonException("ChatConverter couldn't parse Chat JSON");
        }

        var chatJson = jsonDoc.RootElement;

        //var m = new Message()

        return _ollamaService.CreateChat();

    }

    public override void Write(Utf8JsonWriter writer, Chat value, JsonSerializerOptions options) {
        throw new NotImplementedException();
    }
}

public class FileSystemProvider : IStorageProvider {
    public async Task<IEnumerable<Conversation>> LoadConversations() {
        var data = Enumerable.Empty<Conversation>();

        if (File.Exists("conversations.json")) {
            using FileStream stream = File.OpenRead("conversations.json");
            //            var jsonOptions = new JsonSerializerOptions {
            //                Converters = {
            //new ChatConverter(_ollamaService)
            //                }
            //            };

            try {
                data = await JsonSerializer.DeserializeAsync<IEnumerable<Conversation>>(stream);
            } catch (Exception e) {
                Debug.WriteLine(e.Message);
                throw;
            }
        }

        return data;
    }

    public async Task SaveConversation(Conversation conversation) {
        var updatedConversations = await UpdateConversation(conversation);

        await Save(updatedConversations);
    }

    public async Task SaveConversations(IEnumerable<Conversation> conversations) {
        await Save(conversations);
    }

    private async Task<IEnumerable<Conversation>> UpdateConversation(Conversation conversation) {
        var conversations = (await LoadConversations()).ToList();

        var changeConversation = conversations.FirstOrDefault(c => c.Id == conversation.Id);
        if (changeConversation != null) {
            conversations.Remove(changeConversation);
        }

        conversations.Add(conversation);

        return conversations;
    }

    private async Task Save(IEnumerable conversations) {
        using (FileStream stream = File.Open("conversations.json", FileMode.Create)) {
            await JsonSerializer.SerializeAsync(stream, conversations,
                        new JsonSerializerOptions {
                            WriteIndented = true
                        });
        }
    }
}
