using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSW.Service;
using Suityou.Framework.Web.DataModel;
using Suityou.Framework.Web.LogManager;

namespace Suityou.Framework.Web.Common
{
    public class DatabaseOperator
    {
        #region メンバ変数
        private string _connString = string.Empty;
        private int _dbType;
        private AuditLogManager _auditLogManager;
        #region MSSQL用
        private MSSQLDatabaseController mssqlController;
        private MSSQLDatabaseCommand mssqlCommand;
        #endregion

        #region PGSQL用
        private PGSQLDatabaseController pgsqlController;
        private PGSQLDatabaseCommand pgsqlCommand;
        #endregion
        #endregion

        #region コンストラクタ
        #region DatabaseOperator(string, int)
        public DatabaseOperator (string ConnString, int DBType)
        {
            _connString = ConnString;
            _dbType = DBType;
            _auditLogManager = null;

            switch (_dbType)
            {
                case SuityouWFWConst.DBTYPE_MSSQL:
                    InitializeMSSQLSetting();
                    break;
                case SuityouWFWConst.DBTYPE_PGSQL:
                    InitializePGSQLSetting();
                    break;
                case SuityouWFWConst.DBTYPE_NONE:
                    throw new Exception("DBType does not defined!!");
            }
        }
        #endregion
        #region DatabaseOperator(string, int, AuditLogManager)
        public DatabaseOperator (string ConnString, int DBType, AuditLogManager AuditLogManager) : this (ConnString, DBType)
        {
            _auditLogManager = AuditLogManager;
        }
        #endregion
        #endregion

        #region Publicメソッド(接続、トランザクション)

        #region OpenConnection
        public void OpenConnection()
        {
            mssqlController.Open();
        }
        #endregion

        #region DisconnectConnection
        public void DisconnectConnection()
        {
            mssqlController.Close();
        }
        #endregion

        #region BeginTransaction
        public void BeginTransaction()
        {
            mssqlController.BeginTransaction();
        }
        #endregion

        #region CommitTransaction
        public void CommitTransaction()
        {
            mssqlController.Commit();
        }
        #endregion

        #region RollbackTransaction
        public void RollbackTransaction()
        {
            mssqlController.Rollback();
        }
        #endregion

        #endregion

        #region Publicメソッド(SQL実行)

        #region ExecuteSet
        public DataSet ExecuteSet(string Query)
        {
            DataSet dsReturn = new DataSet();

            switch (_dbType)
            {
                case SuityouWFWConst.DBTYPE_MSSQL:
                    try
                    {
                        mssqlController.Open();
                        mssqlCommand = (MSSQLDatabaseCommand)mssqlController.CreateCommand(Query);
                        dsReturn = mssqlController.ExecuteSet(mssqlCommand);
                    }
                    finally
                    {
                        mssqlController.Close();
                    }
                    break;
                case SuityouWFWConst.DBTYPE_PGSQL:
                    try
                    {
                        pgsqlController.Open();
                        pgsqlCommand = (PGSQLDatabaseCommand)pgsqlController.CreateCommand(Query);
                        dsReturn = pgsqlController.ExecuteSet(pgsqlCommand);
                    }
                    finally
                    {
                        mssqlController.Close();
                    }
                    break;
            }

            return dsReturn;
        }
        #endregion

        #region ExecuteSetWithParam
        public DataSet ExecuteSetWithParam(string Query, Dictionary<string, Dictionary<string, object>> ParamDic)
        {
            DataSet dsReturn = new DataSet();

            switch (_dbType)
            {
                case SuityouWFWConst.DBTYPE_MSSQL:
                    try
                    {
                        mssqlController.Open();
                        mssqlCommand = (MSSQLDatabaseCommand)mssqlController.CreateCommand(Query);
                        foreach (string paramName in ParamDic.Keys)
                        {
                            string colType = ParamDic[paramName]["Type"].ToString();
                            switch (colType)
                            {
                                case SuityouWFWConst.COLUMN_TYPE_STRING:
                                    mssqlCommand.AddInputParameter(paramName, DbType.String, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_INT:
                                    mssqlCommand.AddInputParameter(paramName, DbType.Int32, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_DOUBLE:
                                    mssqlCommand.AddInputParameter(paramName, DbType.Double, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_DATETIME:
                                    mssqlCommand.AddInputParameter(paramName, DbType.DateTime, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_DECIMAL:
                                    mssqlCommand.AddInputParameter(paramName, DbType.Decimal, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_NUMERIC:
                                    mssqlCommand.AddInputParameter(paramName, DbType.Decimal, ParamDic[paramName]["Value"]);
                                    break;
                            }
                        }

                        dsReturn = mssqlController.ExecuteSet(mssqlCommand);
                    }
                    finally
                    {
                        mssqlController.Close();
                    }
                    break;
                case SuityouWFWConst.DBTYPE_PGSQL:
                    try
                    {
                        pgsqlController.Open();
                        pgsqlCommand = (PGSQLDatabaseCommand)pgsqlController.CreateCommand(Query);
                        foreach (string paramName in ParamDic.Keys)
                        {
                            string colType = ParamDic[paramName]["Type"].ToString();
                            switch (colType)
                            {
                                case SuityouWFWConst.COLUMN_TYPE_STRING:
                                    pgsqlCommand.AddInputParameter(paramName, DbType.String, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_INT:
                                    pgsqlCommand.AddInputParameter(paramName, DbType.Int32, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_DOUBLE:
                                    mssqlCommand.AddInputParameter(paramName, DbType.Double, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_DATETIME:
                                    pgsqlCommand.AddInputParameter(paramName, DbType.DateTime, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_DECIMAL:
                                    mssqlCommand.AddInputParameter(paramName, DbType.Decimal, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_NUMERIC:
                                    mssqlCommand.AddInputParameter(paramName, DbType.Decimal, ParamDic[paramName]["Value"]);
                                    break;
                            }
                        }

                        dsReturn = pgsqlController.ExecuteSet(pgsqlCommand);
                    }
                    finally
                    {
                        mssqlController.Close();
                    }
                    break;
            }

            return dsReturn;
        }
        #endregion

        #region ExecuteTable
        public DataTable ExecuteTable(string Query)
        {
            DataTable dtReturn = new DataTable();

            switch (_dbType)
            {
                case SuityouWFWConst.DBTYPE_MSSQL:
                    try
                    {
                        mssqlController.Open();
                        mssqlCommand = (MSSQLDatabaseCommand)mssqlController.CreateCommand(Query);
                        dtReturn = mssqlController.ExecuteTable(mssqlCommand);
                    }
                    finally
                    {
                        mssqlController.Close();
                    }
                    break;
                case SuityouWFWConst.DBTYPE_PGSQL:
                    try
                    {
                        pgsqlController.Open();
                        pgsqlCommand = (PGSQLDatabaseCommand)pgsqlController.CreateCommand(Query);
                        dtReturn = pgsqlController.ExecuteTable(pgsqlCommand);
                    }
                    finally
                    {
                        pgsqlController.Close();
                    }
                    break;
            }

            return dtReturn;
        }
        #endregion

        #region ExecuteTableWithParam
        public DataTable ExecuteTableWithParam(string Query, Dictionary<string, Dictionary<string, object>> ParamDic)
        {
            DataTable dtReturn = new DataTable();

            switch (_dbType)
            {
                case SuityouWFWConst.DBTYPE_MSSQL:
                    try
                    {
                        mssqlController.Open();
                        mssqlCommand = (MSSQLDatabaseCommand)mssqlController.CreateCommand(Query);
                        foreach (string paramName in ParamDic.Keys)
                        {
                            string colType = ParamDic[paramName]["Type"].ToString();
                            switch (colType)
                            {
                                case SuityouWFWConst.COLUMN_TYPE_STRING:
                                    mssqlCommand.AddInputParameter(paramName, DbType.String, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_INT:
                                    mssqlCommand.AddInputParameter(paramName, DbType.Int32, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_DOUBLE:
                                    mssqlCommand.AddInputParameter(paramName, DbType.Double, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_DATETIME:
                                    mssqlCommand.AddInputParameter(paramName, DbType.DateTime, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_DECIMAL:
                                    mssqlCommand.AddInputParameter(paramName, DbType.Decimal, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_NUMERIC:
                                    mssqlCommand.AddInputParameter(paramName, DbType.Decimal, ParamDic[paramName]["Value"]);
                                    break;
                            }
                        }

                        dtReturn = mssqlController.ExecuteTable(mssqlCommand);
                    }
                    finally
                    {
                        mssqlController.Close();
                    }
                    break;
                case SuityouWFWConst.DBTYPE_PGSQL:
                    try
                    {
                        pgsqlController.Open();
                        pgsqlCommand = (PGSQLDatabaseCommand)pgsqlController.CreateCommand(Query);
                        foreach (string paramName in ParamDic.Keys)
                        {
                            string colType = ParamDic[paramName]["Type"].ToString();
                            switch (colType)
                            {
                                case SuityouWFWConst.COLUMN_TYPE_STRING:
                                    pgsqlCommand.AddInputParameter(paramName, DbType.String, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_INT:
                                    pgsqlCommand.AddInputParameter(paramName, DbType.Int32, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_DOUBLE:
                                    mssqlCommand.AddInputParameter(paramName, DbType.Double, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_DATETIME:
                                    pgsqlCommand.AddInputParameter(paramName, DbType.DateTime, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_DECIMAL:
                                    mssqlCommand.AddInputParameter(paramName, DbType.Decimal, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_NUMERIC:
                                    mssqlCommand.AddInputParameter(paramName, DbType.Decimal, ParamDic[paramName]["Value"]);
                                    break;
                            }
                        }

                        dtReturn = pgsqlController.ExecuteTable(pgsqlCommand);
                    }
                    finally
                    {
                        pgsqlController.Close();
                    }
                    break;
            }

            return dtReturn;
        }
        #endregion

        #region ExecuteNonQuery
        public int ExecuteNonQuery(string Query, Dictionary<string, Dictionary<string, object>> ParamDic, AuditLogInfo? AuditLogParam)
        {
            int returnValue = 0;
            DateTime execDateTime;

            switch (_dbType)
            {
                case SuityouWFWConst.DBTYPE_MSSQL:
                    try
                    {
                        mssqlController.Open();

                        // トランザクションの開始
                        mssqlController.BeginTransaction();

                        // コマンドの実行
                        mssqlCommand = (MSSQLDatabaseCommand)mssqlController.CreateCommand(Query);
                        foreach (string paramName in ParamDic.Keys)
                        {
                            string colType = ParamDic[paramName]["Type"].ToString();
                            switch (colType)
                            {
                                case SuityouWFWConst.COLUMN_TYPE_STRING:
                                    mssqlCommand.AddInputParameter(paramName, DbType.String, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_INT:
                                    mssqlCommand.AddInputParameter(paramName, DbType.Int32, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_DOUBLE:
                                    mssqlCommand.AddInputParameter(paramName, DbType.Double, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_DATETIME:
                                    mssqlCommand.AddInputParameter(paramName, DbType.DateTime, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_DECIMAL:
                                    mssqlCommand.AddInputParameter(paramName, DbType.Decimal, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_NUMERIC:
                                    mssqlCommand.AddInputParameter(paramName, DbType.Decimal, ParamDic[paramName]["Value"]);
                                    break;
                            }
                        }

                        returnValue = mssqlController.ExecuteNonQuery(mssqlCommand);
                        execDateTime = DateTime.Now;

                        // 監査ログの出力
                        if (AuditLogParam != null)
                        {
                            AuditLogParam.OperationDateTime = execDateTime;
                            _auditLogManager.OutputAuditLog(AuditLogParam);
                        }

                        // コミット
                        mssqlController.Commit();
                    }
                    catch
                    {
                        // ロールバック
                        mssqlController.Rollback();
                    }
                    finally
                    {
                        mssqlController.Close();
                    }
                    break;
                case SuityouWFWConst.DBTYPE_PGSQL:
                    try
                    {
                        pgsqlController.Open();

                        // トランザクションの開始
                        pgsqlController.BeginTransaction();

                        // コマンドの実行
                        pgsqlCommand = (PGSQLDatabaseCommand)pgsqlController.CreateCommand(Query);
                        foreach (string paramName in ParamDic.Keys)
                        {
                            string colType = ParamDic[paramName]["Type"].ToString();
                            switch (colType)
                            {
                                case SuityouWFWConst.COLUMN_TYPE_STRING:
                                    pgsqlCommand.AddInputParameter(paramName, DbType.String, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_INT:
                                    pgsqlCommand.AddInputParameter(paramName, DbType.Int32, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_DOUBLE:
                                    mssqlCommand.AddInputParameter(paramName, DbType.Double, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_DATETIME:
                                    pgsqlCommand.AddInputParameter(paramName, DbType.DateTime, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_DECIMAL:
                                    mssqlCommand.AddInputParameter(paramName, DbType.Decimal, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_NUMERIC:
                                    mssqlCommand.AddInputParameter(paramName, DbType.Decimal, ParamDic[paramName]["Value"]);
                                    break;
                            }
                        }

                        returnValue = pgsqlController.ExecuteNonQuery(pgsqlCommand);
                        execDateTime = DateTime.Now;

                        // 監査ログの出力
                        if (AuditLogParam != null)
                        {
                            AuditLogParam.OperationDateTime = execDateTime;
                            _auditLogManager.OutputAuditLog(AuditLogParam);
                        }

                        // コミット
                        pgsqlController.Commit();
                    }
                    catch
                    {
                        // ロールバック
                        pgsqlController.Rollback();
                    }
                    finally
                    {
                        pgsqlController.Close();
                    }
                    break;
            }

            return returnValue;
        }
        #endregion

        #region ExecuteNonQueryNoTransaction
        public int ExecuteNonQueryNoTransaction(string Query, Dictionary<string, Dictionary<string, object>> ParamDic, AuditLogInfo? AuditLogParam)
        {
            int returnValue = 0;
            DateTime execDateTime;

            switch (_dbType)
            {
                case SuityouWFWConst.DBTYPE_MSSQL:
                    try
                    {
                        // コマンドの実行
                        mssqlCommand = (MSSQLDatabaseCommand)mssqlController.CreateCommand(Query);
                        foreach (string paramName in ParamDic.Keys)
                        {
                            string colType = ParamDic[paramName]["Type"].ToString();
                            switch (colType)
                            {
                                case SuityouWFWConst.COLUMN_TYPE_STRING:
                                    mssqlCommand.AddInputParameter(paramName, DbType.String, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_INT:
                                    mssqlCommand.AddInputParameter(paramName, DbType.Int32, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_DOUBLE:
                                    mssqlCommand.AddInputParameter(paramName, DbType.Double, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_DATETIME:
                                    mssqlCommand.AddInputParameter(paramName, DbType.DateTime, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_DECIMAL:
                                    mssqlCommand.AddInputParameter(paramName, DbType.Decimal, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_NUMERIC:
                                    mssqlCommand.AddInputParameter(paramName, DbType.Decimal, ParamDic[paramName]["Value"]);
                                    break;
                            }
                        }

                        returnValue = mssqlController.ExecuteNonQuery(mssqlCommand);
                        execDateTime = DateTime.Now;

                        // 監査ログの出力
                        if (AuditLogParam != null)
                        {
                            AuditLogParam.OperationDateTime = execDateTime;
                            _auditLogManager.OutputAuditLog(AuditLogParam);
                        }
                    }
                    catch
                    {
                        throw;
                    }
                    break;
                case SuityouWFWConst.DBTYPE_PGSQL:
                    try
                    {
                        // コマンドの実行
                        pgsqlCommand = (PGSQLDatabaseCommand)pgsqlController.CreateCommand(Query);
                        foreach (string paramName in ParamDic.Keys)
                        {
                            string colType = ParamDic[paramName]["Type"].ToString();
                            switch (colType)
                            {
                                case SuityouWFWConst.COLUMN_TYPE_STRING:
                                    pgsqlCommand.AddInputParameter(paramName, DbType.String, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_INT:
                                    pgsqlCommand.AddInputParameter(paramName, DbType.Int32, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_DOUBLE:
                                    mssqlCommand.AddInputParameter(paramName, DbType.Double, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_DATETIME:
                                    pgsqlCommand.AddInputParameter(paramName, DbType.DateTime, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_DECIMAL:
                                    mssqlCommand.AddInputParameter(paramName, DbType.Decimal, ParamDic[paramName]["Value"]);
                                    break;
                                case SuityouWFWConst.COLUMN_TYPE_NUMERIC:
                                    mssqlCommand.AddInputParameter(paramName, DbType.Decimal, ParamDic[paramName]["Value"]);
                                    break;
                            }
                        }

                        returnValue = pgsqlController.ExecuteNonQuery(pgsqlCommand);
                        execDateTime = DateTime.Now;

                        // 監査ログの出力
                        if (AuditLogParam != null)
                        {
                            AuditLogParam.OperationDateTime = execDateTime;
                            _auditLogManager.OutputAuditLog(AuditLogParam);
                        }
                    }
                    catch
                    {
                        throw;
                    }
                    break;
            }

            return returnValue;
        }
        #endregion

        #endregion

        #region Privateメソッド
        #region MSSQL初期設定
        private void InitializeMSSQLSetting ()
        {
            mssqlController = new MSSQLDatabaseController(_connString);
        }
        #endregion

        #region PGSQL初期設定
        private void InitializePGSQLSetting()
        {
            pgsqlController = new PGSQLDatabaseController(_connString);
        }
        #endregion
        #endregion
    }
}
