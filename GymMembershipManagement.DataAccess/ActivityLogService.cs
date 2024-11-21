using GymMembershipManagement.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMembershipManagement.DataAccess
{
    public class ActivityLogService
    {
        private readonly string _connectionString;

        public ActivityLogService(DatabaseConnection dbConnection)
        {
            _connectionString = dbConnection.ConnectionString;
        }

        public async Task AddLogAsync(int userId, string type, string logDetails)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                INSERT INTO Logs (UserId, Type, Logs)
                VALUES (@UserId, @Type, @Logs)";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@Type", type);
                command.Parameters.AddWithValue("@Logs", logDetails);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<List<UserActivityLog>> GetLogsAsync()
        {
            var logs = new List<UserActivityLog>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"
        SELECT l.Id, l.UserId, l.Type, l.Logs, l.CreatedAt
        FROM Logs l
        ORDER BY l.CreatedAt DESC";
                var command = new SqlCommand(query, connection);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var userId = (int)reader["UserId"];
                        var username = userId switch
                        {
                            1 => "Admin - Filip",
                            2 => "Admin - Domagoj",
                            _ => "Nepoznati korisnik"
                        };

                        logs.Add(new UserActivityLog
                        {
                            Id = (int)reader["Id"],
                            UserId = userId,
                            ActionType = reader["Type"].ToString(),
                            ActionDetails = reader["Logs"].ToString(),
                            CreatedAt = (DateTime)reader["CreatedAt"],
                            Username = username
                        });
                    }
                }
            }
            return logs;
        }

    }
}