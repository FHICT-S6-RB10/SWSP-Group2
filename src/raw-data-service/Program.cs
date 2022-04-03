using Microsoft.AspNetCore.Mvc;
using NATS.Client;
using System.Text;
using System.Text.Json;
using System.Threading;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<MeasurementRepository>();

var app = builder.Build();

ConnectionFactory cf = new ConnectionFactory();
Options opts = ConnectionFactory.GetDefaultOptions();
opts.Url = "nats://localhost:4222";

IConnection c = cf.CreateConnection(opts);

EventHandler<MsgHandlerEventArgs> h = (sender, args) =>
{
    //Console.WriteLine($"Received {args.Message}");
    string receivedMessage = Encoding.UTF8.GetString(args.Message.Data);
    var deserializedMessage = JsonDocument.Parse(receivedMessage);
    var decodedMessage = deserializedMessage.RootElement.GetProperty("message").ToString();
    var origin = deserializedMessage.RootElement.GetProperty("origin").ToString();

    
    if (decodedMessage.ToLower() == "new measurement")
    {
        Console.WriteLine("Adding measurement to db..." + origin);
    }

    
};

IAsyncSubscription s = c.SubscribeAsync("raw_data", h);

Timer timer = new Timer(TimerCallback, null, 0, 20000); //executes TimerCallback every 20 seconds

//Method to raise event to technical health service
void TimerCallback(object? state){
    var message = Encoding.UTF8.GetBytes("hearthbeat");
    c.Publish("hearthbeat", message);
    Console.WriteLine("Heartbeat message published");
}


app.MapGet("/publishmessage", ([FromServices] MeasurementRepository repo) =>
{
    var request = new Request("new measurement", "new measurement");
    var message = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));
    c.Publish("raw_data", message);
    return "Message published";
});

app.MapGet("/measurements", ([FromServices] MeasurementRepository repo) => {
    return repo.GetAll();
});

app.MapGet("measurements/{id}", ([FromServices] MeasurementRepository repo, int id) =>{
    var measurement = repo.GetByUserId(id);
    return measurement is not null ? Results.Ok(measurement) : Results.NotFound();
});

app.MapPost("/measurements", ([FromServices] MeasurementRepository repo, Measurement measurement) => {
    repo.Create(measurement);
    return Results.Created($"/measurements/{measurement.id}", measurement);
});

app.Run();

record Measurement(int id, int userId, int stressLevel);

internal record Request(string origin, string message);

class MeasurementRepository {
    private readonly List<Measurement> _measurements = new List<Measurement>();

    public void Create(Measurement measurement){
        if (measurement is null){
            return;
        }

        _measurements.Add(measurement);
    }
   
    public List<Measurement> GetByUserId(int userId){        
        return _measurements.FindAll(x => x.userId == userId);       
    }
    public List<Measurement> GetAll(){
        return _measurements;
    }
}