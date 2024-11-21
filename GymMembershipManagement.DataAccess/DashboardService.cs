using GymMembershipManagement.DataAccess;
using System.Data.SqlClient;
using System.Globalization;
using System.Text.Json;

public class DashboardService
{
    private readonly string _connectionString;

    public DashboardService(DatabaseConnection dbConnection)
    {
        _connectionString = dbConnection.ConnectionString;
    }

    public decimal CalculateMonthlyEarnings()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var sql = "SELECT SUM(Total) AS MonthlyEarnings FROM Payments WHERE MONTH(CreatedAt) = MONTH(GETDATE())";
            var command = new SqlCommand(sql, connection);
            var result = command.ExecuteScalar();
            return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
        }
    }

    public decimal CalculateYearlyEarnings()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var sql = "SELECT SUM(Total) AS YearlyEarnings FROM Payments WHERE YEAR(CreatedAt) = YEAR(GETDATE())";
            var command = new SqlCommand(sql, connection);
            var result = command.ExecuteScalar();
            return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
        }
    }

    public int CountTotalActiveMembers()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var sql = "SELECT COUNT(*) AS TotalActiveMembers FROM Members WHERE DATEADD(MONTH, 1, StartDate) > GETDATE()";
            var command = new SqlCommand(sql, connection);
            var result = command.ExecuteScalar();
            return Convert.ToInt32(result);
        }
    }

    public int CountTotalMembers()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var sql = "SELECT COUNT(*) AS TotalMembers FROM Members";
            var command = new SqlCommand(sql, connection);
            var result = command.ExecuteScalar();
            return Convert.ToInt32(result);
        }
    }

    public string GetMonthlyEarningsChartData()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var sql = "SELECT YEAR(CreatedAt) AS Year, MONTH(CreatedAt) AS Month, SUM(Total) AS TotalSales " +
                      "FROM Payments " +
                      "GROUP BY YEAR(CreatedAt), MONTH(CreatedAt) " +
                      "ORDER BY Year, Month";

            var command = new SqlCommand(sql, connection);
            var reader = command.ExecuteReader();

            var labels = new List<string>();
            var data = new List<decimal>();
            while (reader.Read())
            {
                var month = new DateTime(1, Convert.ToInt32(reader["Month"]), 1).ToString("MMM");
                var year = reader["Year"].ToString();
                labels.Add($"{month} {year}");
                data.Add(Convert.ToDecimal(reader["TotalSales"]));
            }

            var chartData = new
            {
                labels,
                datasets = new[]
                {
                    new
                    {
                        label = "Mjesečna zarada",
                        data,
                        backgroundColor = "rgba(78, 115, 223, 0.5)",
                        borderColor = "rgba(78, 115, 223, 1)",
                        borderWidth = 1
                    }
                }
            };

            return JsonSerializer.Serialize(chartData);
        }
    }

    public string GetGenderPieChartData()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var sql = "SELECT " +
                      "COUNT(CASE WHEN Sex = 'Muški' THEN 1 END) AS MaleCount, " +
                      "COUNT(CASE WHEN Sex = 'Ženski' THEN 1 END) AS FemaleCount " +
                      "FROM Members";

            var command = new SqlCommand(sql, connection);
            var reader = command.ExecuteReader();

            if (reader.Read())
            {
                var chartData = new
                {
                    labels = new[] { "Muški", "Ženski" },
                    datasets = new[]
                    {
                        new
                        {
                            data = new[]
                            {
                                Convert.ToInt32(reader["MaleCount"]),
                                Convert.ToInt32(reader["FemaleCount"])
                            },
                            backgroundColor = new[] { "#4e73df", "#1cc88a" },
                            borderColor = new[] { "#ffffff", "#ffffff" },
                            borderWidth = 1
                        }
                    }
                };

                return JsonSerializer.Serialize(chartData);
            }

            return string.Empty;
        }
    }

    public string GetAnnualEarningsChartData()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var sql = "SELECT YEAR(CreatedAt) AS Year, SUM(Total) AS TotalSales " +
                      "FROM Payments " +
                      "GROUP BY YEAR(CreatedAt) " +
                      "ORDER BY Year";

            var command = new SqlCommand(sql, connection);
            var reader = command.ExecuteReader();

            var labels = new List<string>();
            var data = new List<decimal>();
            while (reader.Read())
            {
                var year = reader["Year"].ToString();
                labels.Add(year);
                data.Add(Convert.ToDecimal(reader["TotalSales"]));
            }

            var chartData = new
            {
                labels,
                datasets = new[]
                {
                new
                {
                    label = "Godišnja zarada",
                    data,
                    backgroundColor = "rgba(28, 200, 138, 0.5)",
                    borderColor = "rgba(28, 200, 138, 1)",
                    borderWidth = 1
                }
            }
            };

            return JsonSerializer.Serialize(chartData);
        }
    }

    public string GetAgeGroupChartData()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var sql = @"
        SELECT
            SUM(CASE WHEN DATEDIFF(YEAR, Birthdate, GETDATE()) BETWEEN 0 AND 18 THEN 1 ELSE 0 END) AS Teenagers,
            SUM(CASE WHEN DATEDIFF(YEAR, Birthdate, GETDATE()) BETWEEN 19 AND 50 THEN 1 ELSE 0 END) AS Adults,
            SUM(CASE WHEN DATEDIFF(YEAR, Birthdate, GETDATE()) >= 51 THEN 1 ELSE 0 END) AS Seniors
        FROM Members";

            var command = new SqlCommand(sql, connection);
            var reader = command.ExecuteReader();

            if (reader.Read())
            {
                var chartData = new
                {
                    labels = new[] { "Tinejdžeri (0-18)", "Odrasli (19-50)", "Stariji (50+)" },
                    datasets = new[]
                    {
                    new
                    {
                        data = new[]
                        {
                            reader["Teenagers"] != DBNull.Value ? Convert.ToInt32(reader["Teenagers"]) : 0,
                            reader["Adults"] != DBNull.Value ? Convert.ToInt32(reader["Adults"]) : 0,
                            reader["Seniors"] != DBNull.Value ? Convert.ToInt32(reader["Seniors"]) : 0
                        },
                        backgroundColor = new[] { "#4e73df", "#1cc88a", "#36b9cc" },
                        borderColor = new[] { "#ffffff", "#ffffff", "#ffffff" },
                        borderWidth = 1
                    }
                }
                };

                return JsonSerializer.Serialize(chartData);
            }

            // Ako nema podataka u bazi, vrati prazan graf
            var emptyChartData = new
            {
                labels = new[] { "Tinejdžeri (0-18)", "Odrasli (19-50)", "Stariji (50+)" },
                datasets = new[]
                {
                new
                {
                    data = new[] { 0, 0, 0 },
                    backgroundColor = new[] { "#4e73df", "#1cc88a", "#36b9cc" },
                    borderColor = new[] { "#ffffff", "#ffffff", "#ffffff" },
                    borderWidth = 1
                }
            }
            };

            return JsonSerializer.Serialize(emptyChartData);
        }
    }

    public string GetMembershipTypeChartData()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var sql = @"
            SELECT 
                Type AS MembershipType,
                COUNT(*) AS Total
            FROM Members
            GROUP BY Type";

            var command = new SqlCommand(sql, connection);
            var reader = command.ExecuteReader();

            var labels = new List<string>();
            var data = new List<int>();
            while (reader.Read())
            {
                labels.Add(reader["MembershipType"].ToString());
                data.Add(Convert.ToInt32(reader["Total"]));
            }

            var chartData = new
            {
                labels,
                datasets = new[]
                {
                new
                {
                    label = "Članarine",
                    data,
                    backgroundColor = new[] { "#4e73df", "#1cc88a", "#36b9cc", "#f6c23e" },
                    borderColor = "#ffffff",
                    borderWidth = 1
                }
            }
            };

            return JsonSerializer.Serialize(chartData);
        }
    }

    public string GetStartDateByMonthChartData()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var sql = @"
        SELECT 
            MONTH(StartDate) AS MonthNumber,
            COUNT(*) AS Total
        FROM Members
        GROUP BY MONTH(StartDate)
        ORDER BY MONTH(StartDate)";

            var command = new SqlCommand(sql, connection);
            var reader = command.ExecuteReader();

            var labels = new List<string>();
            var data = new List<int>();
            var cultureInfo = new CultureInfo("hr-HR"); // Postavi hrvatsku kulturu

            while (reader.Read())
            {
                var monthNumber = Convert.ToInt32(reader["MonthNumber"]);
                var monthName = cultureInfo.DateTimeFormat.GetMonthName(monthNumber); // Dohvati naziv mjeseca na hrvatskom
                labels.Add(monthName);
                data.Add(Convert.ToInt32(reader["Total"]));
            }

            var chartData = new
            {
                labels,
                datasets = new[]
                {
                new
                {
                    label = "Početak treninga po mjesecima",
                    data,
                    backgroundColor = "rgba(54, 162, 235, 0.5)",
                    borderColor = "rgba(54, 162, 235, 1)",
                    borderWidth = 1
                }
            }
            };

            return JsonSerializer.Serialize(chartData);
        }
    }




}
