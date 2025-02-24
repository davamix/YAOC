using Microsoft.Data.Sqlite;
using Yaoc.Core.Data.Mappers;
using Yaoc.Core.Models;

namespace Yaoc.Core.Data {
    public class SqliteProvider : IStorageProvider {
        private string connectionString = "Data Source=yaoc.db";

        public SqliteProvider() {
            InitializeStorage();
        }
        private void InitializeStorage() {
            using (var db = new SqliteConnection(connectionString)) {
                db.Open();
                CreateConversationsTable(db);
                CreateMessagesTable(db);
                // Add images[] and tool_calls[] tables
                // check conversations.json;
            }
        }

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
                } catch (System.Exception e) {
                    System.Console.WriteLine(e.Message);

                }

                return await Task.FromResult(conversations);
            }
        }

        public async Task<IEnumerable<ChatMessage>> LoadMessages(string conversationId) {
            var messages = new List<ChatMessage>();
            var query = "SELECT m.Id, m.ConversationId, m.Role, m.Content, m.CreatedAt " +
                "FROM Messages m " +
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

        public Task<Conversation> SaveConversation(Conversation conversation) {
            if (string.IsNullOrEmpty(conversation.Id)) {
                return Task.FromResult(InsertConversation(conversation)); // Return with new Id
            }

            UpdateConversation(conversation);
            return Task.FromResult(conversation);
        }

        public Task<ChatMessage> SaveMessage(ChatMessage message, string conversationId) {
            if (string.IsNullOrEmpty(message.Id)) {
                return Task.FromResult(InsertMessage(message, conversationId)); // Return with new Id
            }
            UpdateMessage(message, conversationId);
            return Task.FromResult(message);
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

        private void CreateMessagesTable(SqliteConnection db) {
            var query = """
                CREATE TABLE IF NOT EXISTS "Messages" (
                	"Id" TEXT NOT NULL UNIQUE,
                	"ConversationId" TEXT NOT NULL,
                	"Role" TEXT NOT NULL,
                	"Content" TEXT NOT NULL,
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

        private ChatMessage InsertMessage(ChatMessage message, string conversationId) {
            var messageId = Guid.NewGuid().ToString();
            var query = "INSERT INTO Messages (Id, ConversationId, Role, Content, CreatedAt) " +
                "VALUES (@Id, @ConversationId, @Role, @Content, @CreatedAt)";
            using (var db = new SqliteConnection(connectionString)) {
                db.Open();
                var command = new SqliteCommand(query, db);
                command.Parameters.AddWithValue("@Id", messageId);
                command.Parameters.AddWithValue("@ConversationId", conversationId);
                command.Parameters.AddWithValue("@Role", message.OllamaMessage.Role.ToString());
                command.Parameters.AddWithValue("@Content", message.OllamaMessage.Content);
                command.Parameters.AddWithValue("@CreatedAt", DateTime.Now.ToString());

                command.ExecuteNonQuery();

                return new ChatMessage {
                    Id = messageId,
                    ConversationId = conversationId,
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
}
