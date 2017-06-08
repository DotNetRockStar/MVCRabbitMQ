using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon.Framework.Data.Repository
{
    internal class DefaultRepositoryBase
    {
        private readonly string _connectionString;
        public DefaultRepositoryBase()
        {
            _connectionString = ConfigSectionUtility.ConfigSection.RulesConnectionString;
        }

        protected bool IsConnectionStringPresent
        {
            get
            {
                return !string.IsNullOrWhiteSpace(_connectionString);
            }
        }

        protected SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
