using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using NATS.Client;
using NATS.Client.Rx;
using NATS.Client.Rx.Ops; //Can be replaced with using System.Reactive.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Worker
{
    class Program
    {
        static void Main()
        {
            bool firststime = true;
            Console.WriteLine("Worker:");
            ConnectionFactory cf = new ConnectionFactory();
            Options opts = ConnectionFactory.GetDefaultOptions();
            opts.Url = "nats://localhost:4444";

            IConnection c = cf.CreateConnection(opts);

            EventHandler<MsgHandlerEventArgs> h = (sender, args) =>
            {
                Console.WriteLine($"worker received {args.Message}");
                string receivedMessage = System.Text.Encoding.UTF8.GetString(args.Message.Data);
                var deserializedMessage = (JObject)JsonConvert.DeserializeObject(receivedMessage);
                var decodedMessage = deserializedMessage.SelectToken("Message").ToString();
                Console.WriteLine($"Got message: {decodedMessage}");
                if (decodedMessage.ToLower() == "ping")
                {
                    var reply = args.Message.Reply;
                    var replyMessage = Encoding.UTF8.GetBytes("pong");
                    c.Publish(reply, replyMessage);
                    Console.WriteLine($"Published message {Encoding.UTF8.GetString(replyMessage)} to {reply} ");

                }
                if (decodedMessage.ToLower() == "elden")
                {
                    var reply = args.Message.Reply;
                    var replyMessage = Encoding.UTF8.GetBytes("Ring");
                    c.Publish(reply, replyMessage);
                    Console.WriteLine($"Published message {Encoding.UTF8.GetString(replyMessage)} to {reply} ");

                }
                if (decodedMessage.ToLower() == "fizz")
                {
                    var reply = args.Message.Reply;
                    var replyMessage = Encoding.UTF8.GetBytes("buzz");
                    c.Publish(reply, replyMessage);
                    Console.WriteLine($"Published message {Encoding.UTF8.GetString(replyMessage)} to {reply} ");

                }






            };

            IAsyncSubscription s = c.SubscribeAsync("worker","load-balancing-queue", h);

            while (true)
            {
                if (firststime)
                {
                Console.WriteLine("worker listening...");
                    firststime = false;
                }
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        }
    }
}
