using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json;

namespace Status
{
    public class HttpResource : Resource
    {
        private State _state;
        public HttpResource(Uri url = null)
        {
            this.Url = url.AbsoluteUri;
            

        }

        public int ErrorCount { get; set; }
        public override async Task<State> Poll()
        {
            HttpResponseMessage response;
            using (var handler = new HttpClientHandler { UseDefaultCredentials = true, ClientCertificateOptions = ClientCertificateOption.Automatic })
            using (var client = new HttpClient(handler))
            {

                client.DefaultRequestHeaders.Accept.Clear();
                client.Timeout = TimeSpan.FromSeconds(10);

                response = await client.GetAsync(this.Url, HttpCompletionOption.ResponseContentRead);
                response = response.StatusCode == HttpStatusCode.Unauthorized ? await KnownHttpErrorResponseHandler(client, response.StatusCode) : response;
                if (response.StatusCode != HttpStatusCode.BadGateway && response.StatusCode != HttpStatusCode.RequestTimeout)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    content = HttpStateStatusFactory.CreateStatusMessageForFalsePositives(content);
                    _state = string.IsNullOrEmpty(content) ? new State() { Status = response.StatusCode.ToString(), Url = Url,Description=JsonConvert.SerializeObject(response.Headers) }
                    : new State()
                    {
                        Status = content,
                        Url = this.Url
                    };
                }
                else
                {

                    
                    _state = new State()
                    {

                        Status = response.StatusCode.ToString(),
                        Description = "Web Page Is Not Responding, Requests Are Timing Out",

                        Url = this.Url
                    };
                }

            }
            return _state;
        }



        protected virtual async Task<HttpResponseMessage> KnownHttpErrorResponseHandler(HttpClient client, HttpStatusCode code)
        {
            HttpResponseMessage response = null;
            if (code == HttpStatusCode.Unauthorized)
            {
                
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "Uk1TOjMyMzI1ZWEzLWE2YmQtNGRlOC1iYTU2LTY5MDUwYTg0ZDg2ZA==");
                response = await client.GetAsync(this.Url, HttpCompletionOption.ResponseContentRead);
                

            }

            return response;
        }
    }

    public class HttpStateStatusFactory
    {


        public static string CreateStatusMessageForFalsePositives(string content)
        {

            if (content.Contains("Under Maintenance"))
            {
                content = "Website is reponding but with error pages, please check servers. app pools, web server";
            }
            else
            {
                content = string.Empty;
            }

            return content;

        }
    }



}
