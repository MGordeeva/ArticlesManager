using RabbitMQ.Client;
using System.Configuration;
using System.Text;

namespace ArticlesSender
{
    internal class Sender
    {
        public static void Send()
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

            var message = "Some test message";
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: string.Empty,
                                 routingKey: "hello",
                                 basicProperties: null,
                                 body: body);

            Console.WriteLine($" Sent message: {message}");

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
