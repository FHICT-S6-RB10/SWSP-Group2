using NATS.Client;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;

var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddSingleton<MeasurementRepo>();

//var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

//builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();
//app.UseHttpsRedirection();
//app.UseStaticFiles();
//app.UseRouting();

//ConnectionFactory cf = new ConnectionFactory();
//Options opts = ConnectionFactory.GetDefaultOptions();
//opts.Url = "nats://localhost:4222";

//IConnection c = cf.CreateConnection(opts);

//EventHandler<MsgHandlerEventArgs> h = (sender, args) =>
//{
//    Console.WriteLine($"Received {args.Message}");
//    string receivedMessage = Encoding.UTF8.GetString(args.Message.Data);
//    var deserializedMessage = JsonDocument.Parse(receivedMessage);
//    var decodedMessage = deserializedMessage.RootElement.GetProperty("message").ToString();
//    var origin = deserializedMessage.RootElement.GetProperty("origin").ToString();

//    if (decodedMessage.ToLower() == "ping")
//    {
//        var reply = args.Message.Reply;
//        var replyMessage = Encoding.UTF8.GetBytes("th_status_recieved");
//        c.Publish(reply, replyMessage);
//        Console.WriteLine($"Published message {Encoding.UTF8.GetString(replyMessage)} to {reply} ");
//        //client.PostAsync(uri, new StringContent(jsonInString, Encoding.UTF8, "application/json"));

//    }
//};

//app.MapGet("/measurements", ([FromServices] MeasurementRepo repo) => {
//    return repo.GetAll();
//});

app.MapPost("/measurements/", async (Measurement measurement) => {

    Console.WriteLine(measurement.jsonContent);
    return Results.Created($"/measurements/{measurement.patientId}", measurement);
});

app.Run();

public class Measurement
{
    public string patientId { get; set; }
    public string patientName { get; set; }
    public JsonValue jsonContent { get; set; }

    public Measurement(string patientId, string patientName, JsonValue jsonContent)
    {
        this.patientId = patientId;
        this.patientName = patientName;
        this.jsonContent = jsonContent;
    }
}
//record Measurement(string PatientId, string DeviceId, JsonContent content);

//class MeasurementRepo
//{
//    private readonly List<Measurement> _measurements = new List<Measurement>();

//    public void Create(Measurement measurement)
//    {
//        if (measurement is null)
//        {
//            return;
//        }

//        _measurements.Add(measurement);
//    }

//    public List<Measurement> GetByPatientId(string PatientId)
//    {
//        return _measurements.FindAll(x => x.PatientId == PatientId);
//    }
//    public List<Measurement> GetAll()
//    {
//        return _measurements;
//    }
//}

