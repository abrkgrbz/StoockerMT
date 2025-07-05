using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Exceptions.Module
{
    public class ModuleNotFoundException : DomainException
    {
        public int ModuleId { get; }

        public ModuleNotFoundException(int moduleId)
            : base("MODULE_NOT_FOUND", $"Module with ID '{moduleId}' was not found.")
        {
            ModuleId = moduleId;
        }
    }
}
