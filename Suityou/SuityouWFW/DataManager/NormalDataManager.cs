using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Suityou.Framework.Web.Common;
using Suityou.Framework.Web.DataModel;
using Suityou.Framework.Web.LogManager;
using System.Security.Claims;

namespace Suityou.Framework.Web.DataManager
{
    public class NormalDataManager : IDataManager
    {
        #region プロパティ
        protected string DataID = string.Empty;
        public string DataName = string.Empty;
        protected DataInformation dataInfo;
        protected SQLManager sqlManager;
        protected string connString = string.Empty;
        protected int DbType;
        protected DatabaseOperator databaseOperator;
        protected ClaimsPrincipal loginUser;
        protected AuditLogSetting auditLogSetting;
        protected AuditLogManager auditLogManager;
        #endregion

        #region コンストラクタ
        #region NormalDataManager(int, string, string, string)
        public NormalDataManager(string ApplicationSettingFile, string DataDefinitionFolder, string DID, ClaimsPrincipal LoginUser)
        {
            // 初期設定
            DataID = DID;
            dataInfo = SettingsManager.GetDataInformation(DataDefinitionFolder, DataID);
            DataName = dataInfo.DataName;
            loginUser = LoginUser;

            sqlManager = new SQLManager(dataInfo);

            auditLogSetting = SettingsManager.GetAuditLogSetting(ApplicationSettingFile);
            auditLogManager = new AuditLogManager(auditLogSetting, databaseOperator);
            if (!auditLogManager.Initialize())
            {
                // 監査ログ設定不備の場合は監査ログは無効
                auditLogSetting = null;
                auditLogManager = new AuditLogManager(auditLogSetting, databaseOperator);
            }

            DbType = SettingsManager.GetDBType(ApplicationSettingFile);
            connString = SettingsManager.GetDBConnectionString(ApplicationSettingFile);
            databaseOperator = new DatabaseOperator(connString, DbType, auditLogManager);
        }
        #endregion
        #endregion

        #region メソッド

        #region virtual
        #region GetData() : 対象データを取得する(フィルタなし)
        public virtual DataSet GetData()
        {
            // SQL文の生成
            string execSql = sqlManager.CreateSQLStringForSelect(SuityouWFWConst.TABLE_INDEX_MAIN);

            return databaseOperator.ExecuteSet(execSql);
        }
        #endregion
        #region GetData(Dictionary) : 対象データを取得する(フィルタあり)
        public virtual DataSet GetData(Dictionary<string, object> Filter)
        {
            Dictionary<string, Dictionary<string, object>> paramInfoDic = new Dictionary<string, Dictionary<string, object>>();

            // Filter対象カラムリストを生成しながら、パラメータ情報を生成
            List<string> filterColList = new List<string>();
            foreach (string colName in Filter.Keys)
            {
                filterColList.Add(colName);

                Dictionary<string, object> paramInfo = new Dictionary<string, object>();
                foreach (ColumnDefinition colDef in dataInfo.MainTable.Columns)
                {
                    if (colName.Equals(colDef.ColumnName))
                    {
                        paramInfo.Add("Type", colDef.ColumnType);
                        paramInfo.Add("Value", Filter[colName]);
                    }
                }

                paramInfoDic.Add(colName, paramInfo);
            }

            // SQL文の生成
            string execSql = sqlManager.CreateSQLStringForSelect(SuityouWFWConst.TABLE_INDEX_MAIN, filterColList);

            return databaseOperator.ExecuteSetWithParam(execSql, paramInfoDic);
        }
        #endregion
        #region GetAllSubData() : 対象サブデータをすべて取得する(フィルタなし)
        public virtual DataSet GetAllSubData()
        {
            DataSet dsReturn= new DataSet();

            // サブテーブルの定義がない場合は終了
            if (dataInfo.SubTables != null)
            {
                // 全サブテーブルに対して処理を行う
                int tableIndex = 1;
                foreach (TableDefinition tableDef in dataInfo.SubTables)
                {
                    // SQL文の生成
                    string execSql = sqlManager.CreateSQLStringForSelect(tableIndex);
                    DataTable dtData = databaseOperator.ExecuteTable(execSql);
                    dtData.TableName = tableDef.TableName;

                    dsReturn.Tables.Add(dtData);
                    tableIndex++;
                }
            }

            return dsReturn;
        }
        #endregion
        #region AddData(DataRow) : 対象データを追加する
        public virtual int AddData(DataRow AddRow)
        {
            // 監査ログ出力情報初期化
            AuditLogInfo auditLogInfo = new AuditLogInfo();
            auditLogInfo.TargetData = dataInfo.DataName;
            auditLogInfo.Operation = SuityouWFWConst.AUDITLOG_OPERATION_ADD;
            if (loginUser?.Identity is not null)
            {
                auditLogInfo.Operator = loginUser.Identity.Name;
            }
            else
            {
                auditLogInfo.Operator = string.Empty;
            }
            string auditLogNotes = string.Empty;

            // SQL文の生成
            string execSql = sqlManager.CreateSQLStringForInsert(SuityouWFWConst.TABLE_INDEX_MAIN);

            // パラメータ情報の生成
            Dictionary<string, Dictionary<string, object>> paramDic = new Dictionary<string, Dictionary<string, object>>();
            foreach (ColumnDefinition colDef in dataInfo.MainTable.Columns)
            {
                Dictionary<string, object> paramInfoDic = new Dictionary<string, object>();
                paramInfoDic.Add("Type", colDef.ColumnType);
                if (colDef.ExtAttrs?.IsNewTimeStamp == true || colDef.ExtAttrs?.IsUpdateTimeStamp == true )
                {
                    paramInfoDic.Add("Value", DateTime.Now);
                }
                else
                {
                    paramInfoDic.Add("Value", AddRow[colDef.ColumnName]);
                }

                paramDic.Add("@" + colDef.ColumnName, paramInfoDic);

                if (colDef.IsPrimaryKey)
                {
                    if (auditLogNotes.Equals(string.Empty))
                    {
                        auditLogNotes += string.Format("{0} = {1}", colDef.ColumnName, AddRow[colDef.ColumnName]);
                    }
                    else
                    {
                        auditLogNotes += string.Format(", {0} = {1}", colDef.ColumnName, AddRow[colDef.ColumnName]);
                    }
                }

            }

            auditLogInfo.Notes = auditLogNotes;

            return databaseOperator.ExecuteNonQuery(execSql, paramDic, auditLogInfo);
        }
        #endregion
        #region UpdateData(DataRow) : 対象データを更新する
        public virtual int UpdateData(DataRow UpdateRow)
        {
            // 監査ログ出力情報初期化
            AuditLogInfo auditLogInfo = new AuditLogInfo();
            auditLogInfo.TargetData = dataInfo.DataName;
            auditLogInfo.Operation = SuityouWFWConst.AUDITLOG_OPERATION_UPD;
            if (loginUser?.Identity is not null)
            {
                auditLogInfo.Operator = loginUser.Identity.Name;
            }
            else
            {
                auditLogInfo.Operator = string.Empty;
            }
            string auditLogNotes = string.Empty;

            // SQL文の生成
            string execSql = sqlManager.CreateSQLStringForUpdate(SuityouWFWConst.TABLE_INDEX_MAIN);

            // パラメータ情報の生成
            Dictionary<string, Dictionary<string, object>> paramDic = new Dictionary<string, Dictionary<string, object>>();
            foreach (ColumnDefinition colDef in dataInfo.MainTable.Columns)
            {
                Dictionary<string, object> paramInfoDic = new Dictionary<string, object>();
                paramInfoDic.Add("Type", colDef.ColumnType);
                if (colDef.ExtAttrs?.IsUpdateTimeStamp == true)
                {
                    paramInfoDic.Add("Value", DateTime.Now);
                }
                else
                {
                    paramInfoDic.Add("Value", UpdateRow[colDef.ColumnName]);
                }

                paramDic.Add("@" + colDef.ColumnName, paramInfoDic);

                if (colDef.IsPrimaryKey)
                {
                    if (auditLogNotes.Equals(string.Empty))
                    {
                        auditLogNotes += string.Format("{0} = {1}", colDef.ColumnName, UpdateRow[colDef.ColumnName]);
                    }
                    else
                    {
                        auditLogNotes += string.Format(", {0} = {1}", colDef.ColumnName, UpdateRow[colDef.ColumnName]);
                    }
                }
            }

            auditLogInfo.Notes = auditLogNotes;

            return databaseOperator.ExecuteNonQuery(execSql, paramDic, auditLogInfo);
        }
        #endregion
        #region DeleteData(DataRow) : 対象データを削除する
        public virtual int DeleteData(DataRow DeleteRow)
        {
            // 監査ログ出力情報初期化
            AuditLogInfo auditLogInfo = new AuditLogInfo();
            auditLogInfo.TargetData = dataInfo.DataName;
            auditLogInfo.Operation = SuityouWFWConst.AUDITLOG_OPERATION_DEL;
            if (loginUser?.Identity is not null)
            {
                auditLogInfo.Operator = loginUser.Identity.Name;
            }
            else
            {
                auditLogInfo.Operator = string.Empty;
            }
            string auditLogNotes = string.Empty;

            // SQL文の生成
            string execSql = sqlManager.CreateSQLStringForDelete(SuityouWFWConst.TABLE_INDEX_MAIN);

            // パラメータ情報の生成
            Dictionary<string, Dictionary<string, object>> paramDic = new Dictionary<string, Dictionary<string, object>>();
            foreach (ColumnDefinition colDef in dataInfo.MainTable.Columns)
            {
                if (colDef.IsPrimaryKey)
                {
                    Dictionary<string, object> paramInfoDic = new Dictionary<string, object>();
                    paramInfoDic.Add("Type", colDef.ColumnType);
                    if (colDef.ColumnType.Equals("datetime"))
                    {
                        paramInfoDic.Add("Value", DateTime.Now);
                    }
                    else
                    {
                        paramInfoDic.Add("Value", DeleteRow[colDef.ColumnName]);
                    }

                    paramDic.Add("@" + colDef.ColumnName, paramInfoDic);

                    if (auditLogNotes.Equals(string.Empty))
                    {
                        auditLogNotes += string.Format("{0} = {1}", colDef.ColumnName, DeleteRow[colDef.ColumnName]);
                    }
                    else
                    {
                        auditLogNotes += string.Format(", {0} = {1}", colDef.ColumnName, DeleteRow[colDef.ColumnName]);
                    }
                }
            }

            auditLogInfo.Notes = auditLogNotes;

            return databaseOperator.ExecuteNonQuery(execSql, paramDic, auditLogInfo);
        }
        #endregion
        #endregion

        #region public
        #region GetInformation : データ基本情報を取得する
        public string GetInformation ()
        {
            return string.Format("DataID : {0} DataName : {1}", DataID, DataName);
        }
        #endregion
        #region GetDataInformation : データ定義を取得する
        public DataInformation GetDataInformation()
        {
            return this.dataInfo;
        }
        #endregion
        #region ReadUploadFile : アップロードファイルを読み込む
        public DataTable ReadUploadFile(string UploadFilePath, ref List<int>ErrorList)
        {
            DataTable dtResult = new DataTable();
            int dataCount = 0;
            bool formatError = false;

            // 対象テーブル情報取得
            TableDefinition mainTableDef = dataInfo.MainTable;
            int uploadTargetColumn = mainTableDef.Columns.Length;
            
            // DataTable準備
            foreach (ColumnDefinition colDef in mainTableDef.Columns)
            {
                string colName = colDef.ColumnName;
                switch (colDef.ColumnType)
                {
                    case SuityouWFWConst.COLUMN_TYPE_INT:
                        dtResult.Columns.Add(colDef.ColumnName, typeof(int));
                        break;
                    case SuityouWFWConst.COLUMN_TYPE_DOUBLE:
                        dtResult.Columns.Add(colDef.ColumnName, typeof(double));
                        break;
                    case SuityouWFWConst.COLUMN_TYPE_STRING:
                        dtResult.Columns.Add(colDef.ColumnName, typeof(string));
                        break;
                    case SuityouWFWConst.COLUMN_TYPE_BOOL:
                        dtResult.Columns.Add(colDef.ColumnName, typeof(bool));
                        break;
                    case SuityouWFWConst.COLUMN_TYPE_DATETIME:
                        dtResult.Columns.Add(colDef.ColumnName, typeof(DateTime));
                        break;
                    case SuityouWFWConst.COLUMN_TYPE_DECIMAL:
                        dtResult.Columns.Add(colDef.ColumnName, typeof(decimal));
                        break;
                    case SuityouWFWConst.COLUMN_TYPE_NUMERIC:
                        dtResult.Columns.Add(colDef.ColumnName, typeof(decimal));
                        break;
                }
            }

            // ファイル読み込み
            using (StreamReader sReader = new StreamReader(UploadFilePath))
            {
                while (!sReader.EndOfStream)
                {
                    string dataLine = sReader.ReadLine();
                    string[] colValues = dataLine.Split(',');
                    
                    // フォーマットチェック
                    if (colValues.Length != uploadTargetColumn)
                    {
                        formatError = true;
                        break;
                    }

                    // DataTableに登録
                    DataRow drTarget = dtResult.NewRow();
                    int colSeq = 0;
                    foreach (ColumnDefinition colDef in mainTableDef.Columns)
                    {
                        // タイムスタンプカラムは未設定のまま
                        if (colDef.ExtAttrs!.IsNewTimeStamp! == true || colDef.ExtAttrs!.IsUpdateTimeStamp == true)
                        {
                            drTarget[colDef.ColumnName] = System.DBNull.Value;
                            continue;
                        }

                        switch (colDef.ColumnType)
                        {
                            case SuityouWFWConst.COLUMN_TYPE_INT:
                                if (colValues[colSeq].Equals(string.Empty))
                                {
                                    drTarget[colDef.ColumnName] = System.DBNull.Value;
                                }
                                else
                                {
                                    drTarget[colDef.ColumnName] = int.Parse(colValues[colSeq]);
                                }
                                break;
                            case SuityouWFWConst.COLUMN_TYPE_DOUBLE:
                                if (colValues[colSeq].Equals(string.Empty))
                                {
                                    drTarget[colDef.ColumnName] = System.DBNull.Value;
                                }
                                else
                                {
                                    drTarget[colDef.ColumnName] = double.Parse(colValues[colSeq]);
                                }
                                break;
                            case SuityouWFWConst.COLUMN_TYPE_STRING:
                                drTarget[colDef.ColumnName] = colValues[colSeq];
                                break;
                            case SuityouWFWConst.COLUMN_TYPE_BOOL:
                                if (colValues[colSeq].Equals(string.Empty))
                                {
                                    drTarget[colDef.ColumnName] = System.DBNull.Value;
                                }
                                else
                                {
                                    drTarget[colDef.ColumnName] = bool.Parse(colValues[colSeq]);
                                }
                                break;
                            case SuityouWFWConst.COLUMN_TYPE_DATETIME:
                                if (colValues[colSeq].Equals(string.Empty))
                                {
                                    drTarget[colDef.ColumnName] = System.DBNull.Value;
                                }
                                else
                                {
                                    drTarget[colDef.ColumnName] = DateTime.Parse(colValues[colSeq]);
                                }
                                break;
                            case SuityouWFWConst.COLUMN_TYPE_DECIMAL:
                                if (colValues[colSeq].Equals(string.Empty))
                                {
                                    drTarget[colDef.ColumnName] = System.DBNull.Value;
                                }
                                else
                                {
                                    drTarget[colDef.ColumnName] = decimal.Parse(colValues[colSeq]);
                                }
                                break;
                            case SuityouWFWConst.COLUMN_TYPE_NUMERIC:
                                if (colValues[colSeq].Equals(string.Empty))
                                {
                                    drTarget[colDef.ColumnName] = System.DBNull.Value;
                                }
                                else
                                {
                                    drTarget[colDef.ColumnName] = decimal.Parse(colValues[colSeq]);
                                }
                                break;
                        }
                        colSeq++;
                    }

                    dtResult.Rows.Add(drTarget);
                }
            }
            dataCount = dtResult.Rows.Count;

            // データが0件の場合
            if (dataCount == 0)
            {
                ErrorList.Add(SuityouWFWConst.ERRORCODE_READFILE_NO_DATA);
            }
            // フォーマット不正の場合
            else if (formatError)
            {
                ErrorList.Add(SuityouWFWConst.ERRORCODE_READFILE_FORMAT_ERROR);
            }

            return dtResult;
        }
        #endregion
        #region UploadData : アップロード
        public int UploadData(DataTable DtUploadData, bool DeleteData, ref Dictionary<int, string> DicError)
        {
            int addCount = 0;
            int dataIndex = 0;
            bool hasError = false;
            
            // DB接続、トランザクション開始
            databaseOperator.OpenConnection();
            databaseOperator.BeginTransaction();

            try
            {
                // 削除指定時は全データを削除
                if (DeleteData)
                {
                    // SQL文の生成
                    string execSql = sqlManager.CreateSQLStringForAllDelete(SuityouWFWConst.TABLE_INDEX_MAIN);

                    // SQL実行
                    Dictionary<string, Dictionary<string, object>> paramDic = new Dictionary<string, Dictionary<string, object>>();
                    AuditLogInfo auditLogInfo = new AuditLogInfo();
                    auditLogInfo.TargetData = dataInfo.DataName;
                    auditLogInfo.Operation = SuityouWFWConst.AUDITLOG_OPERATION_DEL;
                    if (loginUser?.Identity is not null)
                    {
                        auditLogInfo.Operator = loginUser.Identity.Name;
                    }
                    else
                    {
                        auditLogInfo.Operator = string.Empty;
                    }
                    string auditLogNotes = string.Empty;

                    databaseOperator.ExecuteNonQueryNoTransaction(execSql, paramDic, auditLogInfo);
                }

                // データ追加
                foreach (DataRow drTarget in DtUploadData.Rows)
                {
                    dataIndex++;

                    // 監査ログ出力情報初期化
                    AuditLogInfo auditLogInfo = new AuditLogInfo();
                    auditLogInfo.TargetData = dataInfo.DataName;
                    auditLogInfo.Operation = SuityouWFWConst.AUDITLOG_OPERATION_ADD;
                    if (loginUser?.Identity is not null)
                    {
                        auditLogInfo.Operator = loginUser.Identity.Name;
                    }
                    else
                    {
                        auditLogInfo.Operator = string.Empty;
                    }
                    string auditLogNotes = string.Empty;

                    // SQL文の生成
                    string execSql = sqlManager.CreateSQLStringForInsert(SuityouWFWConst.TABLE_INDEX_MAIN);

                    // パラメータ情報の生成
                    Dictionary<string, Dictionary<string, object>> paramDic = new Dictionary<string, Dictionary<string, object>>();
                    foreach (ColumnDefinition colDef in dataInfo.MainTable.Columns)
                    {
                        Dictionary<string, object> paramInfoDic = new Dictionary<string, object>();
                        paramInfoDic.Add("Type", colDef.ColumnType);
                        if (colDef.ExtAttrs?.IsNewTimeStamp == true || colDef.ExtAttrs?.IsUpdateTimeStamp == true)
                        {
                            paramInfoDic.Add("Value", DateTime.Now);
                        }
                        else
                        {
                            paramInfoDic.Add("Value", drTarget[colDef.ColumnName]);
                        }

                        paramDic.Add("@" + colDef.ColumnName, paramInfoDic);

                        if (colDef.IsPrimaryKey)
                        {
                            if (auditLogNotes.Equals(string.Empty))
                            {
                                auditLogNotes += string.Format("{0} = {1}", colDef.ColumnName, drTarget[colDef.ColumnName]);
                            }
                            else
                            {
                                auditLogNotes += string.Format(", {0} = {1}", colDef.ColumnName, drTarget[colDef.ColumnName]);
                            }
                        }

                    }

                    auditLogInfo.Notes = auditLogNotes;

                    addCount += databaseOperator.ExecuteNonQueryNoTransaction(execSql, paramDic, auditLogInfo);
                }
            }
            catch (Exception ex)
            {
                hasError = true;
                // トランザクションロールバック
                databaseOperator.RollbackTransaction();
                // エラー情報設定
                if (dataIndex == 0)
                {
                    addCount = -1;
                    DicError.Add(dataIndex, ex.ToString());
                    return addCount;
                }
                DicError.Add(dataIndex, ex.ToString());
            }
            finally
            {
                // エラー発生時はトランザクションロールバック
                if (hasError)
                {
                    databaseOperator.RollbackTransaction();
                }
                else
                {
                    databaseOperator.CommitTransaction();
                }

                // 切断
                databaseOperator.DisconnectConnection();
            }

            return addCount;
        }
        #endregion
        #endregion

        #endregion

    }
}
