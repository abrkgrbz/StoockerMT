using StoockerMT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Persistence.Services.Models
{
    public class TenantBackup
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string BackupPath { get; set; }
        public DateTime BackupDate { get; set; }
        public long BackupSize { get; set; }
        public BackupType BackupType { get; set; }
        public BackupStatus Status { get; set; }
    }
}
