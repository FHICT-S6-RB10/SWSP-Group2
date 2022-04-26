using NATS.Client;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

ConnectionFactory cf = new ConnectionFactory();
Options opts = ConnectionFactory.GetDefaultOptions();
opts.Url = "nats://host.docker.internal:4222";

IConnection c = cf.CreateConnection(opts);

Timer timer = new Timer(TimerCallback, null, 0, 20000);

void TimerCallback(object? state)
{
    var request = new Request("sensor_data_service", "technical_health", "heartbeat");
    var message = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));
    c.Publish("technical_health", message);
    Console.WriteLine("Message techincal-health-service");
}

app.MapPost("/measurements/", async (Measurement measurement) => {

    Console.WriteLine(measurement.jsonContent);
    Console.WriteLine(measurement.wearableId);
    //Console.WriteLine(measurement.jsonContent["Interval"].GetValue<int>());

    double valueRR = measurement.jsonContent["Interval"].GetValue<int>();
    Console.WriteLine(valueRR);

    if (valueRR > 200 && valueRR < 1200)
    {
        //send message on event bus
        Console.WriteLine(" normal RR");
        var request = new RequestData("sensor_data_service", "raw-data-service", measurement);
        var message = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));
        Console.WriteLine(Encoding.UTF8.GetString(message));
        c.Publish("normalized-data", message);
    }
    else
    {
        Console.WriteLine(" RR too high");
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

internal record Request(string origin, string target, string message);
internal record RequestData(string origin, string target, Measurement message);

