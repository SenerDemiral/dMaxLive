using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DataLibrary.Models
{
    public class MailRequest
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<MailAttachment> Attachments { get; set; }
        public int DtID { get; set; }
        public int KtID { get; set; }
        public int KhID { get; set; }
    }

    public class MailAttachment
		{
        public string Name { get; set; }
        public MemoryStream Attachment { get; set; }
    }
}
