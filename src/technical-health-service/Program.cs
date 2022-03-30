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
            "http://localhost:3000", "http://localhost:3000/*", "http://localhost:80")));

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
opts.Url = "nats://localhost:4222";

IConnection c = cf.CreateConnection(opts);
HttpClient client = new HttpClient();
client.BaseAddress = new Uri("http://localhost:6060/");

// For testing purposes only
async void AddStateAsync(ServiceState state)
{
    HttpResponseMessage response = await client.PostAsJsonAsync(
        "servicestates", state);
    response.EnsureSuccessStatusCode();
}

EventHandler<MsgHandlerEventArgs> h = (sender, args) =>
{
    Console.WriteLine($"Received {args.Message}");
    string receivedMessage = Encoding.UTF8.GetString(args.Message.Data);
    var deserializedMessage = JsonDocument.Parse(receivedMessage);
    var decodedMessage = deserializedMessage.RootElement.GetProperty("message").ToString();
    var origin = deserializedMessage.RootElement.GetProperty("origin").ToString();

    // Add service to the list 
    if (decodedMessage.ToLower() == "hearthbeat")
    {
        var state = new ServiceState(origin, ServiceStatus.AVAILABLE);
        AddStateAsync(state);
    }
};

IAsyncSubscription s = c.SubscribeAsync("technical_health", h);

#region HTTP request endpoints
app.MapGet("/servicestates", ([FromServices] ServiceStateRepository repo) =>
{
    return repo.GetAll();
})
.WithName("GetServiceStates");

// Used for testing
app.MapGet("/servicestatesmock", ([FromServices] ServiceStateRepository repo) =>
{
    var state1 = new ServiceState("Authentication", ServiceStatus.AVAILABLE);
    var state2 = new ServiceState("SensorDataStorage", ServiceStatus.HAS_ERRORS);
    repo.Create(state1);
    repo.Create(state2);
    return repo.GetAll();
})
.WithName("GetServiceStatesMock");

// Used for testing
app.MapGet("/publishmessage", ([FromServices] ServiceStateRepository repo) =>
{
    var request = new Request("authentication", "hearthbeat", "technical-health-service");
    var message = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));
    c.Publish("technical_health", message);
    return "Message published";
})
.WithName("PublishMessage");

app.MapGet("/servicestates/{name}", ([FromServices] ServiceStateRepository repo, string name) =>
{
    var state = repo.GetByName(name);
    return state is not null ? Results.Ok(state) : Results.NotFound();
})
.WithName("GetServiceStateByName");

app.MapPost("/servicestates", ([FromServices] ServiceStateRepository repo, ServiceState state) =>
{
    repo.Create(state);
    return Results.Created($"/servicestates/{state.name}", state);
})
.WithName("CreateServiceStates");
#endregion

app.Run();

#region Data Management
internal record ServiceState(string name, ServiceStatus status);

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
#endregion