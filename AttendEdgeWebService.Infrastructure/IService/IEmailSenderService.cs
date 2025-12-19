using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendEdgeWebService.Infrastructure.IService
{
    public interface IEmailSenderService
    {
        void SendEmail(string htmlBody, string toEmail, string subject);
    }
}
