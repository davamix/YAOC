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
                var message = messages.FirstOrDefault(x => x.Id == reader.GetString(0));

                if(message == null) {
                    var cm = CreateChatMessage(reader);
                    messages.Add(cm);
                } else {
                    SetMessageValues(message, reader);
                }
            }

            return messages;
        }

        private static ChatMessage CreateChatMessage(SqliteDataReader reader) {
            var message = new ChatMessage();
            SetMessageValues(message, reader);
            return message;
        }

        private static void SetMessageValues(ChatMessage message, SqliteDataReader reader) {
            message.Id = reader.GetString(0);
            message.ConversationId = reader.GetString(1);
            message.Message = reader.GetString(2);
            message.OllamaMessage = new Message() {
                Role = new ChatRole(reader.GetString(3)),
                Content = reader.GetString(4)
            };
            message.CreatedAt = DateTime.Parse(reader.GetString(5));

            if (!reader.IsDBNull(6)) {
                message.AttachedResources.Add(
                    new MessageResource {
                        Path = reader.GetString(6),
                        Name = reader.GetString(7)
                    });
            }
        }
    }
}
