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
// using System.Reflection;

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
    Measurement m = new Measurement();
    foreach (JProperty property in json.Properties())
    {
        if((property.Name).ToString().ToLower() == "patientid"){
            m.PatientId = property.Value.ToString();
            continue;
        }
        if((property.Name).ToString().ToLower() == "wearableid"){
            m.WearableId = property.Value.ToString();
            continue;
        }

        var jsonData = property.Value;

        BsonDocument doc = BsonDocument.Parse(jsonData.ToString());

        m.Data = doc;
    }

    await repo.CreateAsync(m);

};

IAsyncSubscription s = c.SubscribeAsync("measurement:created", h);

Timer timer = new Timer(TimerCallback, null, 0, 20000); //executes TimerCallback every 20 seconds

//Method to raise event to technical health service
void TimerCallback(object? state)
{
    var request = new Request("raw_data_service", "heartbeat", "technical_health");
    var message = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));
    c.Publish("technical_health", message);
    Console.WriteLine("Heartbeat message published");
}

// Used for testing purposes
app.MapGet("/publishheart", async () =>
{
    var jsonData = new
    {
        patientId = "13g1f1qd3asd",
        wearableId = "04141da341",
        jsonContent = new
        {
            Timestamp = "21/02/2022 18:49:47",
            Heart = 83,
            Interval = 726,
            RMSSD = 32.95,
            Event = "None"
        }
    };
    string json = JsonSerializer.Serialize(jsonData);
    // Console.WriteLine("JsonData= " + json);
    var request = new Request("raw_data_service", json, "measurement:created");
    var message = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));
    c.Publish("measurement:created", message);
    Console.WriteLine("New measurement published");
});

app.MapGet("/publishskin", async () =>
{
    int[] records = {185,184,1825,1825,182,1825,1825,1825,182,1815,1805,1795,1795,1795,1795,1795};
    var jsonData = new
    {
        patientId = "13g1f1qd3asd",
        wearableId = "04141da341",
        jsonContent = new
        {
            Frequency= "16hz",
            Records = records
        }
    };
    string json = JsonSerializer.Serialize(jsonData);
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
    m.Data = new { heartRate = new { average = 69, rrData = 681 } }.ToBsonDocument();
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

