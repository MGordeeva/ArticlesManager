using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Configuration;
using ArticlesManager.Shared;
using System.Drawing;
using System.Drawing.Imaging;

namespace ArticlesReceiver
{
    internal class Receiver
    {
        public IConnection GetRabbitMqConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = ConfigurationManager.AppSettings.Get("HostName"),
                Port = int.Parse(ConfigurationManager.AppSettings.Get("Port")),
                Password = ConfigurationManager.AppSettings.Get("Password"),
                UserName = ConfigurationManager.AppSettings.Get("UserName"),
            };

            return factory.CreateConnection();
        }

        public void ReceiveChunkedMessages(IModel model, string filePath)
        {
            model.QueueDeclare(queue: "ChunkedMessageBufferedQueue",
                               durable: true,
                               exclusive: false,
                               autoDelete: false,
                               arguments: null);

            var consumer = new EventingBasicConsumer(model);

            var bytes = new List<byte>();

            consumer.Received += (model, ea) =>
            {
                Console.WriteLine("Received a chunk!");
                var body = ea.Body.ToArray();
                var headers = ea.BasicProperties.Headers;
                bool isLastChunk = Convert.ToBoolean(headers["finished"]);

                using (MemoryStream fileStream = new MemoryStream())
                {
                    bytes.AddRange(body);
                    fileStream.Write(body, 0, body.Length);

                    fileStream.Flush();
                }
                Console.WriteLine("Chunk saved. Finished? {0}", isLastChunk);
                if (bytes.Any() && isLastChunk)
                {
                    try
                    {
                        var compressedImageBytes = ImageHelper.ReduceImageSize(bytes.ToArray());
                        var compressedImage = Image.FromStream(new MemoryStream(compressedImageBytes.ToArray()));
                        compressedImage.Save(filePath, ImageFormat.Jpeg);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        throw;
                    }
                }
            };

            model.BasicConsume(queue: "ChunkedMessageBufferedQueue",
                               autoAck: true,
                               consumer: consumer);
        }
    }
}
