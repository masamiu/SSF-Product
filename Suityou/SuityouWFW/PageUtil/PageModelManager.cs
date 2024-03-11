using Suityou.Framework.Web.Common;
using Suityou.Framework.Web.DataModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Suityou.Framework.Web.PageUtil
{
    public class PageModelManager
    {
        #region staticメソッド
        #region DataTable -> モデルリスト変換
        public static List<object> GetPageModelList(Type ModelType, DataTable DtData)
        {
            List<object> returnList = new List<object>();

            foreach (DataRow drData in DtData.Rows)
            {
                // モデルクラスの生成
                object modelInstance = Activator.CreateInstance(ModelType);

                // モデルクラスのプロパティ設定
                foreach (DataColumn dc in drData.Table.Columns)
                {
                    PropertyInfo propInfo = ModelType.GetProperty(dc.ColumnName);
                    if (drData[dc.ColumnName] is not System.DBNull)
                    {
                        propInfo.SetValue(modelInstance, drData[dc.ColumnName], null);
                    }
                    else
                    {
                        propInfo.SetValue(modelInstance, string.Empty, null);
                    }
                }

                returnList.Add(modelInstance);
            }

            return returnList;
        }
        #endregion
        #region DataRow -> モデル変換
        public static object GetPageModel(Type ModelType, TableDefinition TableDef)
        {
            object returnObj = null;

            // モデルクラスの生成
            returnObj = Activator.CreateInstance(ModelType);

            // DateTime型カラムの初期値に当日日付を設定
            foreach (ColumnDefinition colDef in TableDef.Columns)
            {
                switch (colDef.ColumnType)
                {
                    case SuityouWFWConst.COLUMN_TYPE_DATETIME:
                        PropertyInfo propInfo = ModelType.GetProperty(colDef.ColumnName);
                        propInfo.SetValue(returnObj, DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd")), null);
                        break;
                }
            }

            return returnObj;
        }
        public static object GetPageModel(Type ModelType, DataRow DrData)
        {
            object returnObj = null;

            // モデルクラスの生成
            returnObj = Activator.CreateInstance(ModelType);

            // モデルクラスのプロパティ設定
            foreach (DataColumn dc in DrData.Table.Columns)
            {
                PropertyInfo propInfo = ModelType.GetProperty(dc.ColumnName);
                if (DrData[dc.ColumnName] is not System.DBNull)
                {
                    propInfo.SetValue(returnObj, DrData[dc.ColumnName], null);
                }
                else
                {
                    propInfo.SetValue(returnObj, string.Empty, null);
                }
            }

            return returnObj;
        }
        #endregion

        #region モデル => DataRow変換
        public static DataRow GetDataRow(Type ModelType, object Model, TableDefinition TableDef)
        {
            DataRow drReturn = null;
            DataTable dt = new DataTable();

            foreach (ColumnDefinition colDef in TableDef.Columns)
            {
                switch (colDef.ColumnType)
                {
                    case SuityouWFWConst.COLUMN_TYPE_INT:
                        dt.Columns.Add(colDef.ColumnName, typeof(int));
                        break;
                    case SuityouWFWConst.COLUMN_TYPE_DOUBLE:
                        dt.Columns.Add(colDef.ColumnName, typeof(double));
                        break;
                    case SuityouWFWConst.COLUMN_TYPE_STRING:
                        dt.Columns.Add(colDef.ColumnName, typeof(string));
                        break;
                    case SuityouWFWConst.COLUMN_TYPE_BOOL:
                        dt.Columns.Add(colDef.ColumnName, typeof(bool));
                        break;
                    case SuityouWFWConst.COLUMN_TYPE_DATETIME:
                        dt.Columns.Add(colDef.ColumnName, typeof(DateTime));
                        break;
                    case SuityouWFWConst.COLUMN_TYPE_DECIMAL:
                        dt.Columns.Add(colDef.ColumnName, typeof(decimal));
                        break;
                    case SuityouWFWConst.COLUMN_TYPE_NUMERIC:
                        dt.Columns.Add(colDef.ColumnName, typeof(decimal));
                        break;
                }
            }
            drReturn = dt.NewRow();

            foreach (ColumnDefinition colDef in TableDef.Columns)
            {
                PropertyInfo propInfo = ModelType.GetProperty(colDef.ColumnName);
                var propValue = propInfo.GetValue(Model);
                if (propValue != null)
                {
                    drReturn[colDef.ColumnName] = propValue;
                }
                else
                {
                    drReturn[colDef.ColumnName] = System.DBNull.Value;
                }
            }

            return drReturn;
        }
        #endregion

        #region モデル => PageForm
        #region 登録・編集用
        public static PageForm GetPageFormByModel(object Model, Type ModelType, TableDefinition MainTableDef)
        {
            PageForm retPageForm = new PageForm();

            foreach (ColumnDefinition colDef in MainTableDef.Columns)
            {
                PropertyInfo propInfo = ModelType.GetProperty(colDef.ColumnName);
                var propValue = propInfo.GetValue(Model);

                if (propValue != null)
                {
                    switch (colDef.ColumnType)
                    {
                        case SuityouWFWConst.COLUMN_TYPE_INT:
                            retPageForm.AddItem(new FormItem { Name = colDef.ColumnName, Value = (int)int.Parse(propValue.ToString()) });
                            break;
                        case SuityouWFWConst.COLUMN_TYPE_DOUBLE:
                            retPageForm.AddItem(new FormItem { Name = colDef.ColumnName, Value = (double)double.Parse(propValue.ToString()) });
                            break;
                        case SuityouWFWConst.COLUMN_TYPE_STRING:
                            retPageForm.AddItem(new FormItem { Name = colDef.ColumnName, Value = (string)propValue.ToString() });
                            break;
                        case SuityouWFWConst.COLUMN_TYPE_BOOL:
                            retPageForm.AddItem(new FormItem { Name = colDef.ColumnName, Value = (bool)bool.Parse(propValue.ToString()) });
                            break;
                        case SuityouWFWConst.COLUMN_TYPE_DATETIME:
                            retPageForm.AddItem(new FormItem { Name = colDef.ColumnName, Value = (DateTime)DateTime.Parse(propValue.ToString()) });
                            break;
                        case SuityouWFWConst.COLUMN_TYPE_DECIMAL:
                            retPageForm.AddItem(new FormItem { Name = colDef.ColumnName, Value = (decimal)decimal.Parse(propValue.ToString()) });
                            break;
                        case SuityouWFWConst.COLUMN_TYPE_NUMERIC:
                            retPageForm.AddItem(new FormItem { Name = colDef.ColumnName, Value = (decimal)decimal.Parse(propValue.ToString()) });
                            break;
                    }
                }
                else
                {
                    switch (colDef.ColumnType)
                    {
                        case SuityouWFWConst.COLUMN_TYPE_STRING:
                            retPageForm.AddItem(new FormItem { Name = colDef.ColumnName, Value = string.Empty });
                            break;
                        default :
                            retPageForm.AddItem(new FormItem { Name = colDef.ColumnName });
                            break;
                    }
                }
            }

            return retPageForm;
        }
        #endregion
        #region 検索用
        public static PageForm GetPageFormForSearchByModel(object Model, Type ModelType, TableDefinition MainTableDef)
        {
            PageForm retPageForm = new PageForm();

            foreach (ColumnDefinition colDef in MainTableDef.Columns)
            {
                retPageForm.AddItem(new FormItem { Name = colDef.ColumnName, Value = string.Empty });
            }

            return retPageForm;
        }
        #endregion
        #endregion
        #region PageForm => モデル
        #region 同期処理
        public static object GetModelByPageForm (Dictionary<string, string> ColTypeDic, PageForm PForm, Type ModelType)
        {
            object retModel = Activator.CreateInstance(ModelType);

            foreach (FormItem formItem in PForm.FormItems)
            {
                // Formの値を取得
                var formValue = formItem.Value;

                // モデルに値を設定
                PropertyInfo propInfo = ModelType.GetProperty(formItem.Name);
                switch (ColTypeDic[formItem.Name])
                {
                    case SuityouWFWConst.COLUMN_TYPE_INT:
                        propInfo.SetValue(retModel, int.Parse(formValue.ToString()));
                        break;
                    case SuityouWFWConst.COLUMN_TYPE_DOUBLE:
                        propInfo.SetValue(retModel, double.Parse(formValue.ToString()));
                        break;
                    case SuityouWFWConst.COLUMN_TYPE_STRING:
                        propInfo.SetValue(retModel, formValue.ToString());
                        break;
                    case SuityouWFWConst.COLUMN_TYPE_BOOL:
                        propInfo.SetValue(retModel, CommonUtil.ConvertToBoolFromStringValue(formValue.ToString()));
                        break;
                    case SuityouWFWConst.COLUMN_TYPE_DATETIME:
                        propInfo.SetValue(retModel, DateTime.Parse(formValue.ToString()));
                        break;
                    case SuityouWFWConst.COLUMN_TYPE_DECIMAL:
                        propInfo.SetValue(retModel, decimal.Parse(formValue.ToString()));
                        break;
                    case SuityouWFWConst.COLUMN_TYPE_NUMERIC:
                        propInfo.SetValue(retModel, decimal.Parse(formValue.ToString()));
                        break;
                }
            }

            return retModel;
        }
        #endregion
        #endregion

        #region PageForm => フィルターDictionary変換
        #region 同期
        public static Dictionary<string, object> GetFilterDicFromPageForm(Dictionary<string, string> ColTypeDic, PageForm PForm)
        {
            Dictionary<string, object> retDic = new Dictionary<string, object>();

            foreach (FormItem formItem in PForm.FormItems)
            {
                var formValue = formItem.Value;

                if (formValue != null && !formValue.ToString().Equals(string.Empty))
                {
                    switch (ColTypeDic[formItem.Name])
                    {
                        case SuityouWFWConst.COLUMN_TYPE_INT:
                            retDic.Add(formItem.Name, int.Parse(formValue.ToString()));
                            break;
                        case SuityouWFWConst.COLUMN_TYPE_DOUBLE:
                            retDic.Add(formItem.Name, double.Parse(formValue.ToString()));
                            break;
                        case SuityouWFWConst.COLUMN_TYPE_STRING:
                            retDic.Add(formItem.Name, formValue.ToString());
                            break;
                        case SuityouWFWConst.COLUMN_TYPE_BOOL:
                            retDic.Add(formItem.Name, CommonUtil.ConvertToBoolFromStringValue(formValue.ToString()));
                            break;
                        case SuityouWFWConst.COLUMN_TYPE_DATETIME:
                            retDic.Add(formItem.Name, DateTime.Parse(formValue.ToString()));
                            break;
                        case SuityouWFWConst.COLUMN_TYPE_DECIMAL:
                            retDic.Add(formItem.Name, decimal.Parse(formValue.ToString()));
                            break;
                        case SuityouWFWConst.COLUMN_TYPE_NUMERIC:
                            retDic.Add(formItem.Name, decimal.Parse(formValue.ToString()));
                            break;
                    }
                }
            }

            return retDic;
        }
        #endregion
        #endregion
        #endregion
    }
}
