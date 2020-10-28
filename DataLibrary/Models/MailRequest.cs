using System;
using System.Collections.Generic;
using System.Text;

namespace DataLibrary.Models
{
    public class MailRequest
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        //public List<IFormFile> Attachments { get; set; }

        public int DtID { get; set; }
        public int KtID { get; set; }
        public int KhID { get; set; }
    }
}
