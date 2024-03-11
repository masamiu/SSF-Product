using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suityou.Framework.Web.DataModel
{
    public class ApplicationSetting
    {
        public Setting? Setting { get; set; }
    }

    public class Setting
    {
        public int? DBType { get; set; }
        public string? DBConnectionString { get; set; }
        public string? AuditLogSettingFilePath { get; set; }
        public string? CSSStyle { get; set; }
    }
}
