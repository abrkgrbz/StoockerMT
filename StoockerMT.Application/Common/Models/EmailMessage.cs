using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Models
{
    public class EmailMessage
    {
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool IsHtml { get; set; } = true;
        public List<string> Cc { get; set; } = new();
        public List<string> Bcc { get; set; } = new();
        public Dictionary<string, Stream> Attachments { get; set; } = new();
    }
}
