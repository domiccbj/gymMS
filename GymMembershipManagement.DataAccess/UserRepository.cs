using GymMembershipManagement.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GymMembershipManagement.DataAccess
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(DatabaseConnection dbConnection)
        {
            _connectionString = dbConnection.ConnectionString;
        }


        public List<User> GetAllUsers()
        {
            var users = new List<User>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM Users", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Username = reader["Username"].ToString(),
                            Password = reader["Password"].ToString(),
                            Fullname = reader["Fullname"].ToString(),
                            Address = reader["Address"].ToString(),
                            Phone = reader["Phone"].ToString(),
                            Level = reader["Level"].ToString(),
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                        });
                    }
                }
            }
            return users;
        }

        public User GetUserByUsername(string username)
        {
            User user = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM Users WHERE Username = @Username", connection);
                command.Parameters.AddWithValue("@Username", username);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new User
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Username = reader["Username"].ToString(),
                            Password = reader["Password"].ToString(),
                            Fullname = reader["Fullname"].ToString(),
                            Address = reader["Address"].ToString(),
                            Phone = reader["Phone"].ToString(),
                            Level = reader["Level"].ToString(),
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                        };
                    }
                }
            }
            return user;
        }
    }
}
