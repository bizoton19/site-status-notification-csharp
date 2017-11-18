using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Status
{
    /// <summary>
    /// This abstract class defines
    /// </summary>
   public abstract class DbProviderNameAbstractFactory
    {
        public abstract string GetProviderName(DatabaseType databaseType);
        
           
    }
}
