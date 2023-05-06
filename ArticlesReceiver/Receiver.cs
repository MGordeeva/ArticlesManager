using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Configuration;

namespace ArticlesReceiver
{
    internal class Receiver
    {
        public static void Receive()
        {
            var factory = new ConnectionFactory
            {
                HostName = ConfigurationManager.AppSettings.Get("HostName"),
                Port = int.Parse(ConfigurationManager.AppSettings.Get("Port")),
                Password = ConfigurationManager.AppSettings.Get("Password"),
                UserName = ConfigurationManager.AppSettings.Get("UserName"),
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: ConfigurationManager.AppSettings.Get("QueueName"),
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($" [x] Received {message}");
            };
            channel.BasicConsume(queue: "hello",
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
