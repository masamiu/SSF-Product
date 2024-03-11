using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Suityou.Framework.Web.DataManager;

namespace Suityou.Framework.Web.DataModel
{
    public class UploadData
    {
        #region コンストラクタ
        public UploadData(NormalDataManager Dm)
        {
            dm = Dm;
        }
        #endregion

        public NormalDataManager dm { get; set; }
    }
}
