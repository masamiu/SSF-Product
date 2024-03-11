using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suityou.Framework.Web.DataModel
{
    public class AuditLogInfo
    {
        public DateTime OperationDateTime { get; set; }
        public string? Operator { get; set; }
        public string TargetData { get; set; }
        public string Operation { get; set; }
        public string Notes { get; set; }
    }
}
