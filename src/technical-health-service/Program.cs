using System;
using NATS.Client;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WatsonWebsocket;


#region API Setup
var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ServiceStateRepository>();
builder.Services.AddSingleton<ServiceLoggingRepository>();
builder.Services.AddCors(options =>
    options.AddPolicy(name: MyAllowSpecificOrigins,
        builder => builder.WithOrigins("http://localhost:6060", 
            "http://localhost:3000", "http://localhost:3000/*", 
            "http://localhost:80", "http://localhost:6060/*")));

var app = builder.Build();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
#endregion 

ConnectionFactory cf = new ConnectionFactory();
Options opts = ConnectionFactory.GetDefaultOptions();
opts.Url = "nats://host.docker.internal:4222";

IConnection c = cf.CreateConnection(opts);
HttpClient client = new HttpClient();
client.BaseAddress = new Uri("http://host.docker.internal:6060/");

#region Websocket connection
WatsonWsServer server = new WatsonWsServer("localhost", 6000, false);
server.ClientConnected += (sender, args) => ClientConnected(sender, args, server, app);
server.ClientDisconnected += ClientDisconnected;
server.MessageReceived += MessageReceived;
server.Start();

static void ClientConnected(object sender, ClientConnectedEventArgs args, WatsonWsServer server, WebApplication app)
{
    Console.WriteLine("Client connected: " + args.IpPort);
    SendServicesAndMessages(server, app, args.IpPort);
}

static void ClientDisconnected(object sender, ClientDisconnectedEventArgs args)
{
    Console.WriteLine("Client disconnected: " + args.IpPort);
}

static void MessageReceived(object sender, MessageReceivedEventArgs args)
{
    Console.WriteLine("Message received from " + args.IpPort + ": " + Encoding.UTF8.GetString(args.Data));
}

static void SendServicesAndMessages(WatsonWsServer server, WebApplication app, string address)
{
    var logRepo = app.Services.GetService<ServiceLoggingRepository>();
    var stateRepo = app.Services.GetService<ServiceStateRepository>();

    var logs = JsonSerializer.Serialize(logRepo.GetAll());
    var states = JsonSerializer.Serialize(stateRepo.GetAll());
    var data = $"{{services: {states}, messages: {logs}}}";

    _ = server.SendAsync(address, data);
}
#endregion

#region Message Handling
void HandleServiceState(ServiceStateRepository repo, ServiceState state)
{
    var existing = repo.GetByName(state.name);

    if (existing != null)
        repo.Update(state);

    else
        repo.Create(state);
}

// Set the state of service to UNAVAILABLE if a hearthbeat was not recieved in time
void UpdateStates()
{
    var repo = app.Services.GetService<ServiceStateRepository>();
    var services = repo.GetAll().ToList();

    foreach (ServiceState service in services)
    {
        var diff = DateTime.Now - service.lastUpdated;
        if (diff.TotalSeconds > 35)
        {
            var serviceUpdated = new ServiceState(service.name, ServiceStatus.UNAVAILABLE, service.lastUpdated);
            repo.Update(serviceUpdated);
        }
    }
}

EventHandler<MsgHandlerEventArgs> heartbeatHandler = (sender, args) => OnHeartbeatEvent(sender, args, server, app);

void OnHeartbeatEvent(object sender, MsgHandlerEventArgs args, WatsonWsServer server, WebApplication app)
{
    Console.WriteLine($"Received {args.Message}");
    string receivedMessage = Encoding.UTF8.GetString(args.Message.Data);
    var deserializedMessage = JsonDocument.Parse(receivedMessage);
    var decodedMessage = deserializedMessage.RootElement.GetProperty("message").ToString();
    var origin = deserializedMessage.RootElement.GetProperty("origin").ToString();

    if (decodedMessage.ToLower() == "heartbeat")
    {
        var state = new ServiceState(origin, ServiceStatus.AVAILABLE, DateTime.Now);
        var repo = app.Services.GetService<ServiceStateRepository>();
        if (repo != null)
            HandleServiceState(repo, state);
        else
            Console.WriteLine("Error: couldn't find the service state repository, Program.cs - line 68");
    }

    SendServicesAndMessages(server, app, server.ListClients().First());
}

EventHandler<MsgHandlerEventArgs> loggingEventHandler = (sender, args) => OnLoggingEvent(sender, args, server, app);

void OnLoggingEvent(object sender, MsgHandlerEventArgs args, WatsonWsServer server, WebApplication app)
{
    Console.WriteLine($"Received log: {args.Message}");
    string subject = args.Message.Subject.ToString();
    string receivedMessage = Encoding.UTF8.GetString(args.Message.Data);
    var deserializedMessage = JsonDocument.Parse(receivedMessage);
    var decodedMessage = deserializedMessage.RootElement.GetProperty("message").ToString();
    var origin = deserializedMessage.RootElement.GetProperty("origin").ToString();

    var logLevel = getLogLevelFor(subject);

    if (logLevel != LogLevel.UNKNOWN)
    {

        var message = new LogMessage(logLevel, origin, decodedMessage, DateTime.Now);
        var repo = app.Services.GetService<ServiceLoggingRepository>();
        if (repo != null)
        {
            repo.Create(message);
            //SendServicesAndMessages()
        }
        else
            Console.WriteLine("Error: couldn't find the service logging repository, Program.cs - line 110");
    }

    SendServicesAndMessages(server, app, server.ListClients().First());
}

LogLevel getLogLevelFor(string subject)
{
    switch (subject)
    {
        case "th_logs":
            return LogLevel.LOG;
        case "th_warnings":
            return LogLevel.WARNNING;
        case "th_errors":
            return LogLevel.ERROR;
        default:
            return LogLevel.UNKNOWN;
    }
}
#endregion

// Event Bus subscriptions
IAsyncSubscription s = c.SubscribeAsync("technical_health", heartbeatHandler);
IAsyncSubscription logSubscription = c.SubscribeAsync("th_logs", loggingEventHandler);
IAsyncSubscription warnSubscription = c.SubscribeAsync("th_warnings", loggingEventHandler);
IAsyncSubscription errSubscription = c.SubscribeAsync("th_errors", loggingEventHandler);

#region HTTP request endpoints
app.MapGet("/servicestates", ([FromServices] ServiceStateRepository repo) =>
{
    UpdateStates();
    return repo.GetAll();
})
.WithName("GetServiceStates");

app.MapGet("/servicestates/{name}", ([FromServices] ServiceStateRepository repo, string name) =>
{
    var state = repo.GetByName(name);
    return state is not null ? Results.Ok(state) : Results.NotFound();
})
.WithName("GetServiceStateByName");

app.MapGet("/logs", ([FromServices] ServiceLoggingRepository repo) =>
{
    return repo.GetAll();
})
.WithName("GetLogs");

// Used for testing
app.MapGet("/publishmessage", ([FromServices] ServiceStateRepository repo) =>
{
    var request = new Request("authentication", "heartbeat", "technical-health-service");
    var message = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));
    c.Publish("technical_health", message);
    return "Message published";
})
.WithName("PublishMessage");

// Used for testing
app.MapGet("/publishlog", ([FromServices] ServiceStateRepository repo) =>
{
    var request = new Request("authentication", "New user added!", "technical-health-service");
    var message = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));
    c.Publish("th_logs", message);
    return "Log message published";
})
.WithName("PublishLogMessage");

// Used for testing
app.MapGet("/publishwarning", ([FromServices] ServiceStateRepository repo) =>
{
    var request = new Request("authentication", "The added user has an empty Email field!", "technical-health-service");
    var message = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));
    c.Publish("th_warnings", message);
    return "Warning message published";
})
.WithName("PublishWarningMessage");

// Used for testing
app.MapGet("/publisherror", ([FromServices] ServiceStateRepository repo) =>
{
    var request = new Request("authentication", "Could not add the specified user!", "technical-health-service");
    var message = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));
    c.Publish("th_errors", message);
    return "Error message published";
})
.WithName("PublishErrorgMessage");

app.MapPost("/servicestates", ([FromServices] ServiceStateRepository repo, ServiceState state) =>
{
    repo.Create(state);
    return Results.Created($"/servicestates/{state.name}", state);
})
.WithName("CreateServiceStates");
#endregion

app.Run();

#region Data Management
internal record ServiceState(string name, ServiceStatus status, DateTime lastUpdated);

internal record LogMessage(LogLevel level, string origin, string message, DateTime invoked);

internal record Request(string origin, string message, string target);

enum ServiceStatus
{
    UNAVAILABLE = 0,
    AVAILABLE = 1,
    HAS_ERRORS = 2
}

enum LogLevel
{
    UNKNOWN = 0,
    LOG = 1,
    WARNNING = 2,
    ERROR = 3
}

class ServiceStateRepository
{
    private readonly List<ServiceState> _services = new List<ServiceState>();

    public void Create(ServiceState state)
    {
        if (state == null)
            return;

        _services.Add(state);
    }

    public List<ServiceState> GetAll()
    {
        return _services;
    }

    public ServiceState GetByName(string name)
    {
    #pragma warning disable CS8603 // Possible null reference return.
        return _services.Find(x => x.name == name);
    #pragma warning restore CS8603 // Possible null reference return.
    }

    public void Update(ServiceState state)
    {
        var existingState = GetByName(state.name);
        if(existingState == null)
            return;

        int index = _services.IndexOf(existingState);
        _services[index] = state;
    }
}

class ServiceLoggingRepository
{
    private readonly List<LogMessage> _logs = new List<LogMessage>();

    public void Create(LogMessage logMessage)
    {
        if (logMessage == null)
            return;

        _logs.Add(logMessage);
    }

    public List<LogMessage> GetAll()
    {
        return _logs;
    }

    public List<LogMessage> GetAllWithLevel(LogLevel level)
    {
        var filteredList = _logs.FindAll(x => x.level == level).ToList();
        return filteredList;
    }
}
#endregion