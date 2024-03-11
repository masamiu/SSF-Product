using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suityou.Framework.Web.DataManager
{
    #region IDataManagerインターフェース
    // <summary>
    /// DataManagerが実装するインタフェース
    /// </summary>
    public interface IDataManager
    {
        #region メソッド

        #region GetData() : 対象データを取得する(フィルタなし)
        DataSet GetData();
        #endregion

        #region GetData(Dictionary) : 対象データを取得する(フィルタあり)
        DataSet GetData(Dictionary<string, object> Filter);
        #endregion

        #region AddData(DataRow) : 対象データを追加する
        int AddData(DataRow AddRows);
        #endregion

        #region UpdateData(DataRow) : 対象データを更新する
        int UpdateData(DataRow UpdateRows);
        #endregion

        #region DeleteData(DataRow) : 対象データを削除する
        int DeleteData(DataRow DeleteRows);
        #endregion

        #endregion
    }
    #endregion
}
