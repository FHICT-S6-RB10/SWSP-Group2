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

ConnectionFactory cf = new ConnectionFactory();
Options opts = ConnectionFactory.GetDefaultOptions();
opts.Url = "nats://host.docker.internal:4222";

IConnection c = cf.CreateConnection(opts);

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


Timer timer = new Timer(TimerCallback, null, 0, 20000);

void TimerCallback(object? state)
{
    var request = new Request("sensor_data_service", "technical_health", "heartbeat");
    var message = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));
    c.Publish("technical_health", message);
    Console.WriteLine("Message techincal-health-service");
}

//app.MapGet("/measurements", ([FromServices] MeasurementRepo repo) => {
//    return repo.GetAll();
//});

app.MapPost("/measurements/SkinConductance", async (SkinConductance skinConductance) =>
{
    Console.WriteLine(skinConductance.patientId);
    Console.WriteLine(skinConductance.wearableId);
    foreach (var record in skinConductance.records)
    {
        Console.WriteLine(record);
    }
    Console.WriteLine("-------------------");

    List<double> normalizedArr = skinConductance.NormalizeFrequency(skinConductance.records, skinConductance.frequency);

    double[] normalizedRecords = normalizedArr.ToArray();

    skinConductance.records = normalizedRecords;

    foreach (var record in skinConductance.records)
    {
        Console.WriteLine(record);
    }

    //send message on event bus
    var request = new SkinConductanceData("sensor_data_service", "measurement:created", skinConductance);
    var message = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));
    Console.WriteLine(Encoding.UTF8.GetString(message));
    c.Publish("normalized-data", message);

    return Results.Created($"/measurements/SkinConductance/{skinConductance.patientId}", skinConductance);
});

app.MapPost("/measurements/Heart", async (Heart heart) =>
{
    Console.WriteLine(heart.patientId);
    Console.WriteLine(heart.wearableId);
    Console.WriteLine(heart.interval);

    double valueRR = heart.interval;
    Console.WriteLine(valueRR);

    if (valueRR > 200 && valueRR < 1200)
    {
        //message on event bus
        Console.WriteLine("Normal Heart");
        var request = new HeartData("sensor_data_service", "measurement:created", heart);
        var message = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));
        Console.WriteLine(Encoding.UTF8.GetString(message));
        c.Publish("normalized-data", message);
    }
    else
    {
        Console.WriteLine("Faulty Heart");
    }
    return Results.Created($"/measurements/Heart/{heart.patientId}", heart);
});

app.MapPost("/measurements/RR", async (RR rr) =>
{
    Console.WriteLine(rr.patientId);
    Console.WriteLine(rr.wearableId);
    foreach (var record in rr.records)
    {
        Console.WriteLine(record);
    }

    List<double> normalizedList = rr.NormalizeRR(rr.records);

    double[] normalizedRecords = normalizedList.ToArray();

    rr.records = normalizedRecords;

    //message on event bus
    Console.WriteLine("Normal RR");
    var request = new RRData("sensor_data_service", "measurement:created", rr);
    var message = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));
    Console.WriteLine(Encoding.UTF8.GetString(message));
    c.Publish("normalized-data", message);

    return Results.Created($"/measurements/RR/{rr.patientId}", rr);
});

app.Run();

public class SkinConductance
{
    public string patientId { get; set; }
    public string wearableId { get; set; }
    public int frequency { get; set; }
    public double[] records { get; set; }

    public SkinConductance(string patientId, string wearableId, int frequency, double[] records)
    {
        this.patientId = patientId;
        this.wearableId = wearableId;
        this.frequency = frequency;
        this.records = records;
    }

    public List<double> NormalizeFrequency(double[] records, int frequency)
    {
        List<double> normalizedArr = new List<double>();
        for (int i = 0; i < records.Length; i += frequency)
        {
            if (i + frequency >= records.Length)
                frequency = records.Length - i;
            double[] subArray = new double[frequency];
            Array.Copy(records, i, subArray, 0, frequency);
            normalizedArr.Add(subArray.Average());
        }
        return normalizedArr;
        //return normalizedArr.ToArray(normalizedArr.Count);
    }
}

public class Heart
{
    public string patientId { get; set; }
    public string wearableId { get; set; }
    public string timestamp { get; set; }
    public double heart { get; set; }
    public double interval { get; set; }
    public string eventI { get; set; }

    public Heart(string patientId, string wearableId, string timestamp, double heart, double interval, string eventI)
    {
        this.patientId = patientId;
        this.wearableId = wearableId;
        this.timestamp = timestamp;
        this.heart = heart;
        this.interval = interval;
        this.eventI = eventI;
    }

    public double NormalizeRR(double value, double min, double max)
    {
        double normalizedRR = (value - min) / (max - min);
        return normalizedRR;
        //normalized = (x - min(x)) / (max(x) - min(x))
    }
}

public class RR
{
    public string patientId { get; set; }
    public string wearableId { get; set; }
    public string timestamp { get; set; }
    public double[] records { get; set; }

    public RR(string patientId, string wearableId, string timestamp, double[] records)
    {
        this.patientId = patientId;
        this.wearableId = wearableId;
        this.timestamp= timestamp;
        this.records = records;
    }

    public List<double> NormalizeRR(double[] records)
    {
        List<double> normalizedList = new List<double>();

        double prevRecord = 0;
        double difference = 0;
        for (int i = 0; i < records.Length; i++)
        {
            if (i <= 0)
            {
                prevRecord = records[i];
            }
            else
            {
                prevRecord = records[i - 1];
            }
            difference = (((records[i] - prevRecord) / prevRecord) * 100) * -1;
            if (records[i] > 200 && records[i] < 1200 && 20 >= difference)
            {
                normalizedList.Add(records[i]);
                Console.WriteLine(records[i]);
            }
            else
            {
                Console.WriteLine("Faulty RR");
            }
        }
        return normalizedList;
    }
}

internal record Request(string origin, string target, string message);
internal record HeartData(string origin, string target, Heart message);
internal record RRData(string origin, string target, RR message);
internal record SkinConductanceData(string origin, string target, SkinConductance message);

