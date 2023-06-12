using RabbitMQ.Client;
using System.Configuration;

namespace ArticlesSender
{
    internal class Sender
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
        
        public bool ValidatePicture(string extension)
        {
            return extension == ".jpg";
        }

        public void RunChunkedMessageExample(IModel model, string filePath)
        {
            int chunkSize = 4096;
            Console.WriteLine("Starting file read operation...");
            FileStream fileStream = File.OpenRead(filePath);
            int remainingFileSize = Convert.ToInt32(fileStream.Length);
            bool finished = false;
            byte[] buffer;
            while (remainingFileSize > 0)
            {
                if (remainingFileSize <= 0) break;
                int read = 0;
                if (remainingFileSize > chunkSize)
                {
                    buffer = new byte[chunkSize];
                    read = fileStream.Read(buffer, 0, chunkSize);
                }
                else
                {
                    buffer = new byte[remainingFileSize];
                    read = fileStream.Read(buffer, 0, remainingFileSize);
                    finished = true;
                }

                IBasicProperties basicProperties = model.CreateBasicProperties();
                basicProperties.Headers = new Dictionary<string, object>();
                basicProperties.Headers.Add("finished", finished);

                model.BasicPublish("", "ChunkedMessageBufferedQueue", basicProperties, buffer);
                remainingFileSize -= read;
            }
            Console.WriteLine("Chunks complete.");
        }
    }
}
