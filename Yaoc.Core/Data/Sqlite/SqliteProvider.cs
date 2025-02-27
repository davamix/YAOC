using Microsoft.Data.Sqlite;

namespace Yaoc.Core.Data.Sqlite {
    public partial class SqliteProvider : IStorageProvider {
        private string connectionString = "Data Source=yaoc.db";

        public SqliteProvider() {
            InitializeStorage();
        }

        private void InitializeStorage() {
            using (var db = new SqliteConnection(connectionString)) {
                db.Open();
                CreateConversationsTable(db);
                CreateMessagesTable(db);
                CreateAttachedResourcesTable(db);
                // Add images[] and tool_calls[] tables ??
            }
        }
    }
}
