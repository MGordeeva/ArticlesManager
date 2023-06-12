using ArticlesReceiver;
using RabbitMQ.Client;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Receiver started");
        Receiver receiver = new Receiver();

        IConnection connection = receiver.GetRabbitMqConnection();
        IModel model = connection.CreateModel();
        Console.WriteLine("Please, enter file path for new image.");
        var filePath = Console.ReadLine();
        receiver.ReceiveChunkedMessages(model, filePath);

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}