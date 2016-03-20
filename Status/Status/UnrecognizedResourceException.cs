using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Status
{
    public class UnrecognizedResourceException: Exception
    {
         public UnrecognizedResourceException(): 
            base("The specified resource type is not processable currently, please see the documentation on how to register new resource types")
        {

        }
    }
}
