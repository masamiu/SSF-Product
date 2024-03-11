using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suityou.Framework.Web.Common
{
    public class SuityouWFWConst
    {
        #region データベース定義
        public const int DBTYPE_MSSQL = 0;
        public const int DBTYPE_PGSQL = 1;
        public const int DBTYPE_MYSQL = 2;
        public const int DBTYPE_NONE = -1;
        #endregion

        #region データ定義関連
        public const int TABLE_INDEX_MAIN = 0;
        #endregion

        #region カラム型定義
        public const string COLUMN_TYPE_INT = "int";
        public const string COLUMN_TYPE_DOUBLE = "double";
        public const string COLUMN_TYPE_STRING = "string";
        public const string COLUMN_TYPE_BOOL = "bool";
        public const string COLUMN_TYPE_DATETIME = "datetime";
        public const string COLUMN_TYPE_DECIMAL = "decimal";
        public const string COLUMN_TYPE_NUMERIC = "numeric";
        #endregion

        #region 画面操作種類
        public const string WEB_OPERATION_ADD = "add";
        public const string WEB_OPERATION_MOD = "mod";
        public const string WEB_OPERATION_DEL = "del";
        public const string WEB_OPERATION_DETAIL = "detail";
        public const string WEB_OPERATION_LIST = "list";
        public const string WEB_OPERATION_BACK = "back";
        #endregion

        #region 遷移元タイプ
        public const string FROM_TYPE_LIST = "list";
        public const string FROM_TYPE_DETAIL = "detail";
        #endregion

        #region バリデーションタイプ
        public const string VALIDATION_TYPE_REQUIRED = "Required";
        public const string VALIDATION_TYPE_ISINTEGER = "IsInteger";
        public const string VALIDATION_TYPE_ISDECIMAL = "IsDecimal";
        public const string VALIDATION_TYPE_STRINGLENGTH = "StringLength";
        public const string VALIDATION_TYPE_SELECTVALUE = "SelectValue";
        #endregion

        #region 監査ログ関連
        public const int AUDITLOG_TYPE_FILE = 1;
        public const int AUDITLOG_TYPE_DB = 2;

        public const string AUDITLOG_FILE_LOTATION_NONE = "None";
        public const string AUDITLOG_FILE_LOTATION_DAILY = "Daily";
        public const string AUDITLOG_FILE_LOTATION_MONTHLY = "Monthly";

        public const string AUDITLOG_FILE_PREFIX_DEFAULT = "AuditLog";

        public const string AUDITLOG_OPERATION_ADD = "Add";
        public const string AUDITLOG_OPERATION_UPD = "Update";
        public const string AUDITLOG_OPERATION_DEL = "Delete";
        #endregion

        #region スタイル
        public const string CSS_STYLE_STANDARD = "standard";
        public const string CSS_STYLE_BLUE = "blue";
        public const string CSS_STYLE_GREEN = "green";
        public const string CSS_STYLE_ORANGE = "orange";
        #endregion

        #region アップロード
        public const int ERRORCODE_READFILE_NO_DATA = 1;
        public const int ERRORCODE_READFILE_FORMAT_ERROR = 2;
        #endregion
    }
}
