# site-status-notification-csharp
site monitoring and notification in c# .net

This project is a port from Golang's https://golang.org/doc/codewalk/sharemem/. It was expanded using .NET's async await features since we couldn't use golang. I'm not comparing the methodology here, .NET doesn't have `GO Routines` that share memory by communicating instead of communicating by sharing memory but the ease at which asynchronous operations can be performed now in .NET (with a few gotchas), helped with the task at hand.
The goal of this tool was to provide a single executable that can poll different resources and in a generic way, report on their status in a given interval. We simply wanted to know when a subset of resources were down but also we wanted to look for patterns (time of day and frequency or non OK responses)
It's a lightweight resource monitoring tool that runs on Windows only. It can help if :
* You don't have access to a big enterprise tool such as solarwinds.
* If you are running Windows and want to avoid the setup Nagios or the other tools out there, fantastic tools but may require more upfront time and work to setup.
* If you want total control over how resources are polled. The Status library is extensible, the resource implementations can be substituted.
* With this poller, i can on demand run several instances of the poller with different groups of resources or focus the polling on specific sets of resources, get notified by email, as opposed to drilling down several levels in a UI to finally find the status of a resource.


## Anatomy of Project
The core library is the Status library, the basic concept of polling is defined in there, including several default implementations of the `Resource` abstract class. Each implementation represent a different resource type `(Http, Server, IIS AppPool etc.)`. The default creation of resources is not very robust but a different implementation is possible by overriding the  the `AbstractResourceFactory` class.
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

## Pre-requisites
### For Execution
* Windows 7, Server 2008 , Server 2008R2, Server 2012, Windows 10.
* If running on background mode, SMTP server is required for email alert notifications.

### Configuration
The resources are currently being parsed in the configuration file.
Each resource type follow a pattern in the config file.
  For http resources, the resource factory class doesn't need the type of resource specified, it simply looks for http at the begining of the resource identifier. 
  For other resources, a resource type is specified in the form of `{resource type}:{resource}` or `{server}@{resource}:{resourcetype}` for resources running on server. The resources, of course, can come from a database or from anywhere as long as they can be created through an object that can return an anemic `Resource` type
  ```
  <appSettings>
    <!--Polling various HTTP endpoints -->
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

    public abstract class Resource : IPollable
    {
        public string Status { get; protected set; }
        public string Name { get; protected set; }
        internal string Url { get; set; }
        public abstract Task<State> Poll();
        public virtual bool Exist()
        {
            return !String.IsNullOrEmpty(Url);
        }
        public virtual string GetAbsoluteUri()
        {
            return this.Url;
        }
    }
```
### Security Assumptions
If the sample StatusPollConsole executable is used as is, the polling will be done with the current logged user's Windows security context. This means that any HTTP resources that implement single sign on, WMI resources or databases being polled will require the user running the poller to have the proper security.
## Getting Started
* Clone this git repo
* Open Solution with VS 2015 or >
* Add a config file with an appSettings section as shown in the configuration section above.
* Set the StatusPollConsole as the default project if not already set
* Run in debug if you wish or 
* There is , what i think is probably, the worst [Cake](https://cakebuild.net/) build script file in existence, that will build and deploy the executable and dependencies to the C:\Poller directory.
## Default Deployment
Simply execute the `build.ps1` script
to modify the deployment open the Cake script and make your changes there.




