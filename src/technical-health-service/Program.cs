using System;
using NATS.Client;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;


#region API Setup
var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ServiceStateRepository>();
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

EventHandler<MsgHandlerEventArgs> heartbeatHandler = (sender, args) =>
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
};

//EventHandler<MsgHandlerEventArgs> logHandler = (sender, args) =>
//{
//    string receivedMessage = Encoding.UTF8.GetString(args.Message.Data);
//    var deserializedMessage = JsonDocument.Parse(receivedMessage);
//    var decodedMessage = deserializedMessage.RootElement.GetProperty("message").ToString();
//    var origin = deserializedMessage.RootElement.GetProperty("origin").ToString();

//    Console.WriteLine("Error: couldn't find the service state repository, Program.cs - line 68");
//};

IAsyncSubscription s = c.SubscribeAsync("technical_health", heartbeatHandler);
//IAsyncSubscription logSubscription = c.SubscribeAsync("th_logs", logHandler);
//IAsyncSubscription warnSubscription = c.SubscribeAsync("th_warnings", warningHandler);
//IAsyncSubscription errSubscription = c.SubscribeAsync("th_errors", errorHandler);


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

// Used for testing
app.MapGet("/publishmessage", ([FromServices] ServiceStateRepository repo) =>
{
    var request = new Request("authentication", "heartbeat", "technical-health-service");
    var message = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));
    c.Publish("technical_health", message);
    return "Message published";
})
.WithName("PublishMessage");

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

internal record Log(string name, ServiceStatus status);

internal record Request(string origin, string message, string target);

enum ServiceStatus
{
    UNAVAILABLE = 0,
    AVAILABLE = 1,
    HAS_ERRORS = 2
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

class LogsRepository
{
    private readonly List<ServiceState> _logs = new List<ServiceState>();

}
#endregion