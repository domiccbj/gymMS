using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMembershipManagement.DataAccess
{
    public class DatabaseConnection
    {
        public string ConnectionString { get; }

        public DatabaseConnection(string connectionString)
        {
            ConnectionString = connectionString;
        }
    }
}
