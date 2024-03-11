using Suityou.Framework.Web.Common;
using Suityou.Framework.Web.DataModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suityou.Framework.Web.PageUtil
{
    public class PageDisplayManager
    {
        #region staticメソッド
        #region GetDisplayValue(ColumnDefinition, DataSet, string)
        public static string GetDisplayValue (ColumnDefinition ColDef, DataSet DsSubTable, string ColVal)
        {
            string returnValue = string.Empty;

            // 外部参照カラムでも選択値カラムでもない
            if (ColDef.ExtAttrs.SelectValue == null && ColDef.ExtAttrs.ReferenceTo == null)
            {
                returnValue = ColVal;
            }
            // 選択値カラムの場合
            else if (ColDef.ExtAttrs.SelectValue != null)
            {
                string[] labelArr = ColDef.ExtAttrs.SelectValue.SelectValueField.Split (',');
                string[] valueArr = ColDef.ExtAttrs.SelectValue.SelectValue.Split(',');

                for (int i = 0; i < valueArr.Length; i++)
                {
                    if (ColVal.Equals(valueArr[i]))
                    {
                        returnValue = string.Format("{0} : {1}", valueArr[i], labelArr[i]);
                        break;
                    }
                }

                if (ColDef.ExtAttrs.SelectValue.SelectType.Equals("CheckBox") && returnValue.Equals(string.Empty))
                {
                    returnValue = "-";
                }
            }
            // 外部参照カラムの場合
            else if (ColDef.ExtAttrs.ReferenceTo != null)
            {
                // 外部参照テーブルから対象レコードを取得
                Dictionary<string, object> colDic = new Dictionary<string, object>();
                DataTable dtSubTable = DsSubTable.Tables[ColDef.ExtAttrs.ReferenceTo.TableName];
                colDic.Add(string.Format("{0}@{1}", ColDef.ExtAttrs.ReferenceTo.ValueColumnName, ColDef.ColumnType), ColVal);
                var query = dtSubTable.AsEnumerable().Where(CommonUtil.CreateExpressionTreeWhereAnd(colDic));
                if (query.Any())
                {
                    DataTable dtFilteredSubTable = query.CopyToDataTable();
                    returnValue = string.Format("{0} : {1}", dtFilteredSubTable.Rows[0][ColDef.ExtAttrs.ReferenceTo.ValueColumnName], dtFilteredSubTable.Rows[0][ColDef.ExtAttrs.ReferenceTo.CaptionColumnName]);
                }
                else
                {
                    returnValue = "N/A";
                }
            }

            return returnValue;
        }
        #endregion
        #endregion
    }
}
