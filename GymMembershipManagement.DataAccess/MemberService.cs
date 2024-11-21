using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using GymMembershipManagement.DataAccess.Models;

namespace GymMembershipManagement.DataAccess.Services
{
    public class MemberService
    {
        private readonly string _connectionString;
        private readonly EmailService _emailService;

        public MemberService(DatabaseConnection dbConnection, EmailService emailService)
        {
            _connectionString = dbConnection.ConnectionString;
            _emailService = emailService;
        }



        public async Task NotifyExpiringMembershipsAsync()
        {
            var members = GetMembersWithStatus(); // Ovo dohvaća sve članove s njihovim statusom

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                foreach (var member in members)
                {
                    if (member.RemainingDays == 3 && member.Status == "ExpiringSoon" && !member.IsNotificationSent)
                    {
                        //Console.WriteLine($"Šaljem mail za člana: {member.Fullname}, Email: {member.EmailAddress}, ID: {member.Id}");

                        var subject = "URBAN GYM - Podsjetnik: Vaša članarina ističe uskoro!";
                        var body = $@"
                <h3>Poštovani {member.Fullname},</h3>
                <p>Vaša članarina ({member.MembershipType}) ističe za 3 dana ({member.StartDate?.AddDays(GetMembershipDuration(member.MembershipType)).ToString("dd.MM.yyyy")}).</p>
                <p>Molimo vas da obnovite članstvo kako biste nastavili koristiti naše usluge.</p>
                <p>Hvala,</p>
                <p>Urban GYM</p>
            ";

                        await _emailService.SendEmailAsync(member.EmailAddress, subject, body);

                        // Nakon slanja maila, ažuriraj IsNotificationSent u bazi
                        var updateSql = "UPDATE Members SET IsNotificationSent = 1 WHERE Id = @MemberId";

                        using (var updateCommand = new SqlCommand(updateSql, connection))
                        {
                            updateCommand.Parameters.AddWithValue("@MemberId", member.Id);
                            await updateCommand.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
        }



        // Get all members
        public List<Member> GetAllMembers()
        {
            var members = new List<Member>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = "SELECT * FROM Members";
                var command = new SqlCommand(sql, connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    members.Add(new Member
                    {
                        Id = (int)reader["Id"],
                        Fullname = reader["Fullname"].ToString(),
                        Phone = reader["Phone"].ToString(),
                        Sex = reader["Sex"].ToString(),
                        EmailAddress = reader["EmailAddress"].ToString(),
                        Address = reader["Address"].ToString(),
                        Type = reader["Type"].ToString(),
                        Birthdate = reader["Birthdate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["Birthdate"]),
                        StartDate = reader["StartDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["StartDate"]),
                        Picture = reader["Picture"].ToString(),
                        Description = reader["Description"].ToString(),
                        CreatedAt = (DateTime)reader["CreatedAt"]
                    });

                }
            }

            return members;
        }

        public Member GetMemberById(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = "SELECT * FROM Members WHERE Id = @Id";
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Member
                        {
                            Id = (int)reader["Id"],
                            Fullname = reader["Fullname"].ToString(),
                            Phone = reader["Phone"].ToString(),
                            EmailAddress = reader["EmailAddress"].ToString(),
                            Address = reader["Address"].ToString(),
                            Sex = reader["Sex"].ToString(),
                            Type = reader["Type"].ToString(),
                            Birthdate = reader["Birthdate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["Birthdate"]),
                            StartDate = reader["StartDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["StartDate"]),
                            Picture = reader["Picture"].ToString(),
                            Description = reader["Description"].ToString(),
                            CreatedAt = (DateTime)reader["CreatedAt"]
                        };
                    }
                }
            }
            return null;
        }


        // Add a new member

        public void AddMember(Member member)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Dodaj člana u tablicu Members
                var insertMemberSql = @"
            INSERT INTO Members (Fullname, Phone, Sex, EmailAddress, Address, Type, Birthdate, StartDate, Picture, Description) 
            OUTPUT INSERTED.Id
            VALUES (@Fullname, @Phone, @Sex, @EmailAddress, @Address, @Type, @Birthdate, @StartDate, @Picture, @Description)";

                var command = new SqlCommand(insertMemberSql, connection);
                command.Parameters.AddWithValue("@Fullname", member.Fullname ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Phone", member.Phone ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Sex", member.Sex ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@EmailAddress", member.EmailAddress ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Address", member.Address ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Type", member.Type ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Birthdate", member.Birthdate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@StartDate", member.StartDate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Picture", member.Picture ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Description", member.Description ?? (object)DBNull.Value);

                // Dohvati ID novog člana
                int memberId = (int)command.ExecuteScalar();

                // Dodaj uplatu u tablicu Payments
                var insertPaymentSql = @"
            INSERT INTO Payments (Member, Type, Amount, Total, CreatedAt)
            VALUES (@Member, @Type, @Amount, @Total, GETDATE())";

                var paymentCommand = new SqlCommand(insertPaymentSql, connection);
                paymentCommand.Parameters.AddWithValue("@Member", memberId);
                paymentCommand.Parameters.AddWithValue("@Type", member.Type ?? (object)DBNull.Value);
                paymentCommand.Parameters.AddWithValue("@Amount", GetMembershipAmount(member.Type));
                paymentCommand.Parameters.AddWithValue("@Total", GetMembershipAmount(member.Type)); 
                paymentCommand.ExecuteNonQuery();
            }
        }





        public void UpdateMember(Member member)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Provera da li je članarina aktivna
                var checkSql = @"
        SELECT StartDate, Type 
        FROM Members 
        WHERE Id = @MemberId";

                var checkCommand = new SqlCommand(checkSql, connection);
                checkCommand.Parameters.AddWithValue("@MemberId", member.Id);

                using (var reader = checkCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var startDate = reader["StartDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["StartDate"]);
                        var type = reader["Type"].ToString();
                        var duration = GetMembershipDuration(type);

                        
                        if (startDate.HasValue && startDate.Value.AddDays(duration) >= DateTime.Now.Date)
                        {
                            if (member.StartDate != startDate || member.Type != type)
                            {
                                throw new InvalidOperationException("Ne možete menjati Tip članarine ili Datum početka dok je članarina aktivna.");
                            }
                        }
                    }
                }

                // Ažuriranje podataka
                var sql = @"
        UPDATE Members 
        SET Fullname = @Fullname, Phone = @Phone, Sex = @Sex, EmailAddress = @EmailAddress, 
            Address = @Address, Birthdate = @Birthdate, 
            Picture = CASE WHEN @Picture IS NULL THEN Picture ELSE @Picture END, 
            Description = @Description
        WHERE Id = @Id";

                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", member.Id);
                command.Parameters.AddWithValue("@Fullname", member.Fullname ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Phone", member.Phone ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Sex", member.Sex ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@EmailAddress", member.EmailAddress ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Address", member.Address ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Birthdate", member.Birthdate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Picture", string.IsNullOrEmpty(member.Picture) ? (object)DBNull.Value : member.Picture);
                command.Parameters.AddWithValue("@Description", member.Description ?? (object)DBNull.Value);
                command.ExecuteNonQuery();
            }
        }



        // Delete a member
        public void DeleteMember(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = "DELETE FROM Members WHERE Id = @Id";
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
        }

        // Metoda za dohvaćanje članova sa statusom

        public List<MemberStatus> GetMembersWithStatus()
        {
            var members = new List<MemberStatus>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = @"
            SELECT Id, Fullname, Phone, EmailAddress, Picture, Type, StartDate, 
                   IsNotificationSent
            FROM Members";

                using (var command = new SqlCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var type = reader["Type"].ToString();
                        var startDate = reader["StartDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["StartDate"]);
                        var duration = GetMembershipDuration(type);
                        var remainingDays = startDate.HasValue
                            ? (startDate.Value.AddDays(duration).Date - DateTime.Now.Date).Days
                            : 0;

                        var status = remainingDays > 0
                            ? (remainingDays <= 3 ? "ExpiringSoon" : "Active")
                            : "Expired";

                        members.Add(new MemberStatus
                        {
                            Id = (int)reader["Id"],
                            Fullname = reader["Fullname"].ToString(),
                            Phone = reader["Phone"].ToString(),
                            EmailAddress = reader["EmailAddress"].ToString(),
                            Picture = reader["Picture"].ToString(),
                            MembershipType = type,
                            StartDate = startDate,
                            RemainingDays = remainingDays > 0 ? remainingDays : 0,
                            Status = status,
                            IsNotificationSent = reader["IsNotificationSent"] != DBNull.Value && Convert.ToBoolean(reader["IsNotificationSent"])
                        });
                    }
                }
            }

            return members;
        }




        public bool ProcessPayment(int memberId, string membershipType, decimal amountPaid)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("ProcessPayment", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@MemberId", memberId);
                    command.Parameters.AddWithValue("@MembershipType", membershipType);
                    command.Parameters.AddWithValue("@AmountPaid", amountPaid);

                    command.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Logiraj grešku ako je potrebno
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        private decimal GetMembershipAmount(string type)
        {
            return type switch
            {
                "Mjesečna" => 30m,
                "Dvomjesečna" => 55m,
                "Polugodišnja" => 180m,
                "Godišnja" => 300m,
                _ => 0m
            };
        }



        // Pomoćna metoda za trajanje članstva
        private int GetMembershipDuration(string membershipType)
        {
            return membershipType switch
            {
                "Mjesečna" => 31,
                "Dvomjesečna" => 61,
                "Polugodišnja" => 183,
                "Godišnja" => 366,
                _ => 0
            };
        }

        public string GetMembershipStatus(DateTime startDate, string type)
        {
            int duration = GetMembershipDuration(type);

            DateTime endDate = startDate.AddDays(duration);
            int remainingDays = (endDate.Date - DateTime.Now.Date).Days;

            if (remainingDays > 3)
                return $"<span class='badge bg-success'>Aktivno - Ističe za {remainingDays} dana</span>";
            else if (remainingDays > 0)
                return $"<span class='badge bg-warning'>Ističe za {remainingDays} dana</span>";
            else
                return "<span class='badge bg-danger'>Istekla članarina</span>";
        }


        public void ExtendMembership(int memberId, DateTime newStartDate, string newMembershipType)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Provjeri je li članarina aktivna
                var checkSql = @"
            SELECT StartDate, Type 
            FROM Members 
            WHERE Id = @MemberId";

                var checkCommand = new SqlCommand(checkSql, connection);
                checkCommand.Parameters.AddWithValue("@MemberId", memberId);

                using (var reader = checkCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var startDate = reader["StartDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["StartDate"]);
                        var type = reader["Type"].ToString();
                        var duration = GetMembershipDuration(type);

                        // Ako članarina nije istekla, baci grešku
                        if (startDate.HasValue && startDate.Value.AddDays(duration) >= DateTime.Now.Date)
                        {
                            throw new InvalidOperationException("Članarina je još uvijek aktivna. Produženje nije moguće.");
                        }
                    }
                }

                // Ažuriraj člana i resetiraj IsNotificationSent
                var updateMemberSql = @"
            UPDATE Members 
            SET StartDate = @NewStartDate, 
                Type = @NewMembershipType, 
                IsNotificationSent = 0
            WHERE Id = @MemberId";

                var updateCommand = new SqlCommand(updateMemberSql, connection);
                updateCommand.Parameters.AddWithValue("@NewStartDate", newStartDate);
                updateCommand.Parameters.AddWithValue("@NewMembershipType", newMembershipType);
                updateCommand.Parameters.AddWithValue("@MemberId", memberId);
                updateCommand.ExecuteNonQuery();

                // Dodaj uplatu u tablicu Payments
                var insertPaymentSql = @"
            INSERT INTO Payments (Member, Type, Amount, Total, CreatedAt)
            VALUES (@Member, @Type, @Amount, @Total, GETDATE())";

                var paymentCommand = new SqlCommand(insertPaymentSql, connection);
                paymentCommand.Parameters.AddWithValue("@Member", memberId);
                paymentCommand.Parameters.AddWithValue("@Type", newMembershipType);
                paymentCommand.Parameters.AddWithValue("@Amount", GetMembershipAmount(newMembershipType));
                paymentCommand.Parameters.AddWithValue("@Total", GetMembershipAmount(newMembershipType));
                paymentCommand.ExecuteNonQuery();
            }
        }




    }
}
