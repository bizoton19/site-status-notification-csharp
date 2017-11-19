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
the resources are currently being parsed in the configuration file.
each resource type follow a pattern in the config file.
  For http resources, the resource factory class doesn't need the type of resource specified, it simply looks for http at the begining of the resource identifier
  For other resources, a resource type is specified in the form of {resource type}:{resource}


## Getting Started
