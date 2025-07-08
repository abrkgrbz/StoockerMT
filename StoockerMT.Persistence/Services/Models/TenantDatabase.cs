using StoockerMT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Persistence.Services.Models
{
    public class TenantDatabase
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string DatabaseName { get; set; }
        public string Server { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConnectionString { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public DateTime? LastMigrationDate { get; set; }
        public DateTime? LastBackupDate { get; set; }
        public DateTime? LastRestoreDate { get; set; }
        public DateTime? LastHealthCheckDate { get; set; }
        public string SchemaVersion { get; set; }
        public DatabaseStatus Status { get; set; }
    }
}
