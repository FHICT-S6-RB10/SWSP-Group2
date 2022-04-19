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
// using System.Threading;
// using InfluxDB.Client;
// using InfluxDB.Client.Api.Domain;
// using InfluxDB.Client.Core;
// using InfluxDB.Client.Writes;
// using InfluxDB;
// using Examples;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddSingleton<MeasurementRepository>();
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("MeasurementsDatabase"));
builder.Services.AddSingleton<MeasurementsService>();

var app = builder.Build();

// BsonDocument mesur = new BsonDocument{
//     {"data", new BsonDocument {
//         { "heartRate", "pederas"}
//     } }
// };
Measurement m = new Measurement();
m.Data = new BsonDocument{
    {"heartRate", new BsonDocument {
       {"average", 81.5 },
       {"rrData", 710} 
    }}
};

// await service.CreateAsync(m);

#region event bus
// ConnectionFactory cf = new ConnectionFactory();
// Options opts = ConnectionFactory.GetDefaultOptions();
// opts.Url = "nats://host.docker.internal:4222";

// IConnection c = cf.CreateConnection(opts);

// EventHandler<MsgHandlerEventArgs> h = (sender, args) =>
// {
//     //Console.WriteLine($"Received {args.Message}");
//     string receivedMessage = Encoding.UTF8.GetString(args.Message.Data);
//     var deserializedMessage = JsonDocument.Parse(receivedMessage);
//     var decodedMessage = deserializedMessage.RootElement.GetProperty("message").ToString();
//     var origin = deserializedMessage.RootElement.GetProperty("origin").ToString();

    
//     if (decodedMessage.ToLower() == "new measurement")
//     {
//         Console.WriteLine("Adding measurement to db..." + origin);
//     }
// };

// IAsyncSubscription s = c.SubscribeAsync("raw_data", h);

// Timer timer = new Timer(TimerCallback, null, 0, 20000); //executes TimerCallback every 20 seconds

// //Method to raise event to technical health service
// void TimerCallback(object? state){
//     var request = new Request("raw_data_service", "heartbeat", "technical_health");
//     var message = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));
//     c.Publish("technical_health", message);
//     Console.WriteLine("Heartbeat message published");
// }

// Used for testing purposes
// app.MapGet("/publishmessage", ([FromServices] MeasurementRepository repo) =>
// {
//     var request = new Request("raw_data_service", "measurement:new", "raw_data");
//     var message = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));
//     c.Publish("raw_data", message);
//     Console.WriteLine("New measurement published");
// });

#endregion

#region api calls
//Getting all measurements 
// app.MapGet("/measurements", ([FromServices] MeasurementRepository repo) => {
//     return repo.GetAll();
// });

// //Get measurement by its id
// app.MapGet("/measurements/{id}", ([FromServices] MeasurementRepository repo) => {
//     return repo.GetAll();
// });

// //Get all measurements of an user
// app.MapGet("measurements/by-user/{id}", ([FromServices] MeasurementRepository repo, string id) =>{
//     var measurement = repo.GetByUserId(id);
//     return measurement is not null ? Results.Ok(measurement) : Results.NotFound();
// });

//Create a new measurement
app.MapPost("/measurements", ([FromServices] MeasurementsService repo, Measurement measurement) => {
    repo.CreateAsync(m);
    return($"I dont event know if {measurement} is created");
    // return Results.Created($"/measurements/{measurement.id}", measurement);
});
#endregion


// ExamplesConnection examples = new ExamplesConnection(); 
// examples.DoSomething();

app.Run();

// record Measurement(string id, string userId, string data, string wearableId);


internal record Request(string origin, string message, string target);

// class MeasurementRepository {
//     private readonly List<Measurement> _measurements = new List<Measurement>();

//     public void Create(Measurement measurement){
//         if (measurement is null){
//             return;
//         }

//         _measurements.Add(measurement);
//     }

//     public Measurement GetById(string id){
//         return _measurements.Find(x => x.id == id);
//     }
   
//     public List<Measurement> GetByUserId(string userId){        
//         return _measurements.FindAll(x => x.userId == userId);       
//     }
//     public List<Measurement> GetAll(){
//         return _measurements;
//     }
//     public List<Measurement> GetUserMeasurementsByTimestamp(string id, DateTime start, DateTime finish){
//         //TO DO IMPLEMENT LOGIC
//         return _measurements;
//     }

// }


// #region influxdb 
// namespace Examples
// {
//     public class ExamplesConnection
//     {
//         public static async Task Main(string[] args)
//         {
//             const string token = "zcHwI1xw4FaVB8oSjcsmXrx5iQD2TywwwlskonG72QECkKM8NejT8biFwkPsG_Q0ESdkCrZTU7h_xdK6kcuGGQ==";
//             const string bucket = "bucket";
//             const string org = "organization";

//             using var client = InfluxDBClientFactory.Create("http://localhost:8086", token);

//             Console.WriteLine(client);
//             Console.WriteLine("testing");

//             // const string data = "users,user=pedal1 used_temp=37.43234543";
//             // using (var writeApi = client.GetWriteApi())
//             // {
//             //     writeApi.WriteRecord(bucket, org, WritePrecision.Ns, data);
//             // }

//         }
//         public async void DoSomething()
//         {
//             Console.WriteLine("testing");
//             const string token = "zcHwI1xw4FaVB8oSjcsmXrx5iQD2TywwwlskonG72QECkKM8NejT8biFwkPsG_Q0ESdkCrZTU7h_xdK6kcuGGQ==";
//             const string bucket = "bucket";
//             const string org = "organization";

//             using var client = InfluxDBClientFactory.Create("http://localhost:8086", token);

//             var point = PointData
//                 .Measurement("mem")
//                 .Tag("host", "host1")
//                 .Field("used_percent", 23.43234543)
//                 .Timestamp(DateTime.UtcNow, WritePrecision.Ns);

//             Console.WriteLine(point);

//             using (var writeApi = client.GetWriteApi())
//             {
//                 writeApi.WritePoint(bucket, org, point);
//             }



//             var query = "from(bucket: \"bucket\") |> range(start: -1h)";
//             var tables = await client.GetQueryApi().QueryAsync(query, org);

//             foreach (var record in tables.SelectMany(table => table.Records))
//             {
//                 Console.WriteLine($"{record}");
//             }
//         }
//     }
// }
// #endregion
