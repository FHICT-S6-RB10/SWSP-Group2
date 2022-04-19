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
    Console.WriteLine(measurement.wearableId);
    //Console.WriteLine(measurement.jsonContent["Interval"].GetValue<int>());

    double valueRR = measurement.jsonContent["Interval"].GetValue<int>();
    Console.WriteLine(valueRR);

    if (valueRR > 200 && valueRR < 1200)
    {
        //send message on event buys
        Console.WriteLine(" normal RR");
    }
    else
    {
        Console.WriteLine(" RR to high");
    }

    return Results.Created($"/measurements/{measurement.patientId}", measurement);


});

app.Run();

public class Measurement
{
    public string patientId { get; set; }
    public string wearableId { get; set; }
    public JsonObject jsonContent { get; set; }

    public Measurement(string patientId, string wearableId, JsonObject jsonContent)
    {
        this.patientId = patientId;
        this.wearableId = wearableId;
        this.jsonContent = jsonContent;
    }

    public double NormalizeJSON(double value, double min, double max)
    {
        double normalized = (value - min) / (max - min);
        return normalized;
        //normalized = (x - min(x)) / (max(x) - min(x))
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

