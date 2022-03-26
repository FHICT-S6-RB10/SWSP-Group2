using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using NATS.Client;

namespace THS_Worker
{
    class Worker
    {
        internal record ServiceState(string name, ServiceStatus status);

        public static void Main()
        {
            ConnectionFactory cf = new ConnectionFactory();
            Options opts = ConnectionFactory.GetDefaultOptions();
            opts.Url = "nats://localhost:4444";

            IConnection c = cf.CreateConnection(opts);

            EventHandler<MsgHandlerEventArgs> h = (sender, args) =>
            {
                Console.WriteLine($"Received {args.Message}");
                string receivedMessage = Encoding.UTF8.GetString(args.Message.Data);
                var deserializedMessage = JsonDocument.Parse(receivedMessage);
                var decodedMessage = deserializedMessage.RootElement.GetProperty("Message").ToString();
                Console.WriteLine($"Message: {decodedMessage}");
                if (decodedMessage.ToLower() == "ping")
                {
                    var reply = args.Message.Reply;
                    var replyMessage = Encoding.UTF8.GetBytes("th_status_recieved");
                    c.Publish(reply, replyMessage);
                    Console.WriteLine($"Published message {Encoding.UTF8.GetString(replyMessage)} to {reply} ");
                    //client.PostAsync(uri, new StringContent(jsonInString, Encoding.UTF8, "application/json"));

                }
            };

            IAsyncSubscription s = c.SubscribeAsync("technical_health", h);

            while (true)
            {
                Console.WriteLine("worker listening...");
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        }
    }
}