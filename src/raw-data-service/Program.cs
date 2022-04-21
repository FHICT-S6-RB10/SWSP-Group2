using Microsoft.AspNetCore.Mvc;
using NATS.Client;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using Raw_Data_Service.Models;
using Raw_Data_Service.Services;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using MongoDB.Bson.Serialization;

var builder = WebApplication.CreateBuilder(args);

//Connection to MongoDB
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("MeasurementsDatabase"));
builder.Services.AddSingleton<MeasurementsService>();

var app = builder.Build();

#region event bus
ConnectionFactory cf = new ConnectionFactory();
Options opts = ConnectionFactory.GetDefaultOptions();
opts.Url = "nats://host.docker.internal:4222";

IConnection c = cf.CreateConnection(opts);

EventHandler<MsgHandlerEventArgs> h = async (sender, args) =>
{
    string receivedMessage = Encoding.UTF8.GetString(args.Message.Data);
    var deserializedMessage = JsonDocument.Parse(receivedMessage);
    var decodedMessage = deserializedMessage.RootElement.GetProperty("message").ToString();
    var origin = deserializedMessage.RootElement.GetProperty("origin").ToString();

    JObject json = JObject.Parse(decodedMessage);

    var repo = app.Services.GetService<MeasurementsService>();
    Console.WriteLine("Repo should be :" + repo);
    Measurement m = new Measurement();
    m.Data = json.ToBsonDocument();
    // var bsonDocument = BsonSerializer.Deserialize<BsonDocument[]>(m.Data);
    Console.WriteLine("BSON :" + m.Data);
    Console.WriteLine("JSON :" + json);
    // Console.WriteLine("BSON DOCUMENT :" + bsonDocument);

    await repo.CreateAsync(m);

    Console.WriteLine("I think it worked");
    // Console.WriteLine("MESSAGE: " + decodedMessage);
    // foreach(var item in json){
    //     Console.WriteLine("ITEM: " + item);
    // }    
};

IAsyncSubscription s = c.SubscribeAsync("measurement:created", h);

Timer timer = new Timer(TimerCallback, null, 0, 20000); //executes TimerCallback every 20 seconds

//Method to raise event to technical health service
void TimerCallback(object? state){
    var request = new Request("raw_data_service", "heartbeat", "technical_health");
    var message = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));
    c.Publish("technical_health", message);
    Console.WriteLine("Heartbeat message published");
}

//To Do: simulate message with sensor data
// Used for testing purposes
app.MapGet("/publishmessage", async () =>
{
    var jsonData = new 
    {
        heartRate = new {average = 81.5, rrData = 710}
    };
    string json = JsonSerializer.Serialize(jsonData);
    Console.WriteLine("JsonData= "+ json);
    var request = new Request("raw_data_service", json, "measurement:created");
    var message = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));
    c.Publish("measurement:created", message);
    Console.WriteLine("New measurement published");
});

#endregion

#region api calls
//Getting all measurements 
app.MapGet("/measurements", async ([FromServices] MeasurementsService repo) =>
{
    List<Measurement> mesurs = await repo.GetAsync();
    foreach (var me in mesurs)
    {
        Console.WriteLine($"ID: {me.Id} DATA:{me.Data}");
    };
});
// Creates a new demo measurement for testing purposes
app.MapPost("/measurements", async ([FromServices] MeasurementsService repo) =>
{
    Measurement m = new Measurement();
    m.Data = new {heartRate = new {average = 69, rrData = 681}}.ToBsonDocument();
//     m.Data = new BsonDocument{
//     {"heartRate", new BsonDocument {
//        {"average", 82.5 },
//        {"rrData", 710}
//     }}
// };
    await repo.CreateAsync(m);
    return ($"{m} should be created");
});
#endregion

app.Run();

internal record Request(string origin, string message, string target); //used as a dto for event bus

// public class NewMeasurementEventController : Controller{
//     private readonly MeasurementsService _repo;

//     public NewMeasurementEventController(Measurement repo){
//         _repo = repo;
//     }

// }
