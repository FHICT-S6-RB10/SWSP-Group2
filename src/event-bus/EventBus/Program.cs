using ConsoleApp2.Models;
using Microsoft.EntityFrameworkCore;
using NATS.Client;
using NATS.Client.JetStream;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory cf = new ConnectionFactory();
            Options opts = ConnectionFactory.GetDefaultOptions();
            DataContext dataContext = new DataContext();
            opts.Url = "nats://nats:4222";

            IConnection c = cf.CreateConnection(opts);

            Task task1 = Task.Run(async () =>  // <- marked async
            {
                while (true)
                {
                    var data = await dataContext.failedRequests.Include(w=>w.RequestDTO).ToListAsync();

                    foreach (var item in data)
                    {
                        try
                        {
                            var responseData = c.Request(item.Channel, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(item.RequestDTO)));
                            var receivedOrder = Encoding.UTF8.GetString(responseData.Data);
                            Console.WriteLine($"Received {receivedOrder}");
                            SuccessfulRequests dataModel = new SuccessfulRequests()
                            {
                                Channel = item.Channel,
                                DateTime = DateTime.UtcNow,
                                RequestDTO = item.RequestDTO


                            };
                            dataContext.successfulRequests.Add(dataModel);
                            dataContext.failedRequests.Remove(item);
                            await dataContext.SaveChangesAsync();

                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Server still offline");
                        }
                    }
                    await Task.Delay(60 * 1000);
                }
            });

            while (true)
            {

                Console.WriteLine("Channel: ");
                string channel = Console.ReadLine();
                Console.WriteLine("Message: ");
                string message = Console.ReadLine();
                Console.WriteLine("Times to be sent: ");
                string timesString = Console.ReadLine();
                int times;
                while (true)
                {
                    
                try
                {
                times = Convert.ToInt32(timesString);
                        break;

                }
                catch (Exception)
                {
                    Console.WriteLine("Please specify a number.");
                    timesString = Console.ReadLine();
                    }
                }
                RequestDTO request = new RequestDTO("RequestConsole", message);
                Console.WriteLine($"Sent {message} ");

                for (int i = 0; i < times; i++)
                {
                    var responseData = c.RequestAsync(channel, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request)));
                    try
                    {
                        var receivedOrder = Encoding.UTF8.GetString(responseData.Result.Data);
                        Console.WriteLine($"Received {receivedOrder}");
                        SuccessfulRequests dataModel = new SuccessfulRequests()
                        {
                            Channel = channel,
                            DateTime = DateTime.UtcNow,
                            RequestDTO = request

                         
                        };
                        dataContext.successfulRequests.Add(dataModel);
                        dataContext.SaveChangesAsync();
                    }
                    catch (Exception)
                    {

                        Console.WriteLine($"Order could not be delivered now");
                        FailedRequest dataModel = new FailedRequest()
                        {
                            RequestDTO = request,
                            Channel = channel,
                            DateTime = DateTime.UtcNow,

                        };
                        dataContext.failedRequests.Add(dataModel);
                        dataContext.SaveChangesAsync();
                        Console.WriteLine("The request has been save to the database and will be sent later");
                    }
                }
                




            }
        }
    }
}
