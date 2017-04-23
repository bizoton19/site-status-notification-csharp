# site-status-notification-csharp
site monitoring and notification in c# .net

This project is a port from Golang's https://golang.org/doc/codewalk/sharemem/ to using .NET's async await features.
The goal of this project was to provide a single CL executable that can poll different resources and in a generic way, report on their status in a given interval

## configuration base resources
the resources are currently being parsed in the configuration file.
each resource type follow a pattern in the config file.
  For http resources, the resource factory class doesn't need the type of resource specified, it simply looks for http at the begining of the resource identifier
  For other resources, a resource type is specified in the form of {resource type}:{resource}
