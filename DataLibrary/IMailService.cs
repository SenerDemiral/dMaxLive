using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary
{
    public interface IMailService
    {
        Task SendEmailAsync(Models.MailRequest mailRequest);
        string MailMergeKH(Models.KHmodel rec, string sablon);
    }


}
