using System;
using System.Collections.Generic;
using System.Text;
using Status;

namespace Status.Services
{
    public class ApplicationService
    {
        private IApplicationRepository _appRepo;
        public ApplicationService(IApplicationRepository appRepo)
        {
            _appRepo = appRepo;
        }
    }
}
