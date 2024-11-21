using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymMembershipManagement.DataAccess.Models;

namespace GymMembershipManagement.DataAccess
{
    public class ReportService
    {
        private readonly string _connectionString;

        public ReportService(DatabaseConnection dbConnection)
        {
            _connectionString = dbConnection.ConnectionString;
        }

        public List<MemberReport> GetMemberPayments()
        {
            var reports = new List<MemberReport>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var command = new SqlCommand("GetMemberPayments", connection);
                command.CommandType = CommandType.StoredProcedure;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reports.Add(new MemberReport
                        {
                            MemberId = (int)reader["MemberId"],
                            Fullname = reader["Fullname"].ToString(),
                            Phone = reader["Phone"].ToString(),
                            Address = reader["Address"].ToString(),
                            Membership = reader["Membership"].ToString(),
                            TotalAmount = (decimal)reader["TotalAmount"],
                            Picture = reader["Picture"] != DBNull.Value ? reader["Picture"].ToString() : null // Učitaj sliku
                        });
                    }
                }
            }

            return reports;
        }

    }
}
