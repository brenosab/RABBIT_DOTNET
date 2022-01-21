using ADM_RT_CORE_LIB.Models;
using Microsoft.AspNetCore.Http;
using RabbitMQ.Client.Events;
using System.Collections.Generic;
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

        public static SerializableEmailData ParseEmailData(EmailData data, string htmlMessage)
        {
            SerializableEmailData emailData = new SerializableEmailData() { 
                AppName = data.AppName,
                Title = data.Title,
                Copies = data.Copies,
                Receivers = data.Receivers,
                Message = htmlMessage,
                Attachments = data.Attachments == null ? null : AttachmentsToBytes(data.Attachments)
            };
            return emailData;
        }

        private static List<FileData> AttachmentsToBytes(List<IFormFile> files)
        {
            List<FileData> result = new List<FileData>();
            foreach (var file in files)
            {
                result.Add(new FileData()
                {
                    Type = file.ContentType,
                    Name = file.FileName,
                    Content = GetFileContent(file)
                });
            }
            return result;
        }

        private static byte[] GetFileContent(IFormFile file)
        {
            try
            {
                byte[] bytes = new byte[file.Length];
                int numBytesToRead = (int)file.Length;
                int numBytesRead = 0;
                while (numBytesToRead > 0)
                {
                    int n = file.OpenReadStream().Read(bytes, numBytesRead, numBytesToRead);
                    if (n == 0)
                        break;
                    numBytesRead += n;
                    numBytesToRead -= n;
                }
                return bytes;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

    }
}
