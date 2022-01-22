using System;

namespace HOME_RABBITMQ_CONSUMER
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                AtuadorService atuadorService = new AtuadorService();
                while (true)
                {
                    Console.WriteLine("Home Assistant is runnig");
                    Console.WriteLine("Pressione A para ler a fila");
                    Console.WriteLine("Pressione B para ligar a lâmpada");
                    Console.WriteLine("Pressione C para desligar a lâmpada");
                    Console.WriteLine("Pressione D para desligar o ar-condicionado");

                    var input = Console.ReadKey();
                    switch (input.Key)
                    {
                        case ConsoleKey.A:
                            MessageHandler handler = new MessageHandler();
                            handler.SubscribeToQueue();
                            Console.WriteLine("Subscribed to queue");
                            break;
                        case ConsoleKey.B:
                            atuadorService.LigarLampada().Wait();
                            break;
                        case ConsoleKey.C:
                            atuadorService.DesligarLampada().Wait();
                            break;
                        case ConsoleKey.D:
                            atuadorService.LigarArCondicionado().Wait();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
