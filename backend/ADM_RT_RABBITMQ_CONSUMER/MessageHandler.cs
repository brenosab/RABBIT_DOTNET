using ADM_RT_CORE_LIB.Email;
using ADM_RT_CORE_LIB.Messaging;
using ADM_RT_CORE_LIB.Messaging.Interfaces;
using ADM_RT_CORE_LIB.Models;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ADM_RT_RABBITMQ_CONSUMER
{
    public class MessageHandler
    {
        private readonly IMessageBus bus;
        private IModel channel;
        private IConnection connection;

        public MessageHandler()
        {
            bus = new MessageBus(
                Environment.GetEnvironmentVariable("APPLICATION"),
                Environment.GetEnvironmentVariable("APP_RABBIT_HOST"),
                int.Parse(Environment.GetEnvironmentVariable("APP_RABBIT_PORT")),
                Environment.GetEnvironmentVariable("APP_RABBIT_USER"),
                Environment.GetEnvironmentVariable("APP_RABBIT_PASSWORD"));
        }

        public void SubscribeToQueue()
        {
            Receive("fila_luminosidade");
            //bus.Subscribe(Environment.GetEnvironmentVariable("APP_RABBIT_QUEUENAME"),
            //        (SerializableEmailData data) =>
            //        {
            //            EmailService emailService = new EmailService();
            //            emailService.SetProperties(data);
            //            emailService.SendEmail();
            //            return Task.CompletedTask;
            //        });
        }
        public void Receive(string queueName)
        {

            var policy = Policy.Handle<AlreadyClosedException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetry(1, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            policy.Execute(() =>
            {
                var factory = new ConnectionFactory();
                connection = factory.CreateConnection();
                channel = connection.CreateModel();
            });

            channel.QueueDeclarePassive(queueName);
            
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);
            };
            channel.BasicConsume(queue: queueName,
                                    autoAck: true,
                                    consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        public void CloseConnection()
        {
            bus.Close();
        }

    }
}
