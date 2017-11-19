# site-status-notification-csharp
site monitoring and notification in c# .net

This project is a port from Golang's https://golang.org/doc/codewalk/sharemem/. It was created using .NET's async await features since we couldn't use golang. 
The goal of this project was to provide a single CL executable that can poll different resources and in a generic way, report on their status in a given interval.
It's a lightweight resource monitoring tool that that runs on Windows only. It can help if :
* You don't have access to a big enterprise tool such as solarwinds
* If you are running Windows and want to avoid the setup of nagios or the other tools out there, who are fantastic tools but may require    more upfront time investment to setup.
* If you want total control over how resources are polled. The Status library is extensible, the resource implementations can be substituted.


## Anatomy of Project
The core library is the Status library, the basic concept of polling is defined in there and there several default implementations of the `IPoller` interface. Each implementation represent a different resource type. The default creation of resources is not very robust but a different implementation is possible by overriding the  the `AbstractResourceFactory` class.
Currently, the `ResourceFactory` class implements the `AbstractResourceFactory` class:

``` 
public class ResourceFactory:AbstractResourceFactory
    {
       
        
        
    }
```
It's used in the `StatusPollConsole` sample program like so :

``` 
urls.ForEach(s =>
{
   Resource r = new ResourceFactory().GetResource(s);
   if (r.Exist())
    resources.Add(r);
   else
     Console.WriteLine("Unrecognizable resource: {0} . Please verify config file", string.Concat(r.Name, "@", r.GetAbsoluteUri()));
});
```

## Configuration Based Resources

## Pre-requisites
### For Execution
* Windows 7, Server 2008 , Server 2008R2, Server 2012, Windows 10.
* If running on background mode, SMTP server is required for email alert notifications.

### Configuration
The resources are currently being parsed in the configuration file.
Each resource type follow a pattern in the config file.
  For http resources, the resource factory class doesn't need the type of resource specified, it simply looks for http at the begining of the resource identifier
  For other resources, a resource type is specified in the form of `{resource type}:{resource}` or `{server}@{resource}:{resourcetype}` for resources running on server. The resources, of course, can come from a database or from anywhere as long as they can be created through an object that can return an anemic `Resource` type
  ```
  <appSettings>
    <!--Polling HTTP endpoints app pool on the castleblack server-->
    <!--Polling/Pinging Servers by name or by IP address-->
    <!--Polling a RabbitMQ  runing as a Windows Service on the  castleblack server-->
    <!--Polling HTTP endpoints app pool on the castleblack server-->
    <add key="resources" value="https://www.cloud.gov,
         https://www.hdwih.com,
         https://www.amazon.com,
         stormsend:server,
         castleblack:server,
         castleblack@RabbitMQService:WindowsService, 
         castleblack@DefaultAppPool@IISAppool"/> 
    <add key="To" value="" />
    <add key="mode" value="console"/>
    <add key="PollingInterval" value="20000" />
  </appSettings>
  ```
#### The Resource Record Type
```
/// <summary>
    /// Defines the <see cref="Resource" />
    /// </summary>
    public abstract class Resource : IPollable
    {
        /// <summary>
        /// Gets or sets the Status
        /// </summary>
        public string Status { get; protected set; }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets or sets the Url
        /// </summary>
        internal string Url { get; set; }

        /// <summary>
        /// The Poll
        /// </summary>
        /// <returns>The <see cref="Task{State}"/></returns>
        public abstract Task<State> Poll();

        /// <summary>
        /// The Exist
        /// </summary>
        /// <returns>The <see cref="bool"/></returns>
        public virtual bool Exist()
        {
            return !String.IsNullOrEmpty(Url);
        }

        /// <summary>
        /// The GetAbsoluteUri
        /// </summary>
        /// <returns>The <see cref="string"/></returns>
        public virtual string GetAbsoluteUri()
        {
            return this.Url;
        }
    }
```
### Security Assumptions
If the sample StatusPollConsole executable is used as is, the polling will be done with the current logged user's Windows security context. This means that any HTTP resources that implement single sign on, WMI resources or databases being polled will require the user running the poller to have the proper security.
## Getting Started
