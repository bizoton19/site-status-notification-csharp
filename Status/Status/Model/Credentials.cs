using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;

namespace Status
{
    public class Credentials
    {
        private string _userName;
        private SecureString _password;

        public string UserName
        {
            get
            {
                return _userName;
            }

           private set
            {
                _userName = value;
            }
        }

        public SecureString Password
        {
            get
            {
                return _password;
            }

          private  set
            {
                _password = value;
            }
        }

        public  Credentials (string userName, SecureString password)
        {
            Password = password;
            UserName = userName;
        }
   

}
   
}
