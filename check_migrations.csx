using System;
using System.Threading.Tasks;
using Npgsql;

public class Program {
    public static async Task Main() {
        var connStr = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING");
        if (string.IsNullOrEmpty(connStr)) {
            Console.WriteLine("No connection string found.");
            return;
        }
        await using var conn = new NpgsqlConnection(connStr);
        await conn.OpenAsync();
        
        await using var cmd = new NpgsqlCommand("SELECT \"MigrationId\" FROM \"__EFMigrationsHistory\" ORDER BY \"MigrationId\" DESC LIMIT 5", conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync()) {
            Console.WriteLine(reader.GetString(0));
        }
    }
}
await Program.Main();
