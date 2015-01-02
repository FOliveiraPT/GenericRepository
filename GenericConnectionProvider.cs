using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Edm;

namespace GenericRepository
{
    public class GenericConnectionProvider
    {
        public EntityConnection GenericConnection { get; private set; }

        public GenericConnectionProvider(string serverName, string dataBaseName, string modelFileName)
        {
            GenericConnection = EstablishSQLConnection(serverName, dataBaseName, modelFileName);
        }

        private EntityConnection EstablishSQLConnection(string serverName, string dataBaseName, string modelFileName)
        {
            var sqlBuilder = new SqlConnectionStringBuilder();
            sqlBuilder.DataSource = serverName;
            sqlBuilder.InitialCatalog = dataBaseName;
            sqlBuilder.IntegratedSecurity = true;
            sqlBuilder.MultipleActiveResultSets = true;

            var entityBuilder = new EntityConnectionStringBuilder();
            entityBuilder.Provider = "System.Data.SqlClient";
            entityBuilder.ProviderConnectionString = sqlBuilder.ToString();
            entityBuilder.Metadata = string.Format("res://*/{0}.csdl|res://*/{0}.ssdl|res://*/{0}.msl", modelFileName);

            return new EntityConnection(entityBuilder.ToString());
        }
    }
}
