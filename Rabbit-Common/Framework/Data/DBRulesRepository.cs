using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon.Framework.Data
{
    /// <summary>
    /// Sql implementatino of IRulesRepository.
    /// </summary>
    public class DBRulesRepository : IRulesRepository
    {
        private readonly string _sqlConnectionString;
        public DBRulesRepository()
        {
            _sqlConnectionString = ConfigSectionUtility.RulesConnectionString;
            if (string.IsNullOrWhiteSpace(_sqlConnectionString))
                throw new Exception("Unable to connect to rules database.  Make sure that <rabbit> configSection is registered in your config and <rulesConnectionString> is set.");
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_sqlConnectionString);
        }

        public bool IsQueueApproved(string host, string queueName)
        {
            bool isApproved = false;

            using (var connection = GetConnection())
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(@"SELECT count(id) 
FROM [Queue] 
WHERE (
        [Host] = @Host AND 
        [Name] = @Name AND 
        IsApproved = 1
      ) 
      OR
      (
        [Host] = @Host AND 
        [Name] LIKE '%*%' AND
	    @Name LIKE REPLACE([Name], '*', '%')
      )", connection);
                cmd.Parameters.Add(new SqlParameter("@Host", host));
                cmd.Parameters.Add(new SqlParameter("@Name", queueName));
                var rdr = cmd.ExecuteReader();

                if (rdr.Read())
                {
                    int val = Convert.ToInt32(rdr[0]);
                    if (val > 0)
                        isApproved = true;
                }
            }

            return isApproved;
        }

        public bool AreServerRulesEnforced(string host)
        {
            return true;
        }

        public bool IsExchangeApproved(string host, string name, ExchangeType type)
        {
            bool isApproved = false;

            using (var connection = GetConnection())
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT count(id) FROM [Exchange] WHERE [Host] = @Host AND [Name] = @Name AND [Type] = @Type AND IsApproved = 1", connection);
                cmd.Parameters.Add(new SqlParameter("@Host", host));
                cmd.Parameters.Add(new SqlParameter("@Name", name));
                cmd.Parameters.Add(new SqlParameter("@Type", type.ToString()));
                var rdr = cmd.ExecuteReader();

                if (rdr.Read())
                {
                    int val = Convert.ToInt32(rdr[0]);
                    if (val > 0)
                        isApproved = true;
                }
            }

            return isApproved;
        }
    }
}
