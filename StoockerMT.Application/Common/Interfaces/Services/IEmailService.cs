using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Application.Common.Models;

namespace StoockerMT.Application.Common.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
    }
}
