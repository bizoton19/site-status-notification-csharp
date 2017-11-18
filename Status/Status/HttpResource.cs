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
        /// <summary>
        /// Defines the _state
        /// </summary>
        private State _state;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResource"/> class.
        /// </summary>
        /// <param name="url">The <see cref="Uri"/></param>
        public HttpResource(Uri url = null)
        {
            this.Url = url.AbsoluteUri;
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
            using (var handler = new HttpClientHandler { UseDefaultCredentials = true, ClientCertificateOptions = ClientCertificateOption.Automatic })
            using (var client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.Timeout = TimeSpan.FromSeconds(10);

                response = await client.GetAsync(this.Url, HttpCompletionOption.ResponseContentRead);
   
                if (response.StatusCode != HttpStatusCode.BadGateway && response.StatusCode != HttpStatusCode.RequestTimeout)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    content = HttpStateStatusFactory.CreateStatusMessageForFalsePositives(content);
                    _state = string.IsNullOrEmpty(content) ? new State()
                    {
                        Status = response.StatusCode.ToString(),
                        Url = Url,
                        Description = JsonConvert.SerializeObject(response.Headers)
                    }
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
