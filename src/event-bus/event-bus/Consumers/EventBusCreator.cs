using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NATS.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using event_bus.Models;
using event_bus.Context;
using event_bus.Encryption;

namespace event_bus.Consumers
{
    public class EventBusCreator : BackgroundService
    {
        static EncryptionAlgorithms encryptionAlgorithms;
        static DataContext dataContext;
        static IConfiguration _configuration;
        static IConnection _connection;

        public EventBusCreator(IConnection connection, IServiceScopeFactory factory, IConfiguration configuration)
        {
            dataContext = factory.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
            _connection = connection;
            _configuration = configuration;
            encryptionAlgorithms = new EncryptionAlgorithms(configuration["EncryptionKey"]);

        }
        EventHandler<MsgHandlerEventArgs> hEventBus = (sender, args) =>
        {
            Console.WriteLine($"worker received {args.Message}");
            try
            {
                string receivedMessage = System.Text.Encoding.UTF8.GetString(args.Message.Data);
                var deserializedMessage = (JObject)JsonConvert.DeserializeObject(receivedMessage);
                var decodedMessage = deserializedMessage.SelectToken("Message").ToString();
                RequestDTO requestDTO = System.Text.Json.JsonSerializer.Deserialize<RequestDTO>(decodedMessage);
                Console.WriteLine($"Got message: {decodedMessage}");
                try
                {
                    var responseData = _connection.Request(requestDTO.Target, Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(requestDTO)));
                    requestDTO.Message = encryptionAlgorithms.EncryptString(requestDTO.Message);
                    var receivedOrder = Encoding.UTF8.GetString(responseData.Data);
                    Response receivedOrderToDTO  ;
                    try
                    {
                        receivedOrderToDTO = System.Text.Json.JsonSerializer.Deserialize<Response>(receivedOrder);
                    }
                    catch (Exception)
                    {

                        receivedOrderToDTO = new Response() { Status = "Error", Message = "Failed to cast to JSON in Event Bus Receiver" };
                    }
                    SuccessfulRequests dataModel = new SuccessfulRequests()
                    {
                        Channel = requestDTO.Target,
                        DateTime = DateTime.UtcNow,
                        RequestDTO = requestDTO


                    };
                    dataContext.successfulRequests.Add(dataModel);
                    dataContext.SaveChanges();
                    var reply = args.Message.Reply;
                    var replyMessage = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(receivedOrderToDTO));
                    _connection.Publish(reply, replyMessage);
                    Console.WriteLine($"Order was delivered successfully");

                }
                catch (Exception)
                {
                    Console.WriteLine($"Order could not be delivered now");
                    FailedRequest dataModel = new FailedRequest()
                    {
                        RequestDTO = requestDTO,
                        Channel = requestDTO.Target,
                        DateTime = DateTime.UtcNow,

                    };
                    dataContext.failedRequests.Add(dataModel);
                    dataContext.SaveChanges();
                    Console.WriteLine("The request has been save to the database and will be sent later");
                }
            }
            catch (Exception)
            {
                var replyMessage = Encoding.UTF8.GetBytes("Error : Incorrect DTO format");
                _connection.Publish(args.Message.Reply, replyMessage);
            }

        };


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            IAsyncSubscription sEventBus = _connection.SubscribeAsync("eventbus", "load-balancing-queue-account", hEventBus);
            Console.WriteLine("Event Bus has been started");
            return Task.CompletedTask;
        }
    }
}
