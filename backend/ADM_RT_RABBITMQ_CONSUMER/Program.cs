using System;

namespace HOME_RABBITMQ_CONSUMER
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                MessageHandler handler = new MessageHandler();
                handler.SubscribeToQueue();
                Console.WriteLine("Subscribed to queue");
                Console.ReadLine();
                handler.CloseConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
