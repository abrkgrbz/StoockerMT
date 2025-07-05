using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.DTOs
{
    public class GroupedSelectListDto
    {
        public string Group { get; set; } = string.Empty;
        public List<SelectListDto> Items { get; set; } = new();
    }
}
