using ArticlesSender;
using RabbitMQ.Client;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Sender started");

        Sender sender = new Sender();
        IConnection connection = sender.GetRabbitMqConnection();
        IModel model = connection.CreateModel();
        model.QueueDeclare("ChunkedMessageBufferedQueue", true, false, false, null);

        Console.WriteLine("Please, enter path to your image.");
        var filePath = Console.ReadLine();
        sender.RunChunkedMessageExample(model, filePath);

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}