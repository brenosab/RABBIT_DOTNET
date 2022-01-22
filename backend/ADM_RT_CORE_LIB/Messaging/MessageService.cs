using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace ADM_RT_CORE_LIB.Messaging
{
    public static class MessageService
    {
        public static byte[] Serialize<T>(T payload)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var msg = JsonSerializer.Serialize(payload, options);

                return Encoding.UTF8.GetBytes(msg);
            }
            catch (JsonException ex)
            {
                throw ex;
            }
        }

        public static T Convert<T>(BasicDeliverEventArgs args)
        {
            try
            {
                var msg = Encoding.UTF8.GetString(args.Body.ToArray());

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<T>(msg, options);
            }
            catch (JsonException ex)
            {
                throw ex;
            }
        }
    }
}
