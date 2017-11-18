using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Data;

namespace Status
{
   public abstract class Database: Resource
    {
        
        private Server _serverInstance;
        private string _dbName;
        Credentials _credentials;
        DatabaseType _dbType;

        public Server ServerInstance
        {
            get
            {
                return _serverInstance;
            }

          
        }

        public string DbName
        {
            get
            {
                return _dbName;
            }

          
        }

        

        public DatabaseType DbType
        {
            get
            {
                return _dbType;
            }

            
        }

        

        private string _connectionString;
        private State _state;
        public Database(Server serverInstance, string databaseName,DatabaseType dbType,string connectionstring, Credentials credentials )
        {
            _serverInstance = serverInstance;
            _dbName = databaseName;
            _credentials = credentials;
            _connectionString = connectionstring;
            this.Url = string.Concat(this.ServerInstance.ServerName, ".", this.DbName);
            _state  = new State();

        }

        public override async Task<State> Poll()
        {
            //DbProviderNameAbstractFactory.GetProviderName(DbType)
            DbProviderFactory factory = DbProviderFactories.GetFactory("");

            
            using (DbConnection conn = factory.CreateConnection())
            {
                try
                {
                    conn.ConnectionString = this._connectionString;
                    await conn.OpenAsync();
                }
                catch (DbException ex)
                {
                    _state.Status = ex.Message;
                    _state.Url = this.Url;
                }
                finally
                {
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        _state.Status = "OK";
                        _state.Url = this.Url;
                    }
                }
            }

            return _state;
        }
    }
}
