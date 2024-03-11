using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Suityou.Framework.Web.Common;

namespace Suityou.Framework.Web.Service
{
    public class AppSettingService
    {
        public string CSSStyle (string AppSettingFile)
        {
            return SettingsManager.GetCSSStyle(AppSettingFile);
        }
    }
}
