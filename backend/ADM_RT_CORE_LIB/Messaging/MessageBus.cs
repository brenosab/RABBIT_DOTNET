using ADM_RT_CORE_LIB.Messaging.Interfaces;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace ADM_RT_CORE_LIB.Messaging
{
    public class MessageBus : IMessageBus
    {
        private IConnection connection;
        private IModel channel;

        private readonly string appId;
        private readonly string host;
        private readonly string pass;
        private readonly int port;
        private readonly string user;
        public MessageBus(string appId, string host, int port, string user, string pass)
        {
            this.appId = appId;
            this.host = host;
            this.port = port;
            this.user = user;
            this.pass = pass;
        }
        public bool IsConnected => connection?.IsOpen ?? false;
        public void Close()
        {
            connection.Close();
        }
        public void Publish<T>(T payload, string exchange = "", string routingKey = "")
        {
            TryConnect();

            var props = channel.CreateBasicProperties();
            props.AppId = appId;
            props.UserId = user;
            props.MessageId = Guid.NewGuid().ToString("N");
            props.Persistent = true;

            var body = MessageService.Serialize<T>(payload);

            channel.BasicPublish(exchange, routingKey, props, body);
        }

        public void Subscribe<T>(string queueName, Func<T, Task> onMessage) where T : class
        {
            PreparingConsumer(queueName, async(_, ea) =>
            {
                try
                {
                    await onMessage(MessageService.Convert<T>(ea));

                    channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception)
                {
                    channel.BasicNack(ea.DeliveryTag, false, false);
                }
            });
        }

        public void Receive(string queueName)
        {
            TryConnect();
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
        public void Subscribe<T>(string queueName, Func<T, IChannel, EventArgs, Task> onMessage) where T : class
        {
            PreparingConsumer(queueName, async (_, ea) => await onMessage(MessageService.Convert<T>(ea), (IChannel)channel, (EventArgs)ea));
        }
        private void PreparingConsumer(string queueName, AsyncEventHandler<BasicDeliverEventArgs> handler)
        {
            TryConnect();

            channel.QueueDeclarePassive(queueName);
            channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(channel);
            //consumer.Received += handler;
            string msg = "";
            consumer.Received += (sender, ea) =>
            {
                var body = ea.Body.ToArray(); //ea.Body is of Type ReadOnlyMemory<byte>
                var message = Encoding.UTF8.GetString(body);
                msg = message;
                Console.WriteLine(" [x] Received {0}", message);
                //Diz ao RabbitMQ que a mensagem foi lida com sucesso pelo consumidor
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: true);
            };
            //Registra os consumidor no RabbitMQ
            channel.BasicConsume(queueName, true, consumer);
        }
        public void TryConnect()
        {
            if (IsConnected) return;

            var policy = Policy.Handle<AlreadyClosedException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetry(1, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            policy.Execute(() =>
            {
                var factory = new ConnectionFactory();
                connection = factory.CreateConnection();
                channel = connection.CreateModel();
            });
        }
    }
}
