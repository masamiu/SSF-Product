using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace Suityou.Framework.Web.Common
{
    public class CommonUtil
    {
        #region staticメソッド
        #region bool <=> string変換
        #region string => bool変換("0"/"1" => False/True)
        public static bool ConvertToBoolFromStringValue(string StringValue)
        {
            string tmpValue = string.Empty;
            switch (StringValue)
            {
                case "0":
                    tmpValue = "False";
                    break;
                case "1":
                    tmpValue = "True";
                    break;
            }

            return bool.Parse(tmpValue);
        }
        #endregion
        #region bool => string変換(False/True => "0"/"1" or "False"/"True")
        public static string ConvertToStringFromBoolValue(bool BoolValue, int ProcType)
        {
            string returnValue = string.Empty;
            switch (BoolValue)
            {
                case true:
                    if (ProcType == 1)
                    {
                        returnValue = "1";
                    }
                    else if (ProcType == 2)
                    {
                        returnValue = "True";
                    }
                    break;
                case false:
                    if (ProcType == 1)
                    {
                        returnValue = "0";
                    }
                    else if (ProcType == 2)
                    {
                        returnValue = "False";
                    }
                    break;
            }

            return returnValue;
        }
        #endregion
        #endregion
        #region Lambda関連
        #region AND式ツリー生成
        public static Func<DataRow, bool> CreateExpressionTreeWhereAnd(Dictionary<string, object> ColumnDic)
        {
            var parameterExp = Expression.Parameter(typeof(DataRow), "row");
            //Expression bodyExp = Expression.Constant(true);
            Expression bodyExp = null;
            foreach (string columnInfo in ColumnDic.Keys)
            {
                var tmpColumnInfo = columnInfo.Split('@');
                string columnName = tmpColumnInfo[0];
                string columnType = tmpColumnInfo[1];
                object columnValue = ColumnDic[columnInfo];

                Expression columnExp = Expression.Property(parameterExp, "Item", Expression.Constant(columnName));
                //Expression columnExp = Expression.Call(parameterExp, "Field", null, Expression.Constant(columnName));
                Expression toStringExp = Expression.Call(columnExp, "ToString", null);
                Expression valueExp = Expression.Constant(columnValue.ToString());
                //Expression equalExp = Expression.Equal(columnExp, valueExp);
                Expression equalExp = Expression.Equal(toStringExp, valueExp);

                if (bodyExp == null)
                {
                    bodyExp = equalExp;
                }
                else
                {
                    bodyExp = Expression.And(bodyExp, equalExp);
                }
            }
            return Expression.Lambda<Func<DataRow, bool>>(bodyExp, parameterExp).Compile();
        }
        #endregion
        #endregion
        #region ダウンロードデータ作成
        #region DataTable => ダウンロード用CSV Byteデータ
        public static byte[] GenerateDownloadCSVDataFromDataTable(DataTable DtTarget)
        {
            byte[] returnData = null;

            string csvData = string.Empty;
            foreach (DataRow drRow in DtTarget.Rows)
            {
                string rowData = string.Empty;
                foreach (DataColumn col in DtTarget.Columns)
                {
                    rowData += drRow[col.ColumnName].ToString() + ",";
                }
                rowData = rowData.Substring(0, rowData.Length - 1) + "\r\n";
                csvData += rowData;
            }

            returnData = Encoding.UTF8.GetBytes(csvData);

            return returnData;
        }
        #endregion
        #endregion
        #endregion

        #region privateメソッド
        #region Lambda関連
        #region パラメータ項目を作成
        private static ParameterExpression GetParameterExpression(string ColumnName, Type ColumnType)
        {
            return Expression.Parameter(ColumnType, ColumnName);
        }
        #endregion
        #endregion
        #endregion
    }
}
