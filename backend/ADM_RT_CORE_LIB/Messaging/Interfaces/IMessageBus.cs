using RabbitMQ.Client.Events;
using System;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace ADM_RT_CORE_LIB.Messaging.Interfaces
{
    public interface IMessageBus
    {
        bool IsConnected { get; }
        void Publish<T>(T payload, string exchange = "", string routingKey = "");
        void Subscribe<T>(string queueName, Func<T, Task> onMessage) where T : class;
        void Subscribe<T>(string queueName, Func<T, IChannel, EventArgs, Task> onMessage) where T : class;
        void Close();
        void TryConnect();
    }
}
