using Status;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMonitor.Datastore
{
    public class SqlServerApplicationRepository:IApplicationRepository, IDisposable
    {

        private static SqlConnection _connection;

        public SqlServerApplicationRepository(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
        }
        public async Task<IEnumerable<Resource>> GetApplications(string partitionKey = "",int currIndex=0,int pageSize = 25, Resource resource = null)
        {
            //connect to sql to get apps.
            IList<Application> apps = new List<Application>();
            string sql = "SELECT A.[Id] AS AppId" +
                        ",A.[Name] AS AppName " +
                        ",A.[Description] As AppDescription " +
                        ",R.Id AS ResourceId " +
                        "FROM [dbo].[Application] A " +
                        "JOIN [dbo].AppResource AR on AR.ApplicationId = A.Id " +
                        "JOIN [dbo].[Resource] R ON R.Id = AR.ResourceId " +
                        "JOIN [dbo].[ResourceType] RT ON R.Type = RT.Id ";

            SqlCommand cmd = new SqlCommand(sql, _connection);
            await cmd.Connection.OpenAsync();
            SqlDataReader reader = await cmd.ExecuteReaderAsync();
            Status.Application obj;
            int appId;
            string appName, appDesc, resId;
            while (await reader.ReadAsync())
            {
                appId = Convert.ToInt32(reader["AppId"]);
                appName = reader["AppName"].ToString();
                appDesc = reader["AppDescription"].ToString();
                resId = reader["ResourceId"].ToString();
                obj = new Application(new Status.ApplicationId(appId), appName, appDesc);
                obj.Resources = new List<ResourceId>();
                obj.Resources.Add(new ResourceId());
                apps.Add(obj);

            }
           
            return apps;
        }
        public async Task<bool> ApplicationExist(Status.ApplicationId appId)
        {
            bool exist = false;
            string sql = "SELECT Id FROM dbo.Application WHERE id = @Id";
            SqlCommand cmd = new SqlCommand(sql, _connection);
            if (_connection.State != System.Data.ConnectionState.Open)
            {
                await cmd.Connection.OpenAsync();
            }
            cmd.Parameters.Add(new SqlParameter("@Id", appId.Id));
            SqlDataReader reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                exist = Convert.ToInt32(reader["Id"])==appId.Id? true:false;
            }
            reader.Close();
            cmd.Dispose();
           
            return exist;
        }
        public async Task CreateOrUpdateApplication(Application app,List<Resource> resources)
        {
            string appSql;
            if(_connection.State == System.Data.ConnectionState.Closed)
            {
                await _connection.OpenAsync();
            }
            
            
            if (await ApplicationExist(app.AppId)){
                SqlCommand upcmd = _connection.CreateCommand();
                appSql = " UPDATE [dbo].[Application] SET Name = @Name" +
                         ", Description = @Description " +
                         ", WHERE Id = @Id";
                SqlCommand contactCmd = _connection.CreateCommand();
                upcmd.Parameters.Add(new SqlParameter("Name", app.Name));
                upcmd.Parameters.Add(new SqlParameter("Description", app.Description));
                upcmd.Parameters.Add(new SqlParameter("Id", app.AppId.Id));

                await upcmd.ExecuteNonQueryAsync();
                upcmd.Dispose();
            }
            else
            {
                SqlCommand cmd = _connection.CreateCommand();
                SqlCommand contactCmd = _connection.CreateCommand();
                SqlCommand resourceCmd = default(SqlCommand);
                SqlCommand appResourceCmd = default(SqlCommand);
                appSql = "INSERT INTO [dbo].[Application] " +
                         "([Name] " +
                         ",[Description] " +
                         ",[Insert_TimeStamp] " +
                         ",[Update_TimeStamp]) " +
                  "VALUES " +
                        "(@Name" +
                        ",@Description " +
                        ",@Insert_TimeStamp "+
                        ",@Update_TimeStamp); SELECT SCOPE_IDENTITY(); ";

                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = _connection;
                cmd.Parameters.AddWithValue("@Name", app.Name);
                cmd.Parameters.AddWithValue("@Description", app.Description);
                cmd.Parameters.AddWithValue("@Insert_TimeStamp", DateTime.UtcNow.ToString());
                cmd.Parameters.AddWithValue("@Update_TimeStamp", DateTime.UtcNow.ToString());
                SqlTransaction transaction;
               
                transaction = _connection.BeginTransaction();
                cmd.Connection = _connection;
                cmd.Transaction = transaction;
                try
                {
                    cmd.CommandText = appSql;
                    Int32 appid = Convert.ToInt32(cmd.ExecuteScalar());
                    app.AppId = new Status.ApplicationId(appid);
                    var resourcesCmd = CreateResourcesCmd(resources);
                    app.Resources = new List<ResourceId>();
                    foreach (var r in resourcesCmd)
                    {
                        r.Transaction = transaction;
                        Int32 resId = Convert.ToInt32(r.ExecuteScalar());
                        app.Resources.Add(new ResourceId(resId));
                    }
                       
              
                    Int32 appResId;
                    var appResCmd = CreateOrUpdateAppResources(app);
                    foreach(var c in appResCmd)
                    {
                         c.Transaction = transaction;
                         appResId = c.ExecuteNonQuery();
                         
                    }

                    contactCmd.Transaction = transaction;
                    await CreateOrUpdateAppContactCmd(app, contactCmd).ExecuteNonQueryAsync();

                    transaction.Commit();
                
                }
                catch (Exception ex)
                {
                    try
                    {
                        transaction.Rollback();
                        
                    }
                    catch (Exception rollbkExcep)
                    {
                        
                    }
                    
                    
                }
                finally
                {
                   
                    //cmd.Dispose();
                    //resourceCmd.Dispose();
                    //contactCmd.Dispose();
                    //appResourceCmd.Dispose();
 
                }


            }

            

        }
        private List<SqlCommand> CreateOrUpdateAppResources(Application app)
        {
            List<SqlCommand> appRescmd = new List<SqlCommand>();
            StringBuilder resourceSql;
            foreach (var item in app.Resources)
            {
               SqlCommand cmd = _connection.CreateCommand();
               resourceSql = new StringBuilder("INSERT INTO [dbo].[AppResource] " +
                     "([ApplicationId] " +
                     ",[ResourceId] " +
                     ",[Insert_TimeStamp] " +
                     ",[Update_TimeStamp]) " +
                      "VALUES"  +
                      "(" +
                     "@ApplicationId " +
                     ",@ResourceId " + 
                     ",@Insert_TimeStamp " +
                     ",@Update_TimeStamp); SELECT SCOPE_IDENTITY(); ");

                cmd.CommandText = resourceSql.ToString();
                cmd.Parameters.AddWithValue("@ApplicationId", app.AppId.Id);
                cmd.Parameters.AddWithValue("@ResourceId", item.Id);
                cmd.Parameters.AddWithValue("@Insert_TimeStamp", DateTime.UtcNow.ToString());
                cmd.Parameters.AddWithValue("@Update_TimeStamp", DateTime.UtcNow.ToString());
                appRescmd.Add(cmd);
            }
            return appRescmd;
        }

        private SqlCommand CreateOrUpdateAppContactCmd(Application app,SqlCommand contactCmd)
        {

            string appContactSql = "INSERT INTO [dbo].[ApplicationContact] " +
                       "([ApplicationId] " +
                       ",[FirstName] " +
                       ",[LastName] " +
                       ",[Email] " +
                       ",[Phone] " +
                       ",Insert_TimeStamp " +
                      ",Update_TimeStamp) " +

                 "VALUES " +
                      "(@ApplicationId" +
                      ",@FirstName " +
                      ",@LastName " +
                      ",@Email " +
                      ",@Phone " +
                      ",@Insert_TimeStamp " +
                      ",@Update_TimeStamp); SELECT SCOPE_IDENTITY(); ";

            contactCmd.CommandText = appContactSql;
            contactCmd.Parameters.AddWithValue("@ApplicationId", app.AppId.Id);
            contactCmd.Parameters.AddWithValue("@FirstName", app.Contact.FirstName);
            contactCmd.Parameters.AddWithValue("@LastName", app.Contact.LastName);
            contactCmd.Parameters.AddWithValue("@Email", app.Contact.Email);
            contactCmd.Parameters.AddWithValue("@Phone", app.Contact.Phone);
            contactCmd.Parameters.AddWithValue("@Insert_TimeStamp", DateTime.UtcNow.ToString());
            contactCmd.Parameters.AddWithValue("@Update_TimeStamp", DateTime.UtcNow.ToString());

            return contactCmd;

        }
        private List<SqlCommand> CreateResourcesCmd(List<Resource> resources)
        {
            //resource section
            List<SqlCommand> cmds = new List<SqlCommand>();
            foreach (var resource in resources)
            {
                SqlCommand resCmd = _connection.CreateCommand();
                StringBuilder resourceSql = new StringBuilder("INSERT INTO [dbo].[Resource] " +
                         "([Name] " +
                         ",[Type] " +
                         ",[URL] " +
                         ",[Description] " +
                         ",[Environment_Id] " +
                         ",[Insert_TimeStamp] " +
                         ",[Update_TimeStamp]) " +
                  "VALUES " +
                        "(@Name" +
                        ",@Type" +
                        ",@URL" +
                        ",@Description" +
                        ",@Environment_Id" +
                        ",@Insert_TimeStamp" +
                        ",@Update_TimeStamp); SELECT SCOPE_IDENTITY(); ");

                resCmd.CommandText = resourceSql.ToString();
                resCmd.Parameters.AddWithValue($"@Name", resource.Name);
                resCmd.Parameters.AddWithValue($"@Type", resource.Type);
                resCmd.Parameters.AddWithValue($"@URL", resource.Url);
                resCmd.Parameters.AddWithValue($"@Description", resource.Description);
                resCmd.Parameters.AddWithValue($"@Environment_Id", 1);//Prod
                resCmd.Parameters.AddWithValue($"@Insert_TimeStamp", DateTime.UtcNow.ToString());
                resCmd.Parameters.AddWithValue($"@Update_TimeStamp", DateTime.UtcNow.ToString());
                cmds.Add(resCmd);

            }
            return cmds;
        }
        

        public void Dispose()
        {
            _connection.Close();
        }
    }
    }
