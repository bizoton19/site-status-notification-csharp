namespace Status
{
    using Newtonsoft.Json;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="HttpResource" />
    /// </summary>
    public class HttpResource : Resource
    {
        static HttpClientHandler handler = new HttpClientHandler { UseDefaultCredentials = true, ClientCertificateOptions = ClientCertificateOption.Automatic };
        static HttpClient client = new HttpClient(handler);
        /// <summary>
        /// Defines the _state
        /// </summary>
        private State _state;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResource"/> class.
        /// </summary>
        /// <param name="url">The <see cref="Uri"/></param>
        public HttpResource(Uri url = null,string name="")
        {
            this.Url = url.AbsoluteUri;
            this.Name = string.IsNullOrEmpty(name) ? url.ToString(): name;
        }

        /// <summary>
        /// Gets or sets the ErrorCount
        /// </summary>
        public int ErrorCount { get; set; }

        /// <summary>
        /// The Poll
        /// </summary>
        /// <returns>The <see cref="Task{State}"/></returns>
        public override async Task<State> Poll()
        {
            HttpResponseMessage response;
                response = await client.GetAsync(this.Url, HttpCompletionOption.ResponseContentRead);
   
                if (response.StatusCode != HttpStatusCode.BadGateway && response.StatusCode != HttpStatusCode.RequestTimeout)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    content = HttpStateStatusFactory.CreateStatusMessageForFalsePositives(content);
                    _state = string.IsNullOrEmpty(content) ? new State()
                    {
                        Status = response.StatusCode.ToString(),
                        Url = Url,
                        Description = JsonConvert.SerializeObject(response.Headers),
                        Type = this.GetType().Name
                    }
                    : new State()
                    {
                        Status = content,
                        Url = this.Url,
                        Type = this.GetType().Name
                    };
                }
                else
                {
                    _state = new State()
                    {
                        Status = response.StatusCode.ToString(),
                        Description = "Web Page Is Not Responding, Requests Are Timing Out",
                        Type = this.GetType().Name,
                        Url = this.Url
                    };
                }
            
            return _state;
        }

        
    }

    /// <summary>
    /// Defines the <see cref="HttpStateStatusFactory" />
    /// </summary>
    public class HttpStateStatusFactory
    {
        /// <summary>
        /// The CreateStatusMessageForFalsePositives
        /// </summary>
        /// <param name="content">The <see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
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
