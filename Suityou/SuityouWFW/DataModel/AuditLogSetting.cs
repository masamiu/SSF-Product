using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suityou.Framework.Web.DataModel
{
    public class AuditLogSetting
    {
        public LogSetting? LogSetting { get; set; }
    }

    public class LogSetting
    {
        public int AuditLogType { get; set; }
        public string? AuditLogFolderPath { get; set; }
        public string? AuditLogFileLotation { get; set; }
        public string? AuditLogFilePrefix { get; set; }
        public string? AuditLogTableName { get; set; }
    }
}
