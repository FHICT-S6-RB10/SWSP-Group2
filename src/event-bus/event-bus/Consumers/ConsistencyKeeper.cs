using event_bus.Context;
using event_bus.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NATS.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace event_bus.Consumers
{
    public class ConsistencyKeeper : BackgroundService
    {

        static DataContext dataContext;
        static IConfiguration _configuration;
        static IConnection _connection;

        public ConsistencyKeeper(IConnection connection, IServiceScopeFactory factory, IConfiguration configuration)
        {
            dataContext = factory.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
            _connection = connection;
            _configuration = configuration;


        }
        static void EventAAsync()
        {

                while (true)
                {
                
                    try
                    {
                        var data = dataContext.failedRequests.Include(w => w.RequestDTO).ToListAsync().Result;
                    if (data.Count == 0)
                    {
                        break;
                    }

                        foreach (var item in data)
                        {
                            try
                            {
                                Msg responseData =  _connection.RequestAsync(item.Channel, Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(item.RequestDTO))).Result;
                                string receivedOrder = Encoding.UTF8.GetString(responseData.Data);
                                Console.WriteLine($"Received {receivedOrder}");
                                SuccessfulRequests dataModel = new SuccessfulRequests()
                                {
                                    Channel = item.Channel,
                                    DateTime = DateTime.UtcNow,
                                    RequestDTO = item.RequestDTO



                                };
                                dataContext.successfulRequests.Add(dataModel);
                                dataContext.failedRequests.Remove(item);
                                dataContext.SaveChangesAsync().Wait();

                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Server still offline");

                            }
                        }
                    }
                    catch (Exception)
                    {

                    break;
                    }


                }
         

        }



        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Consistency Keeper Service has been started");
            while (true)
            {
                Console.WriteLine("ran Consistency");
                EventAAsync();
                Thread.Sleep(TimeSpan.FromSeconds(60));
            }

        }
    }
}
