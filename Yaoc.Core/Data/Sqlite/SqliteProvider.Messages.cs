using Microsoft.Data.Sqlite;
using Yaoc.Core.Data.Mappers;
using Yaoc.Core.Models;

namespace Yaoc.Core.Data.Sqlite;

public partial class SqliteProvider {
    public async Task<IEnumerable<ChatMessage>> LoadMessages(string conversationId) {
        var messages = new List<ChatMessage>();
        var query = "SELECT m.Id, m.ConversationId, m.Message, m.Role, m.Content, m.CreatedAt, af.ResourcePath, af.ResourceName " +
            "FROM Messages m " +
            "LEFT JOIN AttachedResources af " +
            "ON af.MessageId = m.Id " +
            "WHERE m.ConversationId = @ConversationId " +
            "ORDER BY m.CreatedAt asc";

        using (var db = new SqliteConnection(connectionString)) {
            db.Open();
            var command = new SqliteCommand(query, db);
            command.Parameters.AddWithValue("@ConversationId", conversationId);

            var reader = command.ExecuteReader();
            messages = reader.ToMessages().ToList();
        }

        return await Task.FromResult(messages);
    }

    public Task<ChatMessage> SaveMessage(ChatMessage message, string conversationId) {
        if (string.IsNullOrEmpty(message.Id)) {
            return Task.FromResult(InsertMessage(message, conversationId)); // Return with new Id
        }

        UpdateMessage(message, conversationId);
        
        return Task.FromResult(message);
    }

    private void CreateMessagesTable(SqliteConnection db) {
        var query = """
                CREATE TABLE IF NOT EXISTS "Messages" (
                	"Id" TEXT NOT NULL UNIQUE,
                	"ConversationId" TEXT NOT NULL,
                	"Role" TEXT NOT NULL,
                    "Message" TEXT NOT NULL,
                	"Content" TEXT NOT NULL,
                	"CreatedAt" TEXT NOT NULL,
                	PRIMARY KEY("Id")
                )
                """;
        var command = new SqliteCommand(query, db);
        command.ExecuteReader();
    }

    private ChatMessage InsertMessage(ChatMessage message, string conversationId) {
        var messageId = Guid.NewGuid().ToString();
        var query = "INSERT INTO Messages (Id, ConversationId, Message, Role, Content, CreatedAt) " +
            "VALUES (@Id, @ConversationId, @Message, @Role, @Content, @CreatedAt)";

        using (var db = new SqliteConnection(connectionString)) {
            db.Open();
            var command = new SqliteCommand(query, db);
            command.Parameters.AddWithValue("@Id", messageId);
            command.Parameters.AddWithValue("@ConversationId", conversationId);
            command.Parameters.AddWithValue("@Message", message.Message);
            command.Parameters.AddWithValue("@Role", message.OllamaMessage.Role.ToString());
            command.Parameters.AddWithValue("@Content", message.OllamaMessage.Content);
            command.Parameters.AddWithValue("@CreatedAt", DateTime.Now.ToString());

            command.ExecuteNonQuery();

            // Save attached files
            SaveAttachedResources(messageId, message.AttachedResources);

            return new ChatMessage {
                Id = messageId,
                ConversationId = conversationId,
                Message = message.Message,
                AttachedResources = message.AttachedResources,
                CreatedAt = DateTime.Now,
                OllamaMessage = message.OllamaMessage
            };
        }
    }

    private void UpdateMessage(ChatMessage message, string conversationId) {
        var query = "UPDATE Messages SET Role = @Role, Content = @Content WHERE ConversationId = @ConversationId";

        using (var db = new SqliteConnection(connectionString)) {
            db.Open();
            var command = new SqliteCommand(query, db);
            command.Parameters.AddWithValue("@Role", message.OllamaMessage.Role.ToString());
            command.Parameters.AddWithValue("@Content", message.OllamaMessage.Content);
            command.Parameters.AddWithValue("@ConversationId", conversationId);
            command.ExecuteNonQuery();
        }
    }
}
