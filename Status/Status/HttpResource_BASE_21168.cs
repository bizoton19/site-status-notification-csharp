using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Status
{
   public class HttpResource: Resource
    {
        private State _state;
       public HttpResource (Uri url=null){
         this.Url=url.AbsoluteUri;
         
       }
       
       public int ErrorCount { get; set; }
       public override async Task<State> Poll()
       {
           HttpResponseMessage response;
            using (var handler = new HttpClientHandler { UseDefaultCredentials = true })
            using (var client = new HttpClient(handler)){

               client.DefaultRequestHeaders.Accept.Clear();
               client.Timeout = TimeSpan.FromSeconds(100);
               response = await client.GetAsync(this.Url,HttpCompletionOption.ResponseHeadersRead);
                
               if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    response = await client.GetAsync(this.Url,HttpCompletionOption.ResponseHeadersRead);
                    
                }
                _state = new State() { Status = response.StatusCode.ToString(), Url = this.Url };
            }
           return _state;
       }
    }
}
