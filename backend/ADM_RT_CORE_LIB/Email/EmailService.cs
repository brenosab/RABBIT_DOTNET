using ADM_RT_CORE_LIB.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace ADM_RT_CORE_LIB.Email
{
    public class EmailService
    {
        private MailMessage Message { get; set; }

        public EmailService()
        {
            Message = new MailMessage()
            {
                Priority = MailPriority.Normal,
                IsBodyHtml = true,
                HeadersEncoding = Encoding.UTF8,
                From = new MailAddress(Environment.GetEnvironmentVariable("ADM_EMAIL"), "", Encoding.UTF8)
            };
        }

        public void SendEmail()
        {
            using (var client = new SmtpClient(
                Environment.GetEnvironmentVariable("ADM_MAIL_HOST"),
                int.Parse(Environment.GetEnvironmentVariable("ADM_EMAIL_PORT"))))
            {
                client.Credentials = new NetworkCredential(
                    Environment.GetEnvironmentVariable("ADM_EMAIL"),
                    Environment.GetEnvironmentVariable("ADM_EMAIL_PASSWORD"));
                client.Send(Message);
            }
        }

        public void SetProperties(SerializableEmailData emailData)
        {
            SetSingleProperties(emailData);
            SeTAttachments(emailData.Attachments);
            SeTCopies(emailData.Copies);
            SeTReceivers(emailData.Receivers);
        }

        private void SetSingleProperties(SerializableEmailData emailData)
        {
            Message.Body = emailData.Message;
            Message.Subject = emailData.Title;
        }

        private void SeTAttachments(List<FileData> attachments)
        {
            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    Stream contentStream = new MemoryStream(attachment.Content);
                    Message.Attachments.Add(new Attachment(contentStream, attachment.Name, attachment.Type));
                }
            }
        }

        private void SeTCopies(List<string> copies)
        {
            if (copies != null)
            {
                foreach (string copy in copies)
                {
                    Message.CC.Add(copy);
                }
            }
        }

        private void SeTReceivers(List<string> emails)
        {
            foreach (string email in emails)
            {
                Message.To.Add(email);
            }
        }

    }
}
