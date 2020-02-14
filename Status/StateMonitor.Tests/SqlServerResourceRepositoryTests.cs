using Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using StateMonitor.Datastore;
using System.Configuration;

namespace StateMonitor.Tests
{
    public class SqlServerResourceRepositoryTests
    {
        public static string _connString;
        public static SqlServerResourceRepository _repo;
        public SqlServerResourceRepositoryTests()
        {
            _connString = ConfigurationManager.AppSettings["connString"];
            _repo = new SqlServerResourceRepository(_connString);
        }
        [Fact]
        public async Task GetAllResourcesTest()
        {
       
            IEnumerable<Resource> resources = await _repo.GetResources();
            Assert.NotNull(resources);
            Assert.True(resources.ToList().Count >0);
        }
       // [Theory]
        //[InlineData(false,new Resource(), "12-05-2018")]

        //public void buldClauseTest(bool hasClause, Resource resource, SqlCommand cmd)
        //{
            
        //}
    }
}
