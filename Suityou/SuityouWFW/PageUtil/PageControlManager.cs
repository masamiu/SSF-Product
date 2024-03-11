using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSW.Service;
using Suityou.Framework.Web.Common;
using Suityou.Framework.Web.DataModel;

namespace Suityou.Framework.Web.PageUtil
{
    public class PageControlManager
    {
        private DataInformation dataInfo;
        private TableDefinition? mainTableDef;
        private TableDefinition[]? subTableDefs;
        private PageForm formData;
        private DataSet dsSubTable;

        #region コンストラクタ
        #region PageControlManager(DataInformation, PageForm, DataSet)
        public PageControlManager(DataInformation DataInfo, PageForm FormData, DataSet DsSubTable)
        {
            dataInfo = DataInfo;
            mainTableDef = dataInfo.MainTable;
            if (dataInfo.SubTables?.Length > 0)
            {
                subTableDefs = dataInfo.SubTables;
            }
            formData = FormData;
            dsSubTable = DsSubTable;
        }
        #endregion
        #endregion

        #region publicメソッド
        #region GeneratePageControlInformation : ページ制御情報作成
        public Dictionary<string, PageControlInfomation> GeneratePageControlInformation ()
        {
            Dictionary<string, PageControlInfomation> retDic = new Dictionary<string, PageControlInfomation>();

            foreach (FormItem formItem in formData.FormItems)
            {
                foreach (ColumnDefinition colDef in mainTableDef.Columns)
                {
                    if (colDef.ColumnName.Equals(formItem.Name))
                    {
                        PageControlInfomation controlInfo = new PageControlInfomation();

                        // 表示名設定
                        controlInfo.DisplayName = colDef.ExtAttrs.DisplayName == null ? colDef.ColumnName : colDef.ExtAttrs.DisplayName;
                        // カラム型設定
                        controlInfo.ColumnType = colDef.ColumnType;
                        // 主キー設定
                        controlInfo.IsPrimaryKey = colDef.IsPrimaryKey;
                        // 編集可能
                        controlInfo.IsEditable = colDef.ExtAttrs.IsEditable;
                        // 登録タイムスタンプ
                        if (colDef.ExtAttrs.IsNewTimeStamp == null)
                        {
                            controlInfo.IsNewTimeStamp = false;
                        }
                        else
                        {
                            controlInfo.IsNewTimeStamp = (bool)colDef.ExtAttrs.IsNewTimeStamp;
                        }
                        // 更新タイムスタンプ
                        if (colDef.ExtAttrs.IsUpdateTimeStamp == null)
                        {
                            controlInfo.IsUpdateTimeStamp = false;
                        }
                        else
                        {
                            controlInfo.IsUpdateTimeStamp = (bool)colDef.ExtAttrs.IsUpdateTimeStamp;
                        }
                        // 外部参照
                        if (colDef.ExtAttrs.ReferenceTo != null)
                        {
                            controlInfo.IsReference = true;
                            controlInfo.ReferenceTable = colDef.ExtAttrs.ReferenceTo.TableName;
                            controlInfo.RefValueColumn = colDef.ExtAttrs.ReferenceTo.ValueColumnName;
                            controlInfo.RefCaptionColumn = colDef.ExtAttrs.ReferenceTo.CaptionColumnName;
                            foreach (TableDefinition subTableDef in subTableDefs)
                            {
                                if (subTableDef.TableName.Equals(colDef.ExtAttrs.ReferenceTo.TableName))
                                {
                                    controlInfo.ParentKeys = new List<string>();
                                    foreach (KeyReferenceInformation keyRef in subTableDef.KeyReference)
                                    {
                                        if (!keyRef.ColumnName.Equals(colDef.ColumnName))
                                        {
                                            controlInfo.ParentKeys.Add(keyRef.ColumnName);
                                        }
                                    }

                                    if (controlInfo.ParentKeys.Count == 0)
                                    {
                                        // 自身のみの単一キー外部参照の場合
                                        controlInfo.ReferenceData = dsSubTable.Tables[subTableDef.TableName];
                                    }
                                    else
                                    {
                                        // 複合キー外部参照の場合
                                        bool isKeyValueSet = true;
                                        Dictionary<string, object> colDic = new Dictionary<string, object>();
                                        foreach (string keyColumnName in controlInfo.ParentKeys)
                                        {
                                            foreach (FormItem fItem in formData.FormItems)
                                            {
                                                if (fItem.Name.Equals(keyColumnName))
                                                {

                                                    switch (colDef.ColumnType)
                                                    {
                                                        case SuityouWFWConst.COLUMN_TYPE_STRING:
                                                            if (string.IsNullOrEmpty((string)fItem.Value))
                                                            {
                                                                isKeyValueSet = false;
                                                                colDic.Add(string.Format("{0}@{1}", keyColumnName, SuityouWFWConst.COLUMN_TYPE_STRING), string.Empty);
                                                            }
                                                            else
                                                            {
                                                                colDic.Add(string.Format("{0}@{1}", keyColumnName, SuityouWFWConst.COLUMN_TYPE_STRING), (string)fItem.Value);
                                                            }
                                                            break;
                                                        case SuityouWFWConst.COLUMN_TYPE_INT:
                                                            if ((int)fItem.Value == null)
                                                            {
                                                                isKeyValueSet = false;
                                                                colDic.Add(string.Format("{0}@{1}", keyColumnName, SuityouWFWConst.COLUMN_TYPE_INT), null);
                                                            }
                                                            else
                                                            {
                                                                colDic.Add(string.Format("{0}@{1}", keyColumnName, SuityouWFWConst.COLUMN_TYPE_INT), (int)fItem.Value);
                                                            }
                                                            break;
                                                        case SuityouWFWConst.COLUMN_TYPE_BOOL:
                                                            if ((bool)fItem.Value == null)
                                                            {
                                                                isKeyValueSet = false;
                                                                colDic.Add(string.Format("{0}@{1}", keyColumnName, SuityouWFWConst.COLUMN_TYPE_BOOL), null);
                                                            }
                                                            else
                                                            {
                                                                colDic.Add(string.Format("{0}@{1}", keyColumnName, SuityouWFWConst.COLUMN_TYPE_BOOL), (bool)fItem.Value);
                                                            }
                                                            break;
                                                        case SuityouWFWConst.COLUMN_TYPE_DATETIME:
                                                            if ((DateTime)fItem.Value == null)
                                                            {
                                                                isKeyValueSet = false;
                                                                colDic.Add(string.Format("{0}@{1}", keyColumnName, SuityouWFWConst.COLUMN_TYPE_DATETIME), null);
                                                            }
                                                            else
                                                            {
                                                                colDic.Add(string.Format("{0}@{1}", keyColumnName, SuityouWFWConst.COLUMN_TYPE_DATETIME), (DateTime)fItem.Value);
                                                            }
                                                            break;
                                                    }
                                                }
                                            }
                                        }
                                        if (isKeyValueSet)
                                        {
                                            DataTable dtSubTable = dsSubTable.Tables[subTableDef.TableName];
                                            var query = dtSubTable.AsEnumerable().Where(CommonUtil.CreateExpressionTreeWhereAnd(colDic));
                                            if (query.Any())
                                            {
                                                controlInfo.ReferenceData = query.CopyToDataTable();
                                            }
                                            else
                                            {
                                                controlInfo.ReferenceData = new DataTable();
                                            }
                                        }
                                        else
                                        {
                                            controlInfo.ReferenceData = new DataTable();
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            controlInfo.IsReference = false;
                        }
                        // 値選択
                        if (colDef.ExtAttrs.SelectValue != null)
                        {
                            controlInfo.IsSelectValue = true;
                            controlInfo.SelectType = colDef.ExtAttrs.SelectValue.SelectType;

                            var tmpValueFieldArr = colDef.ExtAttrs.SelectValue.SelectValueField.Split(',');
                            var tmpValueArr = colDef.ExtAttrs.SelectValue.SelectValue.Split(',');

                            if (tmpValueFieldArr.Length == tmpValueArr.Length)
                            {
                                Dictionary<string, string> tmpSelValDic = new Dictionary<string, string>();
                                for (int i = 0; i < tmpValueFieldArr.Length; i++)
                                {
                                    if (controlInfo.SelectType.Equals("CheckBox"))
                                    {
                                        tmpSelValDic.Add(tmpValueArr[i], tmpValueFieldArr[i]);
                                    }
                                    else
                                    {
                                        tmpSelValDic.Add(tmpValueArr[i], tmpValueFieldArr[i]);
                                    }
                                }
                                controlInfo.SelectValueDic = tmpSelValDic;
                            }
                        }
                        else
                        {
                            controlInfo.IsSelectValue = false;
                        }
                        // 外部参照キー
                        controlInfo.IsParentKeyColumn = false;
                        controlInfo.ParentKeyOf = new List<string>();
                        if (subTableDefs?.Length > 0)
                        {
                            foreach (TableDefinition subTableDef in subTableDefs)
                            {
                                if (subTableDef.KeyReference != null && subTableDef.KeyReference.Length > 1)
                                {
                                    foreach (KeyReferenceInformation keyRef in subTableDef.KeyReference)
                                    {
                                        if (keyRef.ColumnName.Equals(colDef.ColumnName))
                                        {
                                            controlInfo.IsParentKeyColumn = true;
                                            controlInfo.ParentKeyOf.Add(subTableDef.TableName);
                                        }
                                    }
                                }
                            }
                        }

                        retDic.Add(colDef.ColumnName, controlInfo);
                    }
                }
            }

            return retDic;
        }
        #endregion
        #endregion
    }

    public record PageControlInfomation
    {
        public string DisplayName { get; set; }
        public string ColumnType { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsEditable { get; set; }
        public bool IsNewTimeStamp { get; set; }
        public bool IsUpdateTimeStamp { get; set; }
        public bool IsReference { get; set; }
        public string? ReferenceTable { get; set; }
        public List<string>? ParentKeys { get; set; }
        public DataTable? ReferenceData { get; set; }
        public string? RefValueColumn { get; set; }
        public string? RefCaptionColumn { get; set; }
        public bool IsParentKeyColumn { get; set; }
        public List<string>? ParentKeyOf { get; set; }
        public bool IsSelectValue { get; set; }
        public string? SelectType { get; set; }
        public Dictionary<string, string>? SelectValueDic { get; set; }


    }
}
