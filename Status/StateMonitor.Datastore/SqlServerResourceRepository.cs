using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Status;
using System.Data.SqlClient;
using System.Reflection;

namespace StateMonitor.Datastore
{
    public class SqlServerResourceRepository : IResourceRepository,IDisposable
    {
        private static SqlConnection _connection;

        public SqlServerResourceRepository(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
        }
        public async Task<IEnumerable<Resource>> GetResources(string partitionKey = "", Resource clause=null)
        {
            //connect to sql to get resources.
            IList<Resource> resources = new List<Resource>();
            string resType = "http";
            string resName = "http";
            string resUri = "http://www.cpsc.gov";
            int resId;
            bool hasClause = clause!=null?true:false;
            string sql = "SELECT r.[Id] " +
                        ",r.[Name] " +
                        ",rt.[Name] AS Resource_Type_Name" +
                        ",rt.[Description] AS Type_Description" +
                        ",[URL] " +
                        ",r.[Description] " +
                        ",e.[Name] " +
                        ",e.[Description] " +
                        ",r.[Update_TimeStamp] " +
                        ",r.Insert_TimeStamp " +
                        "FROM [Resource] r " +
                        "JOIN [Environment] e on e.Id = r.Environment_Id " +
                        "JOIN [ResourceType] rt on rt.Id = r.[Type]" ;
            
            SqlCommand cmd = new SqlCommand(sql, _connection);
            cmd = buildClause(hasClause,clause,cmd);
            await cmd.Connection.OpenAsync();
            SqlDataReader reader = await cmd.ExecuteReaderAsync();
            Resource obj;
            while (await reader.ReadAsync())
            {
                resId = Convert.ToInt32(reader["Id"]);
                resType = reader["Resource_Type_Name"].ToString();
                resUri = reader["URL"].ToString();
                resName = reader["Name"].ToString();
                obj = new ResourceFactory().GetResource(resUri, resType, resName);
                obj.Type = resType;
                obj.ResourceId = new ResourceId(resId);//TODO: move out to construcor
                obj.Description = reader["Description"].ToString();
                obj.TypeDescription = reader["Type_Description"].ToString();
                resources.Add(obj);

            }
            cmd.Connection.Close();
            return resources;
        }

        private SqlCommand buildClause(bool hasClause, Resource resource,SqlCommand cmd)
        {
            
            FieldInfo[] fi = typeof(Resource).GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var info in fi)
            {
               string fieldName = info.Name;
               string dbFieldName;
                if ( fieldName != null)
                {
                    dbFieldName = fieldName == "Name" ? fieldName :
                        fieldName == "Type" ? "Resource_Type_Name" :
                        fieldName == "Url" ? "URL" :
                        fieldName == "Description" ? fieldName :
                        fieldName == "TypeDescription" ? "Type_Description" : string.Empty;
                    if (!cmd.CommandText.ToLowerInvariant().Contains("WHERE"))//possibly extend CommandText class to check for clause
                    {
                        cmd.CommandText += $"WHERE {dbFieldName} = @dbFieldName";
                        cmd.Parameters.AddWithValue("@dbFieldName",info.GetValue(null).ToString());
                       
                    }
                    else
                    {
                        cmd.CommandText += $"AND {dbFieldName} = @dbFieldName";
                        cmd.Parameters.AddWithValue("@dbFieldName", info.GetValue(null).ToString());
                    }
                }

                

            }

            return cmd;

        }

        public Task Save(ICollection<State> state)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _connection.Close();
        }
    }
}
