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
using System.Globalization;
using System.Net.Mime;
using System.IO.Compression;

namespace DataLibrary
{
    public class MailService : IMailService
    {
        readonly IDataAccess _dataAccess;

        public MailService(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
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

						if (mailRequest.Attachments != null)
						{
								foreach (var file in mailRequest.Attachments)
								{
										if (file.Attachment.Length > 0)
										{
                        builder.Attachments.Add(file.Name, file.Attachment);
										}
								}
						}

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

        public async Task SentCSVs(int DtID)
        {
            MailRequest mr = new MailRequest();

            var dtRec = _dataAccess.GetDTrec(DtID);
            mr.Body = "Data yedekleriniz ektedir.";
            mr.Subject = dtRec.Ad;
            mr.ToEmail = dtRec.Mail;

            mr.DtID = DtID;
            mr.KtID = 0;
            mr.KhID = 0;

            mr.Attachments = new List<MailAttachment>();
            
            ////MailAttachment ma = new MailAttachment();
            ////ma.Name = "KisiTanim.csv";
            ////ma.Attachment = await _dataAccess.KtCSV(DtID);
            ////mr.Attachments.Add(ma);

            ////ma = new MailAttachment();
            ////ma.Name = "KisiHareket.csv";
            ////ma.Attachment = await _dataAccess.KhCSV(DtID);
            ////mr.Attachments.Add(ma);

            Dictionary<string, byte[]> Files = new Dictionary<string, byte[]>();
            
            var ms = await _dataAccess.KtCSV(DtID);
            Files.Add("KT.csv", ms.ToArray());
            ms = await _dataAccess.KhCSV(DtID);
            Files.Add("KH.csv", ms.ToArray());

            var ma = new MailAttachment();
            ma.Name = "Data.zip";

            //MemoryStream stream = new MemoryStream(ZipFiles(Files));
            //ma.Attachment = stream;
            ma.Attachment = ZipFiles(Files);
            mr.Attachments.Add(ma);

            await SendEmailAsync(mr);
        }

        public static byte[] ZipFiles(Dictionary<string, byte[]> files)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (ZipArchive archive = new ZipArchive(ms, ZipArchiveMode.Update))
                {
                    foreach (var file in files)
                    {
                        ZipArchiveEntry orderEntry = archive.CreateEntry(file.Key); //create a file with this name
                        using (BinaryWriter writer = new BinaryWriter(orderEntry.Open()))
                        {
                            writer.Write(file.Value); //write the binary data
                        }
                    }
                }
                //ZipArchive must be disposed before the MemoryStream has data
                return ms.ToArray();
            }
        }
    }
}
