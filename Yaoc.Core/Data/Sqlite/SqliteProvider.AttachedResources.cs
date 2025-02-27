using Microsoft.Data.Sqlite;
using Yaoc.Core.Models;

namespace Yaoc.Core.Data.Sqlite;

public partial class SqliteProvider {
    private void CreateAttachedResourcesTable(SqliteConnection db) {
        var query = """
                CREATE TABLE IF NOT EXISTS "AttachedResources" (
                    "Id" TEXT NOT NULL UNIQUE,
                    "MessageId" TEXT NOT NULL,
                    "ResourcePath" TEXT NOT NULL,
                    "ResourceName" TEXT NOT NULL,
                    "CreatedAt" TEXT NOT NULL,
                    PRIMARY KEY("Id")
                )
                """;
        var command = new SqliteCommand(query, db);
        command.ExecuteReader();
    }

    private void SaveAttachedResources(string messageId, IEnumerable<MessageResource> resources) {
        foreach (var resource in resources) {
            InsertAttachedResource(messageId, resource);
        }
    }

    private void InsertAttachedResource(string messageId, MessageResource resource) {
        var fileId = Guid.NewGuid().ToString();
        var query = "INSERT INTO AttachedResources (Id, MessageId, ResourcePath, ResourceName, CreatedAt) " +
            "VALUES (@Id, @MessageId, @ResourcePath, @ResourceName, @CreatedAt)";

        using (var db = new SqliteConnection(connectionString)) {
            db.Open();
            var command = new SqliteCommand(query, db);
            command.Parameters.AddWithValue("@Id", fileId);
            command.Parameters.AddWithValue("@MessageId", messageId);
            command.Parameters.AddWithValue("@ResourcePath", resource.Path);
            command.Parameters.AddWithValue("@ResourceName", resource.Name);
            command.Parameters.AddWithValue("@CreatedAt", DateTime.Now.ToString());
            command.ExecuteNonQuery();
        }
    }
}
