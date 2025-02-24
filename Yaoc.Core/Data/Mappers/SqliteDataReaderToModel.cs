using Microsoft.Data.Sqlite;
using OllamaSharp.Models.Chat;
using Yaoc.Core.Models;

namespace Yaoc.Core.Data.Mappers {
    public static class SqliteDataReaderToModel {
        public static IEnumerable<Conversation> ToConversations(this SqliteDataReader reader) {
            var conversations = new List<Conversation>();

            foreach (var r in reader) {
                conversations.Add(new Conversation {
                    Id = reader.GetString(0),
                    Name = reader.GetString(1),
                    Model = reader.GetString(2),
                    CreatedAt = DateTime.Parse(reader.GetString(3))
                });
            }

            return conversations;
        }

        public static IEnumerable<ChatMessage> ToMessages(this SqliteDataReader reader) {
            var messages = new List<ChatMessage>();

            foreach (var r in reader) {
                messages.Add(new ChatMessage {
                    Id = reader.GetString(0),
                    ConversationId = reader.GetString(1),
                    OllamaMessage = new Message() {
                        Role = new ChatRole(reader.GetString(2)),
                        Content = reader.GetString(3)
                    },
                    CreatedAt = DateTime.Parse(reader.GetString(4))
                });
            }

            return messages;
        }
    }
}
