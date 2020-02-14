using StateMonitor.Datastore;
using Status;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StateMonitor.Tests
{
    public class SqlServerApplicationRepositoryTests
    {

        public static string _connString;
        public static SqlServerApplicationRepository _repo;
        public SqlServerApplicationRepositoryTests()
        {
            _connString = ConfigurationManager.AppSettings["connString"];
            _repo = new SqlServerApplicationRepository(_connString);
        }
        [Fact]
        public async Task GetAllResourcesTest()
        {

            IEnumerable<Resource> apps = await _repo.GetApplications();
            Assert.NotNull(apps);
            Assert.True(apps.ToList().Count > 0);
        }
        [Fact]
        public async Task CreateOrUpdateApplicationTest()
        {
            Application app = new Application(new Status.ApplicationId(0),"StateContractor","State contractors working with field investigation");
            ApplicationContact contact = new ApplicationContact()
            {
                Email = "contact@gmailxc.com",
                FirstName = "Ale",
                LastName = "Sal",
                Phone = "444-702-3240"

            };
            Resource res = new Server("Aurelia", "1.0.2.76");
            res.Type = "2";
            res.Description = "Test Server";
            Resource http = new HttpResource(new Uri("https://aurelia-serv.gov"));
            http.Type = "1";
            http.Description="test url";
            
            List<Resource> resources = new List<Resource>();
            resources.Add(res);
            resources.Add(http);
            app.Contact = contact;
            app.Resources = resources.Select(id=>id.ResourceId).ToList();
            SqlServerApplicationRepository appRepo = new SqlServerApplicationRepository(_connString);
            await appRepo.CreateOrUpdateApplication(app, resources);
            appRepo.Dispose();
            Assert.True(app.AppId.Id > 0);
            


        }
        [Fact]
        public async Task RemoveApplicationTest()
        {

        }
        [Fact]
        public async Task UpdateApplicationTest()
        {

        }
    }
}
