using System.Configuration;
using Microsoft.Data.SqlClient;

namespace Crm.Data
{
    public static class DbConnector
    {
        public static SqlConnection GetConnection()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["crm"]?.ConnectionString;

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Die Verbindungszeichenfolge 'crm' wurde nicht gefunden.");

            return new SqlConnection(connectionString);
        }
    }
}