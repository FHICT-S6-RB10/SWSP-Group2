using Microsoft.AspNetCore.Mvc;
using NATS.Client;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using System.Dynamic;
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

//Connection to NATS
ConnectionFactory cf = new ConnectionFactory();
Options opts = ConnectionFactory.GetDefaultOptions();
opts.Url = "nats://host.docker.internal:4222";

IConnection c = cf.CreateConnection(opts);

#region organization credentials

//Name of current organization
string localOrganizationID = "2e830c86-a94c-45f7-87e8-f137b72406ff";

//Gets the current organization from message
EventHandler<MsgHandlerEventArgs> organizationHandler = (sender, args) => OnOrganizationIdEvent(sender, args);
void OnOrganizationIdEvent(object sender, MsgHandlerEventArgs args)
{
    string receivedMessage = Encoding.UTF8.GetString(args.Message.Data);
    var deserializedMessage = JsonDocument.Parse(receivedMessage);
    var decodedMessage = deserializedMessage.RootElement.GetProperty("message").ToString();
    var organizationId = deserializedMessage.RootElement.GetProperty("organizationId").ToString();

    JObject json = JObject.Parse(decodedMessage);

    foreach (JProperty property in json.Properties())
    {
        var name = property.Name.ToString();
        if (name.ToLower() == "id")
        {
            localOrganizationID = property.Value.ToString();
            break;
        }
    }
}

IAsyncSubscription organizationSubscription = c.SubscribeAsync("organization-created", organizationHandler);

#endregion

#region Measurements

//Listener for new measurements
EventHandler<MsgHandlerEventArgs> h = async (sender, args) =>
{
    string receivedMessage = Encoding.UTF8.GetString(args.Message.Data);
    var deserializedMessage = JsonDocument.Parse(receivedMessage);
    var decodedMessage = deserializedMessage.RootElement.GetProperty("message").ToString();
    var origin = deserializedMessage.RootElement.GetProperty("origin").ToString();

    JObject json = JObject.Parse(decodedMessage);

    var repo = app.Services.GetService<MeasurementsService>();
    Measurement m = new Measurement();

    dynamic jsonData = new ExpandoObject();

    foreach (JProperty property in json.Properties())
    {
        var name = property.Name.ToString();
        if (name.ToLower() == "patientid")
        {
            m.PatientId = property.Value.ToString();
            continue;
        }
        if (name.ToLower() == "wearableid")
        {
            m.WearableId = property.Value.ToString();
            continue;
        }
        if (name.ToLower() == "timestamp")
        {
            m.Timestamp = property.Value.ToString();
            continue;
        }

        AddProperty(jsonData, name, property.Value); //assigns all wearable data to a dynamic object 

    }
    //Converts and saves the wearable data to the "Data" property and adds measurement to the database
    BsonDocument doc = BsonDocument.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(jsonData).ToString());

    m.Data = doc;

    await repo.CreateAsync(m);
};

IAsyncSubscription s = c.SubscribeAsync("measurement:created", h);

#endregion

#region Heartbeat status

Timer timer = new Timer(TimerCallback, null, 0, 20000); //executes TimerCallback every 20 seconds

//Method to raise event to technical health service
void TimerCallback(object? state)
{
    var request = new Request("raw_data_service", "heartbeat", "technical_health", localOrganizationID);
    var message = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));
    c.Publish("technical_health", message);
    Console.WriteLine("Heartbeat message published");
}

#endregion

#endregion

//Method to add properties to dynamic objects
static void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
{
    var expandoDict = expando as IDictionary<string, object>;
    if (expandoDict.ContainsKey(propertyName))
        expandoDict[propertyName] = propertyValue;
    else
        expandoDict.Add(propertyName, propertyValue);
}

#region API requests that publish measurement data for testing purposes
app.MapGet("/publishheart", async () =>
{
    var jsonData = new
    {
        patientId = "13g1f1qd3asd",
        wearableId = "04141da341",
        timestamp = "21/02/2022 18:49:47",
        heart = 83,
        interval = 726,
        eventI = "None",
    };
    string json = JsonSerializer.Serialize(jsonData);
    var request = new Request("raw_data_service", json, "measurement:created", localOrganizationID);
    var message = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));
    c.Publish("measurement:created", message);
    Console.WriteLine("New measurement published");
});

app.MapGet("/publishskin", async () =>
{
    int[] recordsArray = { 185, 184, 1825, 1825, 182, 1825, 1825, 1825, 182, 1815, 1805, 1795, 1795, 1795, 1795, 1795 };
    var jsonData = new
    {
        patientId = "13g1f1qd3asd",
        wearableId = "04141da341",
        frequency = 4,
        records = recordsArray,
    };
    string json = JsonSerializer.Serialize(jsonData);
    var request = new Request("raw_data_service", json, "measurement:created", localOrganizationID);
    var message = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));
    c.Publish("measurement:created", message);
    Console.WriteLine("New measurement published");
});
#endregion


app.Run();

internal record Request(string origin, string message, string target, string organizationId); //used as a dto for event bus

