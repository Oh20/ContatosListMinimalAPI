using NUnit.Framework;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ContatosListMinimalAPI.Tests
{
    [TestFixture]
    public class DatabaseConnectionTests
    {
        [Test]
        [Category("DatabaseIntegration")]
        public async Task TestDatabaseConnection()
        {
            //var connectionString = "Server=tcp:testdbsql-001.database.windows.net,1433;Initial Catalog=Banco_lab_temp;Persist Security Info=False;User ID=azureadmin;Password=Senha@@5966;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not set in the environment variables.");
            }

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                Assert.AreEqual(System.Data.ConnectionState.Open, connection.State, "Failed to open connection to the database.");
            }
        }
    }
}
