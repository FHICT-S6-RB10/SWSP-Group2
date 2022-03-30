using NATS.Client;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

ConnectionFactory cf = new ConnectionFactory();
Options opts = ConnectionFactory.GetDefaultOptions();
opts.Url = "nats://localhost:4222";

IConnection c = cf.CreateConnection(opts);

EventHandler<MsgHandlerEventArgs> h = (sender, args) =>
{
    Console.WriteLine($"Received {args.Message}");
    string receivedMessage = Encoding.UTF8.GetString(args.Message.Data);
    var deserializedMessage = JsonDocument.Parse(receivedMessage);
    var decodedMessage = deserializedMessage.RootElement.GetProperty("message").ToString();
    var origin = deserializedMessage.RootElement.GetProperty("origin").ToString();


    if (decodedMessage.ToLower() == "new measurement")
    {
        Console.WriteLine("Storing measurement" + origin);
    }
};

IAsyncSubscription s = c.SubscribeAsync("raw_data", h);


app.Run();

record Measurement(int id, int userId, JsonContent measurement);

internal record Request(string origin, string message);




//--------------------------------------------

//var builder = WebApplication.CreateBuilder(args);

//var app = builder.Build();

//app.MapGet("/servicestates", () => new
//{
//    name = "SensorDataService",
//    status = 1
//});

//app.Run();

//record State(String name, int status);

//---------------------------------------

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddRazorPages();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthorization();

//app.MapRazorPages();

//app.Run();
