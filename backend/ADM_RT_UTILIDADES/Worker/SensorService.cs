using ADM_RT_CORE_LIB.Messaging.Interfaces;
using ADM_RT_CORE_LIB.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ADM_RT_UTILIDADES.Worker
{
    public class SensorService : IHostedService, IDisposable
    {
        private readonly ILogger<SensorService> logger;
        private int number = 0;
        private Timer timer;
        private readonly int time = 20;

        private readonly IMessageBus bus;

        public SensorService(ILogger<SensorService> logger, IMessageBus bus)   
        {
            this.logger = logger;
            this.bus = bus;
        }
        public void Dispose()
        {
            timer?.Dispose();
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Process Started");
            timer = new Timer(async o =>
            {
                Interlocked.Increment(ref number);
                logger.LogInformation($"Printing from worker number: {number} at "
                    + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                logger.LogInformation("Envio de Luminosidade");
                
                Random randNum = new Random();
                int luminosidade = randNum.Next(100, 500);

                bus.Publish(new MessageData
                    {
                        AppName = "sensor de luminosidade",
                        Title = "nivel de luminosidade",
                        Message = luminosidade.ToString() 
                    },
                    Environment.GetEnvironmentVariable("APP_RABBIT_EXCHANGE"),
                    Environment.GetEnvironmentVariable("APP_RABBIT_ROUTINGKEY"));
                
                logger.LogInformation("Message published to queue");
            },
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(time));

            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Process Stopping");
            return Task.CompletedTask;
        }
    }
}
