using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Status.Factories
{
  public abstract class AbstractFactory
    {
       public  abstract Resource GetResource(string resourceName);
       
    }
}
