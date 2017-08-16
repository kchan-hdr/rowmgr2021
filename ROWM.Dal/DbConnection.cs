using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    internal class DbConnection
    {
        internal static readonly SqlConnectionStringBuilder _CONN_ = new SqlConnectionStringBuilder
        {
            //Data Source=octa-dev.database.windows.net;Initial Catalog=octa_dev;Integrated Security=False;User ID=stella;Password=********;Connect Timeout=15;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False
            DataSource = "octa-dev.database.windows.net",
            InitialCatalog = "rowm",
            IntegratedSecurity = false,
            ConnectTimeout = 15,
            UserID="rowm_app",
            Password = "SbhrDX6Cq5VPcR9z",
            Encrypt = false,
            TrustServerCertificate = false,
            MultipleActiveResultSets = true,
            ApplicationIntent = ApplicationIntent.ReadWrite,
            MultiSubnetFailover = false,
            ApplicationName = "ROWM"
        };

        internal static readonly SqlConnectionStringBuilder _CONN = new SqlConnectionStringBuilder
        {
            DataSource = @"(localdb)\MSSQLLocalDB",
            InitialCatalog = "rowm",
            IntegratedSecurity = true,
            MultipleActiveResultSets = true,
            ApplicationIntent = ApplicationIntent.ReadWrite,
            ApplicationName = "ROWM"
        };

        internal static string GetConnectionString() => _CONN.ConnectionString;

        internal static SqlConnection GetConnection() => new SqlConnection(GetConnectionString());
    }
}
