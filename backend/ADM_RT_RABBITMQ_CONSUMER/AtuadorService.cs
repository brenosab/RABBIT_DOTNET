using Grpc.Net.Client;
using GrpcGreeterClient;
using System;   
using System.Threading.Tasks;

namespace HOME_RABBITMQ_CONSUMER
{
    class AtuadorService
    {
        private enum CodAtuador
        {
            lampada = 27,
            ar_condicionado = 12,
            incendio = 71
        }
        public async Task LigarArCondicionado()
        {
            var creditRequest = new MsgRequest
            {
                Cod = (int)CodAtuador.ar_condicionado,
                Tipo = Enum.GetName(CodAtuador.ar_condicionado)
            };
            // The port number(5001) must match the port of the gRPC server.
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var reply = await client.LigarAsync(creditRequest);
            Console.WriteLine($"\nResposta válida: {reply.Valid} do tipo: {reply.Tipo} último estado: {reply.Ligado}!");
        }
        public async Task LigarLampada()
        {   
            var creditRequest = new MsgRequest 
            { 
                Cod = (int)CodAtuador.lampada,
                Tipo = Enum.GetName(CodAtuador.lampada)
            };
            // The port number(5001) must match the port of the gRPC server.
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var reply = await client.LigarAsync(creditRequest);
            Console.WriteLine($"\nResposta válida: {reply.Valid} do tipo: {reply.Tipo} último estado: {reply.Ligado}!");
        }
        public async Task DesligarLampada()
        {
            var creditRequest = new MsgRequest
            {
                Cod = (int)CodAtuador.lampada,
                Tipo = Enum.GetName(CodAtuador.lampada)
            };
            // The port number(5001) must match the port of the gRPC server.
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var reply = await client.DesligarAsync(creditRequest);
            Console.WriteLine($"\nResposta válida: {reply.Valid} do tipo: {reply.Tipo} último estado: {reply.Ligado}!");
        }
    }
}