using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.DTOs
{
    public class SelectListDto
    {
        public string Value { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public bool Selected { get; set; }
        public bool Disabled { get; set; }
        public Dictionary<string, object> Data { get; set; } = new();
    }
}
