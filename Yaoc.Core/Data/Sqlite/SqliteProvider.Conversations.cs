using Microsoft.Data.Sqlite;
using Yaoc.Core.Data.Mappers;
using Yaoc.Core.Models;

namespace Yaoc.Core.Data.Sqlite;

public partial class SqliteProvider {
    public async Task<IEnumerable<Conversation>> LoadConversations() {
        var conversations = new List<Conversation>();

        var query = "SELECT Id, Name, Model, CreatedAt " +
            "FROM Conversations " +
            "ORDER BY CreatedAt asc";

        using (var db = new SqliteConnection(connectionString)) {
            db.Open();
            var command = new SqliteCommand(query, db);
            try {
                var reader = command.ExecuteReader();

                conversations = reader.ToConversations().ToList();
            } catch (Exception ex) {
                throw;
            }

            return await Task.FromResult(conversations);
        }
    }

    public Task<Conversation> SaveConversation(Conversation conversation) {
        if (string.IsNullOrEmpty(conversation.Id)) {
            return Task.FromResult(InsertConversation(conversation)); // Return with new Id
        }

        UpdateConversation(conversation);
        return Task.FromResult(conversation);
    }

    public Task DeleteConversation(string conversationId) {
        var query = "DELETE FROM Messages WHERE ConversationId = @Id;" +
            "DELETE FROM Conversations WHERE Id = @Id";
        using (var db = new SqliteConnection(connectionString)) {
            db.Open();
            var command = new SqliteCommand(query, db);
            command.Parameters.AddWithValue("@Id", conversationId);
            command.ExecuteNonQuery();
        }

        return Task.CompletedTask;
    }

    private void CreateConversationsTable(SqliteConnection db) {
        var query = """
                CREATE TABLE IF NOT EXISTS "Conversations" (
                	"Id" TEXT NOT NULL UNIQUE,
                	"Name" TEXT,
                	"Model" TEXT,
                	"CreatedAt" TEXT NOT NULL,
                	PRIMARY KEY("Id")
                )
                """;

        var command = new SqliteCommand(query, db);
        command.ExecuteReader();
    }

    private Conversation InsertConversation(Conversation conversation) {
        var conversationId = Guid.NewGuid().ToString();
        var query = "INSERT INTO Conversations (Id, Name, Model, CreatedAt) " +
            "VALUES (@Id, @Name, @Model, @CreatedAt)";

        using (var db = new SqliteConnection(connectionString)) {
            db.Open();
            var command = new SqliteCommand(query, db);
            command.Parameters.AddWithValue("@Id", conversationId);
            command.Parameters.AddWithValue("@Name", conversation.Name);
            command.Parameters.AddWithValue("@Model", conversation.Model);
            command.Parameters.AddWithValue("@CreatedAt", DateTime.Now.ToString());
            command.ExecuteNonQuery();

            return new Conversation {
                Id = conversationId,
                Name = conversation.Name,
                Model = conversation.Model,
                CreatedAt = DateTime.Now
            };
        }
    }

    private void UpdateConversation(Conversation conversation) {
        var query = "UPDATE Conversations SET Name = @Name, Model = @Model WHERE Id = @Id";

        using (var db = new SqliteConnection(connectionString)) {
            db.Open();
            var command = new SqliteCommand(query, db);
            command.Parameters.AddWithValue("@Name", conversation.Name);
            command.Parameters.AddWithValue("@Model", conversation.Model);
            command.Parameters.AddWithValue("@Id", conversation.Id);
            command.ExecuteNonQuery();
        }
    }
}
