![GitHub last commit (branch)](https://img.shields.io/github/last-commit/MunroRaymaker/eventmanager-webapi/main)
![Build and Test](https://github.com/MunroRaymaker/eventmanager-webapi/workflows/Build%20and%20Test/badge.svg)

# EventManager WebAPI

## Brief description
This an ASP.NET Core WebApi demo project for showcasing a solution to keep track of jobs and their progress.
It implements a minimal job scheduling solution exposed via a web API. It makes it possible for clients to enqueue an array of numbers to be
sorted in the background and to query the state of any previously enqueued job.

---

## Contents

- [Requirements](#requirements)
- [Design](#design)
  - [Surviving application restarts](#surviving-application-restarts)
  - [Running in multihosted environments](#running-in-multihosted-environments)
  - [Separation of concerns](#separation-of-concerns)
- [Technologies](#technologies)
- [Technical setup](#technical-setup)
- [Code structure](#code-structure)
- [Usage](#usage)  
  - [Get All](#get-all)
  - [GetById](#getbyid)
  - [AddJob](#addjob)
- [Logging](#logging)
- [Deployment](#deployment)
- [vNext](#vnext)
- [References](#references)


---

## Requirements
The design has these requirements:

* The client can enqueue a new job, by providing an unsorted array of numbers as input
* The client can retrieve an overview of all jobs (both pending and completed)
* The client can retrieve a specific job by its ID (see below), including the output (sorted array) if the job has completed

A job takes an unsorted array of numbers as input, sorts the input using an algorithm of your choice, and outputs the sorted array. 
Apart from the input and output arrays a job should include the following metadata:

* An ID - a unique ID assigned by the application
* A timestamp - when was the job enqueued?
* A duration - how much time did it take to execute the job?
* A status - for example "pending" or "completed"

All jobs should be processed in the background and clients should not forced to wait for jobs to complete.
To view the output of a job (the sorted array), the client must query a previously enqueued job.

---

## Design
The design is based on a fire-and-forget pattern. The client sends a job to the api, and receives an id back immediatly. The client must poll the api 
to see the progress of the job. In this simplistic solution we don't keep track of partially completed jobs, they only have two states - either pending or completed.

The eventmanager api should defer the actual CPU processing to another background thread. This makes the design scalable so many requests can be received, without depleting the IIS thread pool.

This design will store all data in memory to keep things simple, but it should be trivial to add a database later on, like sql server or sqlite.

So there will be three components to the solution:
* WebAPI - responsible for receiving jobs and getting status
* A `QueuedWorkerService` which submits events, checks completion and returns results
* A `BackendTaskQueue` which holds the actual queue, and ensured thread safe access to enqueuing and dequeueing.

The design uses the `IHostedService` interface from Microsoft.Extensions.Hosting to create the background queue which listens for events, and processes them.

But beware - there be dragons with this design.

### Surviving application restarts
The application could restart for a number of reasons, so we should think about persisting the queue of jobs if the AppDomain is lost. 
If everything is kept in memory the queue will be lost if eg. the server is rebooted or the application pool recycles. We loose track of how far the processing went, and jobs could be lost or data corrupted. 
The IHostedService however will register the backgroundservice within the application lifetime environment, so when the AppDomain shuts down, a signal will be sent and the StopAsync method will be called.
This gives the service time to gracefully shutdown, and release any locks. The default timeout is only 5 seconds, so it is increased to 30 seconds in Startup.cs:

``` csharp
services.Configure<HostOptions>(options => options.ShutdownTimeout = TimeSpan.FromSeconds(30));
```

### Running in multihosted environments
The design also has a flaw in that if the site runs in a web farm, you could end up with multiple instances of the app that all attempt to run the same task at the same time.

### Separation of concerns
The web api should ideally not run the queuing tasks itself, but instead delegate them to a central external messaging system like Azure Service Bus or RabbitMQ.
The data could be stored in Blob storage, which then could trigger a service bus event that processes the data. This would be a more robust design.

---

## Technologies
EventManager is build using these technologies 
* .NET 5.0
* FluentValidation
* AutoMapper
* Swashbuckle
* XUnit
* FluentAssertions & Moq

---

## Technical setup
EventManager runs as a web api and can be started either from Visual Studio or VS Code using F5 or from the command line using

```
dotnet run
```

There is no authentication, so all users can access the api.
All configuration is located in `appSettings.json` and for logging `nlog.config`.

There are some intentional delays in the file `QueuedWorkerService.cs`. This is to allow for querying the data before the processing takes place.
Try adding some jobs, and then wait to see the results. Only one job a minute will be processed. If you want to change the delay go to appSettings.Development.json.

---

## Code structure
The solution consists of this project:

1. Munnro.WebAPI

There is also a unit test project(s).
* Munro.UnitTests

---

## Usage
The WebAPI uses Swagger to make it easy to test the api. Simply visit http://localhost:port/swagger/index.html and learn the api. 

To test the API go to [http://localhost:port/swagger/index.html](http://localhost:port/swagger/index.html) and click POST EventManager/AddJob.
Then add some jobs with a name, username and an array of integers.
Next click Get /EventManager to fetch all jobs from storage and view their progress.
After some seconds retry and see their status being completed, and the arrays have been sorted.

Too see what's being logged during use, you can fire up this command in powershell. Just replace the file name with your log file.

```
Get-Content -Path .\scheduler-api-all-2021-02-01.log -Wait
```

If running under IISExpress in development, you can also see the queue service is shut down properly if you shut down the webapi.

An example of the console output can be seen here.
![EventManager console output](https://i.imgur.com/dNNJuOx.png)


The methods exposed are:

### Get All
> curl -X GET "http://localhost:4298/EventManager" -H  "accept: text/plain"

### GetById
> curl -X GET "http://localhost:4298/EventManager/byId/1" -H  "accept: text/plain"

### AddJob
> curl -X POST "http://localhost:4298/EventManager/AddJob" -H  "accept: text/plain" -H  "Content-Type: application/json" -d "[1,2,3]"

---

## Logging
The application logs using NLog logger to a flat file located in c:\Log. More log targets can be configured in nlog.config. For more info visit [https://github.com/NLog/NLog/wiki/Getting-started-with-ASP.NET-Core-5](https://github.com/NLog/NLog/wiki/Getting-started-with-ASP.NET-Core-5).

---

## Deployment
No deployment pipeline exists.

---

## vNext
* Add sqlite database
* Use Hangfire for background processing

---

## References
* [Implement background tasks in microservices with IHostedService and the BackgroundService class](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/multi-container-microservice-net-applications/background-tasks-with-ihostedservice)
* [Background tasks with hosted services in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-5.0&tabs=netcore-cli)
* [The Dangers of Implementing Recurring Background Tasks In ASP.NET](https://haacked.com/archive/2011/10/16/the-dangers-of-implementing-recurring-background-tasks-in-asp-net.aspx/)
* [Extending the shutdown timeout setting to ensure graceful IHostedService shutdown](https://andrewlock.net/extending-the-shutdown-timeout-setting-to-ensure-graceful-ihostedservice-shutdown/)
