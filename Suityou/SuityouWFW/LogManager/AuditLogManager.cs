using Suityou.Framework.Web.Common;
using Suityou.Framework.Web.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suityou.Framework.Web.LogManager
{
    public class AuditLogManager
    {
        #region プロパティ
        protected AuditLogSetting _setting;
        protected DatabaseOperator _databaseOperator;
        #region 監査ログテーブルカラム定義
        protected List<string> AuditLogTableColumns = new List<string>() { "SEQ", "OPERATION_DATETIME", "OPERATOR", "TARGET_DATA", "OPERATION", "NOTES" };
        #endregion
        #endregion

        #region コンストラクタ
        #region AuditLogManager(AuditLogSetting, DatabaseOperator)
        public AuditLogManager(AuditLogSetting Setting, DatabaseOperator DatabaseOperator)
        {
            _setting = Setting;
            _databaseOperator = DatabaseOperator;
        }
        #endregion
        #endregion

        #region メソッド

        #region publicメソッド
        #region Initialize - 初期化処理
        public bool Initialize()
        {
            bool retResult = true;

            // 監査ログタイプに応じた正常性確認
            // DB
            if (_setting.LogSetting.AuditLogType == SuityouWFWConst.AUDITLOG_TYPE_DB)
            {
                // 監査ログ格納テーブル名が未設定または存在しない場合は正常性エラー
                if (_setting.LogSetting.AuditLogTableName == null)
                {
                    // 監査ログ設定が存在しない
                    return false;
                }
                else if (string.IsNullOrEmpty(_setting.LogSetting.AuditLogTableName))
                {
                    // 監査ログ設定が未設定
                    return false;
                }
                else
                {
                    // 監査ログ格納テーブルの存在確認
                    string chkSQL1 = @$"
SELECT * 
FROM sys.tables
WHERE type = 'U'
AND name = '{_setting.LogSetting.AuditLogTableName}'";
                    DataTable dtChkResult = _databaseOperator.ExecuteTable(chkSQL1);
                    int auditLogTableObjectID;
                    if (dtChkResult.Rows.Count == 0)
                    {
                        return false;
                    }
                    else
                    {
                        auditLogTableObjectID = (int)dtChkResult.Rows[0]["object_id"];
                    }

                    // 監査ログ格納テーブルの定義チェック
                    string chkSQL2 = @$"
SELECT *
FROM sys.all_columns
WHERE object_id = {auditLogTableObjectID}
ORDER BY column_id
";
                    dtChkResult = _databaseOperator.ExecuteTable(chkSQL2);
                    List<string> auditLogTableColList = new List<string>();
                    foreach (DataRow drAuditLogCol in dtChkResult.Rows)
                    {
                        auditLogTableColList.Add(drAuditLogCol["name"].ToString());
                    }
                    foreach (string colName in AuditLogTableColumns)
                    {
                        if (!auditLogTableColList.Exists(col => col.Equals(colName)))
                        {
                            return false;
                        }
                    }
                }
            }
            // ファイル
            else if (_setting.LogSetting.AuditLogType == SuityouWFWConst.AUDITLOG_TYPE_FILE)
            {
                // 監査ログファイル格納フォルダが未設定またはフォルダが存在しない場合は正常性エラー
                if (_setting.LogSetting.AuditLogFolderPath == null)
                {
                    return false;
                }
                else if (!Directory.Exists(_setting.LogSetting.AuditLogFolderPath))
                {
                    return false;
                }

                // 監査ログファイルローテーション設定が未設定または不正な設定の場合はローテーションなしとする
                if (_setting.LogSetting.AuditLogFileLotation == null)
                {
                    _setting.LogSetting.AuditLogFileLotation = SuityouWFWConst.AUDITLOG_FILE_LOTATION_NONE;
                }
                else
                {
                    switch (_setting.LogSetting.AuditLogFileLotation)
                    {
                        case SuityouWFWConst.AUDITLOG_FILE_LOTATION_NONE:
                        case SuityouWFWConst.AUDITLOG_FILE_LOTATION_MONTHLY:
                        case SuityouWFWConst.AUDITLOG_FILE_LOTATION_DAILY:
                            break;
                        default:
                            _setting.LogSetting.AuditLogFileLotation = SuityouWFWConst.AUDITLOG_FILE_LOTATION_NONE;
                            break;
                    }
                }

                // 監査ログファイル接頭子設定が未設定または不正な設定の場合はデフォルトの接頭子とする
                if (_setting.LogSetting.AuditLogFilePrefix == null)
                {
                    _setting.LogSetting.AuditLogFilePrefix = SuityouWFWConst.AUDITLOG_FILE_PREFIX_DEFAULT;
                }
                else if (_setting.LogSetting.AuditLogFilePrefix.Trim().Equals(string.Empty))
                {
                    _setting.LogSetting.AuditLogFilePrefix = SuityouWFWConst.AUDITLOG_FILE_PREFIX_DEFAULT;
                }
            }
            // それ以外
            else
            {
                return false;
            }

            return retResult;
        }
        #endregion
        #region OutputAuditLog 監査ログ出力
        public void OutputAuditLog (AuditLogInfo LogParam)
        {
            // 監査ログ未設定時は処理終了
            if (_setting == null)
            {
                return;
            }

            // 監査ログ出力
            if (_setting.LogSetting.AuditLogType.Equals(SuityouWFWConst.AUDITLOG_TYPE_FILE))
            {
                OutputTextAuditLog(LogParam);
            }
            else if (_setting.LogSetting.AuditLogType.Equals(SuityouWFWConst.AUDITLOG_TYPE_DB))
            {
                OutputDBAuditLog(LogParam);
            }
        }
        #endregion
        #endregion

        #region privateメソッド
        #region テキスト監査ログ出力
        private void OutputTextAuditLog(AuditLogInfo LogParam)
        {
            string _logFileFolder = _setting.LogSetting.AuditLogFolderPath;
            string _logFileName = string.Empty;
            string _logContent = string.Empty;

            // ファイル名の設定
            DateTime outputDT = DateTime.Now;
            switch (_setting.LogSetting.AuditLogFileLotation)
            {
                case SuityouWFWConst.AUDITLOG_FILE_LOTATION_NONE:
                    _logFileName = string.Format("{0}.log", _setting.LogSetting.AuditLogFilePrefix);
                    break;
                case SuityouWFWConst.AUDITLOG_FILE_LOTATION_MONTHLY:
                    _logFileName = string.Format("{0}_{1}.log", _setting.LogSetting.AuditLogFilePrefix, outputDT.ToString("yyyyMM"));
                    break;
                case SuityouWFWConst.AUDITLOG_FILE_LOTATION_DAILY:
                    _logFileName = string.Format("{0}_{1}.log", _setting.LogSetting.AuditLogFilePrefix, outputDT.ToString("yyyyMMdd"));
                    break;
            }
            string _logFile = _logFileFolder + @"/" + _logFileName;

            // ログファイルがない場合は作成する
            if (!File.Exists(_logFile))
            {
                FileInfo fileInfo = new FileInfo(_logFile);
                FileStream fileStream = fileInfo.Create();
                fileStream.Close();
            }

            // 出力内容の生成
            _logContent = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\r\n", 
                LogParam.OperationDateTime.ToString("yyyy/MM/dd HH:mm:ss"),
                LogParam.Operator,
                LogParam.TargetData,
                LogParam.Operation,
                LogParam.Notes);

            // ファイル出力
            File.AppendAllText(_logFile, _logContent);
        }
        #endregion
        #region DB監査ログ出力
        private void OutputDBAuditLog(AuditLogInfo LogParam)
        {
            string execSQL = "INSERT INTO " + _setting.LogSetting.AuditLogTableName + "(";
            int colCounter = 0;
            foreach (string colName in AuditLogTableColumns)
            {
                if (colCounter != 0)
                {
                    if (colCounter != AuditLogTableColumns.Count - 1)
                    {
                        execSQL += colName + ", ";
                    }
                    else
                    {
                        execSQL += colName + ")";
                    }
                }
                colCounter++;
            }
            execSQL += $@"
VALUES (
    {LogParam.OperationDateTime.ToString("yyyy/MM/dd HH:mm:ss")}
  , {LogParam.Operator}
  , {LogParam.TargetData}
  , {LogParam.Operation}
  , {LogParam.Notes}
)";

            // 監査ログレコード登録
            _databaseOperator.ExecuteNonQuery(execSQL, new Dictionary<string, Dictionary<string, object>>(), null);
        }
        #endregion
        #endregion

        #endregion
    }
}
