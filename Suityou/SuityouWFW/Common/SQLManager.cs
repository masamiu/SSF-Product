using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Suityou.Framework.Web.DataModel;
using Suityou.Framework.Web.Common;
using System.Data;

namespace Suityou.Framework.Web.Common
{
    public class SQLManager
    {
        #region メンバ変数
        // データ定義
        DataInformation _DataInfo;
        #endregion

        #region コンストラクタ
        public SQLManager (DataInformation DInfo)
        {
            _DataInfo = DInfo;
        }
        #endregion

        #region メソッド

        #region Public
        #region CreateSQLStringForSelect(int) : 対象テーブルのSELECT文を生成する(フィルタなし)
        public string CreateSQLStringForSelect (int TableIndex)
        {
            string sqlString = string.Empty;
            TableDefinition tableDef = null;

            // 対象のテーブル定義を取得
            if (TableIndex == SuityouWFWConst.TABLE_INDEX_MAIN)
            {
                tableDef = _DataInfo.MainTable;
            }
            else
            {
                tableDef = _DataInfo.SubTables[TableIndex - 1];
            }

            sqlString = string.Format("SELECT {0} FROM {1}", CreateSQLStringSelectPart(tableDef), tableDef.TableName);

            if (tableDef.Sort != null)
            {
                string orderString = " ";
                orderString += CreateSQLStringOrderPart(tableDef.Sort);
                sqlString += orderString;
            }

            return sqlString;
        }
        #endregion
        #region CreateSQLStringForSelect(int, Dictionary) : 対象テーブルのSELECT文を生成する(フィルタあり)
        public string CreateSQLStringForSelect(int TableIndex, List<string> FilterColList)
        {
            string sqlString = string.Empty;
            TableDefinition tableDef = null;

            // 対象のテーブル定義を取得
            if (TableIndex == SuityouWFWConst.TABLE_INDEX_MAIN)
            {
                tableDef = _DataInfo.MainTable;
            }
            else
            {
                tableDef = _DataInfo.SubTables[TableIndex - 1];
            }

            string selectString = string.Format("SELECT {0} FROM {1}", CreateSQLStringSelectPart(tableDef), tableDef.TableName);
            string whereString = CreateSQLStringWherePart(FilterColList);
            sqlString = string.Format("{0} {1}", selectString, whereString);

            if (tableDef.Sort != null)
            {
                string orderString = " ";
                orderString += CreateSQLStringOrderPart(tableDef.Sort);
                sqlString += orderString;
            }

            return sqlString;
        }
        #endregion
        #region CreateSQLStringForInsert(int, DataRow) : 対象テーブルのINSERT文を生成する
        public string CreateSQLStringForInsert (int TableIndex)
        {
            string sqlString = string.Empty;
            TableDefinition tableDef = null;

            // 対象のテーブル定義を取得
            if (TableIndex == SuityouWFWConst.TABLE_INDEX_MAIN)
            {
                tableDef = _DataInfo.MainTable;
            }
            else
            {
                tableDef = _DataInfo.SubTables[TableIndex - 1];
            }

            return CreateSQLStringInsert(tableDef);
        }
        #endregion
        #region CreateSQLStringForUpdate(int, DataRow) : 対象テーブルのUPDATE分を生成する
        public string CreateSQLStringForUpdate (int TableIndex)
        {
            string sqlString = string.Empty;
            TableDefinition tableDef = null;

            // 対象のテーブル定義を取得
            if (TableIndex == SuityouWFWConst.TABLE_INDEX_MAIN)
            {
                tableDef = _DataInfo.MainTable;
            }
            else
            {
                tableDef = _DataInfo.SubTables[TableIndex - 1];
            }

            // 主キーのListを生成
            List<string> pkList = new List<string>();
            foreach (ColumnDefinition col in tableDef.Columns)
            {
                // 主キーの場合Dictianaryに追加
                if (col.IsPrimaryKey)
                {
                    pkList.Add(col.ColumnName);
                }
            }

            string updateString = string.Format("UPDATE {0} {1}", tableDef.TableName, CreateSQLStringUpdateSetPart(tableDef));
            string whereString = CreateSQLStringWherePart(pkList);

            sqlString = string.Format("{0} {1}", updateString, whereString);

            return sqlString;
        }
        #endregion
        #region CreateSQLStringForDelete(int) : 対象テーブルのDELETE分を生成する
        public string CreateSQLStringForDelete(int TableIndex)
        {
            string sqlString = string.Empty;
            TableDefinition tableDef = null;

            // 対象のテーブル定義を取得
            if (TableIndex == SuityouWFWConst.TABLE_INDEX_MAIN)
            {
                tableDef = _DataInfo.MainTable;
            }
            else
            {
                tableDef = _DataInfo.SubTables[TableIndex - 1];
            }

            // 主キーのListを生成
            List<string> pkList = new List<string>();
            foreach (ColumnDefinition col in tableDef.Columns)
            {
                // 主キーの場合Dictianaryに追加
                if (col.IsPrimaryKey)
                {
                    pkList.Add(col.ColumnName);
                }
            }

            string deleteString = string.Format("DELETE FROM {0} ", tableDef.TableName);
            string whereString = CreateSQLStringWherePart(pkList);

            sqlString = string.Format("{0} {1}", deleteString, whereString);

            return sqlString;
        }
        #endregion
        #region CreateSQLStringForAllDelete(int) : 対象テーブルのDELETE分を生成する
        public string CreateSQLStringForAllDelete(int TableIndex)
        {
            string sqlString = string.Empty;
            TableDefinition tableDef = null;

            // 対象のテーブル定義を取得
            if (TableIndex == SuityouWFWConst.TABLE_INDEX_MAIN)
            {
                tableDef = _DataInfo.MainTable;
            }
            else
            {
                tableDef = _DataInfo.SubTables[TableIndex - 1];
            }

            sqlString = string.Format("DELETE FROM {0} ", tableDef.TableName);

            return sqlString;
        }
        #endregion
        #endregion

        #region Private
        #region CreateSQLStringSelectPart : 対象テーブルのSELECT文のSELECT句、FROM句を生成する
        private string CreateSQLStringSelectPart (TableDefinition TDef)
        {
            List<string> columnList = new List<string>();

            foreach (ColumnDefinition col in TDef.Columns)
            {
                columnList.Add(col.ColumnName);
            }

            return string.Join(", ", columnList.ToArray());
        }
        #endregion
        #region CreateSQLStringWherePart : WHERE句を生成する
        private string CreateSQLStringWherePart (List<string> FilterColList)
        {
            string whereString = string.Empty;

            foreach (string colName in FilterColList)
            {
                if (string.IsNullOrEmpty(whereString))
                {
                    whereString += string.Format("WHERE {0} = @{0} ", colName);
                }
                else
                {
                    whereString += string.Format("AND {0} = @{0} ", colName);
                }
            }

            return whereString;
        }
        #endregion
        #region CreateSQLStringOrderPart : ORDER BY句を生成する
        private string CreateSQLStringOrderPart(string[] Sort)
        {
            return string.Format("ORDER BY {0}", string.Join(", ", Sort));
        }
        #endregion
        #region CreateSQLStringInsert : 対象テーブルのINSERT文を生成する
        private string CreateSQLStringInsert (TableDefinition TDef)
        {
            string returnSQLString = "INSERT INTO {0} VALUES ({1})";

            List<string> columnList = new List<string>();
            foreach (ColumnDefinition col in TDef.Columns)
            {
                columnList.Add("@" + col.ColumnName);
            }

            string valuesColString = string.Join(", ", columnList.ToArray());
            returnSQLString = string.Format(returnSQLString, TDef.TableName, valuesColString);

            return returnSQLString;
        }
        #endregion
        #region CreateSQLStringUpdateSetPart : 対象テーブルのUPDATE文のSET句を生成する
        private string CreateSQLStringUpdateSetPart (TableDefinition TDef)
        {
            string returnSQLString = string.Empty;

            List<string> columnValueList = new List<string>();

            foreach (ColumnDefinition col in TDef.Columns)
            {
                if (string.IsNullOrEmpty(returnSQLString))
                {
                    returnSQLString += string.Format("SET {0} = @{0} ", col.ColumnName);
                }
                else
                {
                    returnSQLString += string.Format(", {0} = @{0}", col.ColumnName);
                }
            }

            return returnSQLString;
        }
        #endregion
        #endregion

        #endregion
    }
}
