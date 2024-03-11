using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Suityou.Framework.Web.DataModel;
using System.Security.Cryptography;
using System.Data;

namespace Suityou.Framework.Web.Common
{
    public class SettingsManager
    {
        #region メンバ変数
        #endregion

        #region コンストラクタ
        static SettingsManager()
        {

        }
        #endregion

        #region メソッド

        #region GetDBConnectionString (string) : DB接続文字列を取得する
        public static string GetDBConnectionString (string appSettingFile)
        {
            string JSONString = string.Empty;

            // JSONファイルが存在しない場合はnullを返却
            if (!File.Exists(appSettingFile))
            {
                return string.Empty;
            }

            // JSONファイルの内容をデシリアライズ(エラーが発生した場合はnullを返却)
            try
            {
                using (FileStream fs = new FileStream(appSettingFile, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        JSONString = sr.ReadToEnd();
                    }
                }
            }
            catch
            {
                return string.Empty;
            }

            ApplicationSetting? applicationSetting = JsonSerializer.Deserialize<ApplicationSetting>(JSONString);

            return string.IsNullOrEmpty(applicationSetting?.Setting?.DBConnectionString) ? string.Empty : applicationSetting.Setting.DBConnectionString;
        }
        #endregion

        #region GetDataInformation(string, string) : データ定義情報を取得する
        public static DataInformation GetDataInformation (string dataSettingFileFolder, string DID)
        {
            var dataInformation = new DataInformation();
            string JSONString = string.Empty;
            string dataSettingFile = dataSettingFileFolder + @"/" + DID + ".json";

            // JSONファイルが存在しない場合はnullを返却
            if (!File.Exists(dataSettingFile))
            {
                return null;
            }

            // JSONファイルの内容をデシリアライズ(エラーが発生した場合はnullを返却)
            try
            {
                using (FileStream fs = new FileStream(dataSettingFile, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        JSONString = sr.ReadToEnd();
                    }
                }
            }
            catch
            {
                return null;
            }

            DataDefinition? dataDef = JsonSerializer.Deserialize<DataDefinition>(JSONString);

            return dataDef.DataInformation;
        }
        #endregion

        #region GetValidationInformation(string, string) : バリデーション定義情報を取得する
        public static ValidationInformation GetValidationInformation(string validationSettingFolder, string DID)
        {
            string JSONString = string.Empty;
            string validationSettingFile = validationSettingFolder + @"/" + DID + "Validation.json";

            // JSONファイルが存在しない場合はnullを返却
            if (!File.Exists(validationSettingFile))
            {
                return null;
            }

            // JSONファイルの内容をデシリアライズ(エラーが発生した場合はnullを返却)
            try
            {
                using (FileStream fs = new FileStream(validationSettingFile, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        JSONString = sr.ReadToEnd();
                    }
                }
            }
            catch
            {
                return null;
            }

            ValidationDefinition? validDef = JsonSerializer.Deserialize<ValidationDefinition>(JSONString);

            return validDef.ValidationInformation;
        }
        #endregion

        #region GetAuditLogSetting(string) : 監査ログ設定を取得する
        public static AuditLogSetting GetAuditLogSetting(string appSettingFile)
        {
            string JSONString = string.Empty;

            // アプリケーション設定JSONファイルが存在しない場合はnullを返却
            if (!File.Exists(appSettingFile))
            {
                return null;
            }

            // アプリケーション設定JSONファイルの内容をデシリアライズ(エラーが発生した場合はnullを返却)
            try
            {
                using (FileStream fs = new FileStream(appSettingFile, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        JSONString = sr.ReadToEnd();
                    }
                }
            }
            catch
            {
                return null;
            }

            ApplicationSetting? applicationSetting = JsonSerializer.Deserialize<ApplicationSetting>(JSONString);

            // 監査ログ設定ファイルが未設定の場合はnullを返却
            if (applicationSetting.Setting.AuditLogSettingFilePath == null)
            {
                return null;
            }

            string auditLogSettingFile = applicationSetting.Setting.AuditLogSettingFilePath;

            // 監査ログ設定JSONファイルが存在しない場合はnullを返却
            if (!File.Exists(auditLogSettingFile))
            {
                return null;
            }

            // 監査ログ設定JSONファイルの内容をデシリアライズ(エラーが発生した場合はnullを返却)
            try
            {
                using (FileStream fs = new FileStream(auditLogSettingFile, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        JSONString = sr.ReadToEnd();
                    }
                }
            }
            catch
            {
                return null;
            }

            return JsonSerializer.Deserialize<AuditLogSetting>(JSONString);
        }
        #endregion

        #region GetCSSStyle (string) : CSSスタイルを取得する
        public static string GetCSSStyle(string appSettingFile)
        {
            string JSONString = string.Empty;

            // JSONファイルが存在しない場合はnullを返却
            if (!File.Exists(appSettingFile))
            {
                return string.Empty;
            }

            // JSONファイルの内容をデシリアライズ(エラーが発生した場合はnullを返却)
            try
            {
                using (FileStream fs = new FileStream(appSettingFile, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        JSONString = sr.ReadToEnd();
                    }
                }
            }
            catch
            {
                return string.Empty;
            }

            ApplicationSetting? applicationSetting = JsonSerializer.Deserialize<ApplicationSetting>(JSONString);

            return string.IsNullOrEmpty(applicationSetting?.Setting?.CSSStyle) ? string.Empty : applicationSetting.Setting.CSSStyle;
        }
        #endregion

        #region GetDBType (string) : DBタイプを取得する
        public static int GetDBType(string appSettingFile)
        {
            string JSONString = string.Empty;

            // JSONファイルが存在しない場合はnullを返却
            if (!File.Exists(appSettingFile))
            {
                return SuityouWFWConst.DBTYPE_NONE;
            }

            // JSONファイルの内容をデシリアライズ(エラーが発生した場合はnullを返却)
            try
            {
                using (FileStream fs = new FileStream(appSettingFile, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        JSONString = sr.ReadToEnd();
                    }
                }
            }
            catch
            {
                return SuityouWFWConst.DBTYPE_NONE;
            }

            ApplicationSetting? applicationSetting = JsonSerializer.Deserialize<ApplicationSetting>(JSONString);

            return (int)applicationSetting?.Setting?.DBType == null ? SuityouWFWConst.DBTYPE_NONE : (int)applicationSetting.Setting.DBType;
        }
        #endregion
        #endregion
    }
}
