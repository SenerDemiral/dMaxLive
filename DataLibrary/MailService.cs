using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit;
using DataLibrary.Models;
using MailKit.Security;
using System.Threading.Tasks;

namespace DataLibrary
{
    public class MailService : IMailService
    {
        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse("dmax.live.mail@gmail.com");
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = $"Sayın {mailRequest.Subject}";
            var builder = new BodyBuilder();
            /*
            if (mailRequest.Attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in mailRequest.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }*/
            //builder.HtmlBody = mailRequest.Body;
            builder.TextBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);
            smtp.Authenticate("dmax.live.mail@gmail.com", "dmlm2020");
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
        public string MailMergeKH(KHmodel rec, string sablon)
        {
            string body = sablon
                .Replace("[Ben]", $"{rec.Ad}", StringComparison.OrdinalIgnoreCase)  // !!!!!!!!!!!!!!!!
                .Replace("[Ad]", $"{rec.Ad}", StringComparison.OrdinalIgnoreCase)
                .Replace("[DğmTrh]", $"{rec._DgmTrh}", StringComparison.OrdinalIgnoreCase)
                .Replace("[Yaş]", $"{rec.Yas}", StringComparison.OrdinalIgnoreCase)
                .Replace("[Sex]", $"{rec.Sex}", StringComparison.OrdinalIgnoreCase)
                .Replace("[KiloKg]", $"{rec._KiloKg}", StringComparison.OrdinalIgnoreCase)
                .Replace("[Boy]", $"{rec._BoyCm}", StringComparison.OrdinalIgnoreCase)
                .Replace("[BMI]", $"{rec._BMHkCal}", StringComparison.OrdinalIgnoreCase)
                .Replace("[BMIRAd]", $"{rec.BMIRad}", StringComparison.OrdinalIgnoreCase)
                .Replace("[Info]", $"{rec.Info}", StringComparison.OrdinalIgnoreCase);

            return body;
        }
    }



}
