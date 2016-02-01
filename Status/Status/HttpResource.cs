using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Status
{
   public class HttpResource: Resource
    {
       public HttpResource (Uri url){
         this.Url=url.AbsoluteUri;
         
       }
       
       public int ErrorCount { get; set; }
       public override async Task<dynamic> Poll()
       {
           string resMsg;
           HttpResponseMessage response;
           using (var client = new HttpClient()){
               client.DefaultRequestHeaders.Accept.Clear();
               response = await client.GetAsync(this.Url);
               
           }

           return response;
       }
    }
}
