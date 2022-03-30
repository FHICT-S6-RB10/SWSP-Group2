using ConsoleApp2.Models;
using Microsoft.EntityFrameworkCore;
using NATS.Client;
using NATS.Client.JetStream;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
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
            opts.Url = "nats://host.docker.internal:4222";
            opts.Timeout = 15500;
            IConnection c = cf.CreateConnection(opts);
            dataContext.Database.EnsureCreated();
            EventHandler<MsgHandlerEventArgs> h = (sender, args) =>
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
                        var responseData = c.Request(requestDTO.Target, Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(requestDTO)));
                        var receivedOrder = Encoding.UTF8.GetString(responseData.Data);
                        Console.WriteLine($"Received {receivedOrder}");
                        SuccessfulRequests dataModel = new SuccessfulRequests()
                        {
                            Channel = requestDTO.Target,
                            DateTime = DateTime.UtcNow,
                            RequestDTO = requestDTO


                        };
                        dataContext.successfulRequests.Add(dataModel);
                        dataContext.SaveChanges();
                        var reply = args.Message.Reply;
                        var replyMessage = Encoding.UTF8.GetBytes("Replied");
                        c.Publish(reply, replyMessage);
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
                    c.Publish(args.Message.Reply, replyMessage);
                }
               
            };
            IAsyncSubscription s = c.SubscribeAsync("eventbus", "load-balancing-queue", h);


            int firstRun = 1;
            while (true)
            {
                Task task1 = EventAAsync(dataContext, c);
                if (firstRun == 1)
                {
                    Console.WriteLine("Event Bus Started");
                    firstRun = 0;
                }
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }

            //while (true)
            //{

            //    Console.WriteLine("Channel: ");
            //    string channel = Console.ReadLine();
            //    Console.WriteLine("Message: ");
            //    string message = Console.ReadLine();
            //    Console.WriteLine("Times to be sent: ");
            //    string timesString = Console.ReadLine();
            //    int times;
            //    while (true)
            //    {

            //        try
            //        {
            //            times = Convert.ToInt32(timesString);
            //            break;

            //        }
            //        catch (Exception)
            //        {
            //            Console.WriteLine("Please specify a number.");
            //            timesString = Console.ReadLine();
            //        }
            //    }
            //    RequestDTO request = new RequestDTO("RequestConsole", message, channel);
            //    Console.WriteLine($"Sent {message} ");

            //    for (int i = 0; i < times; i++)
            //    {
            //        try
            //        {
            //            var responseData = c.RequestAsync(channel, Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(request)));
            //            var receivedOrder = Encoding.UTF8.GetString(responseData.Result.Data);
            //            Console.WriteLine($"Received {receivedOrder}");
            //            SuccessfulRequests dataModel = new SuccessfulRequests()
            //            {
            //                Channel = channel,
            //                DateTime = DateTime.UtcNow,
            //                RequestDTO = request


            //            };
            //            dataContext.successfulRequests.Add(dataModel);
            //            dataContext.SaveChangesAsync();
            //        }
            //        catch (Exception)
            //        {

            //            Console.WriteLine($"Order could not be delivered now");
            //            FailedRequest dataModel = new FailedRequest()
            //            {
            //                RequestDTO = request,
            //                Channel = channel,
            //                DateTime = DateTime.UtcNow,

            //            };
            //            dataContext.failedRequests.Add(dataModel);
            //            dataContext.SaveChangesAsync();
            //            Console.WriteLine("The request has been save to the database and will be sent later");
            //        }
            //    }





        }


            static async Task EventAAsync(DataContext dataContext, IConnection c)
            {
                Task task1 = Task.Run(async () =>  // <- marked async
                {

                    while (true)
                    {
                        try
                        {
                            var data = await dataContext.failedRequests.Include(w => w.RequestDTO).ToListAsync();

                            foreach (var item in data)
                            {
                                try
                                {
                                    Msg responseData = await c.RequestAsync(item.Channel, Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(item.RequestDTO)));
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
                                    await dataContext.SaveChangesAsync();

                                }
                                catch (Exception)
                                {
                                    Console.WriteLine("Server still offline");

                                }
                            }
                        }
                        catch (Exception)
                        {

                            throw;
                        }


                    }
                });
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }

