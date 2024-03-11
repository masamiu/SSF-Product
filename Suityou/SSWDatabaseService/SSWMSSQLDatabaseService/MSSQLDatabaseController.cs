using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace SSW.Service
{
    public class MSSQLDatabaseController : IDatabaseController, IDisposable
    {
        #region 定数
        private const string PROVIDER = "System.Data.SqlClient";
        private const int DEFAULT_DB_TIMEOUT = 300;
        #endregion

        #region プロパティ
        // 接続文字列
        public string ConnectionString { get; set; }
        // DB接続
        public DbConnection Connection { get; set; }
        // トランザクション
        public DbTransaction Transaction { get; set; }
        public DbProviderFactory Factory { get; private set; }
        // タイムアウト値
        public int DbTimeOut { get; private set; }

        // コネクションの状態
        public ConnectionState ConnectionState
        {
            get { return Connection.State; }
        }
        #endregion

        #region コンストラクタ
        /// <summary>
        /// クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="connect">接続文字列</param>
        public MSSQLDatabaseController(string connectString)
        {
            string providerName = PROVIDER;

            DbProviderFactories.RegisterFactory(providerName, System.Data.SqlClient.SqlClientFactory.Instance);
            DbProviderFactory factory = DbProviderFactories.GetFactory(providerName);
            DbConnection cn = factory.CreateConnection();
            cn.ConnectionString = connectString;

            this.ConnectionString = connectString;
            this.Factory = factory;
            this.Connection = cn;
            this.Transaction = null;
            this.DbTimeOut = DEFAULT_DB_TIMEOUT;
        }

        /// <summary>
        /// クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="connect">接続文字列</param>
        public MSSQLDatabaseController(string connectString, int timeout)
        {
            string providerName = PROVIDER;

            DbProviderFactories.RegisterFactory(providerName, System.Data.SqlClient.SqlClientFactory.Instance);
            DbProviderFactory factory = DbProviderFactories.GetFactory(providerName);
            DbConnection cn = factory.CreateConnection();
            cn.ConnectionString = connectString;

            this.ConnectionString = connectString;
            this.Factory = factory;
            this.Connection = cn;
            this.Transaction = null;
            this.DbTimeOut = timeout;
        }
        #endregion

        #region Publicメソッド
       /// <summary>
        /// 現在の接続に関連付けられたコマンドを作成する。
        /// [SQLテキストコマンド用]
        /// </summary>
        /// <param name="sqlCommand">テキストコマンド</param>
        /// <returns>DatabaseCommandオブジェクト</returns>
        public IDatabaseCommand CreateCommand(string sqlCommand)
        {
            DbCommand cmd = new SqlCommand();
            cmd = this.Connection.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sqlCommand;

            MSSQLDatabaseCommand dCmd = new MSSQLDatabaseCommand();
            dCmd.Command = cmd;
            dCmd.CommandTimeOut = this.DbTimeOut;
            return dCmd;
        }

        /// <summary>
        /// 現在の接続に関連付けられたコマンドを作成する。
        /// [ストアドプロシージャ用]
        /// </summary>
        /// <param name="spName">実行SP名</param>
        /// <returns>DatabaseCommandオブジェクト</returns>
        public IDatabaseCommand CreateSPCommand(string spName)
        {
            DbCommand cmd = new SqlCommand();
            cmd = this.Connection.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = spName;

            MSSQLDatabaseCommand dCmd = new MSSQLDatabaseCommand();
            dCmd.Command = cmd;
            dCmd.CommandTimeOut = this.DbTimeOut;
            return dCmd;
        }

        /// <summary>
        /// SqlDataAdapterを初期化する。
        /// </summary>
        /// <returns>SqlDataAdapterオブジェクト</returns>
        public SqlDataAdapter CreateAdapter()
        {
            return new SqlDataAdapter();
        }

        /// <summary>
        /// DBに接続する。
        /// </summary>
        public void Open()
        {
            // 接続を作成する(既にあれば作成しない)
            CreateConnection();
            // DB接続オープン(既にオープン済みであれば実施しない)
            if (Connection.State == ConnectionState.Open) return;
            Connection.Open();
        }

        /// <summary>
        /// トランザクションを開始する。
        /// </summary>
        public void BeginTransaction()
        {
            Transaction = Connection.BeginTransaction();
        }

        /// <summary>
        /// トランザクションを開始する。
        /// </summary>
        /// <param name="isolationLevel">トランザクションの分離レベル</param>
        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            Transaction = Connection.BeginTransaction(isolationLevel);
        }

        /// <summary>
        /// コミットする。
        /// </summary>
        public void Commit()
        {
            if (Transaction != null)
            {
                Transaction.Commit();
            }
            Transaction = null;
        }

        /// <summary>
        /// ロールバックする。
        /// </summary>
        public void Rollback()
        {
            if (Transaction != null)
            {
                Transaction.Rollback();
            }
            Transaction = null;
        }

        /// <summary>
        /// DBから切断する。
        /// </summary>
        public void Close()
        {
            if (Connection != null)
            {
                Connection.Close();
                Connection.Dispose();
                Connection = null;
            }
        }

        /// <summary>
        /// DBから切断する。
        /// </summary>
        public void Dispose()
        {
            Close();
        }

        /// <summary>
        /// コマンドを実行し、影響を与えた行数を取得する。
        /// </summary>
        /// <param name="dbCommand">コマンド</param>
        /// <returns>影響を与えた行数</returns>
        public int ExecuteNonQuery(IDatabaseCommand dbCommand)
        {
            DbCommand command = dbCommand.Command;

            if (Transaction != null)
            {
                command.Transaction = Transaction;
            }

            return command.ExecuteNonQuery();
        }

        /// <summary>
        /// コマンドを実行し、結果セットの1行目1列目を取得する。
        /// </summary>
        /// <param name="dbCommand">コマンド</param>
        /// <returns>結果セットの1行目1列目</returns>
        public object ExecuteScaler(IDatabaseCommand dbCommand)
        {
            DbCommand command = dbCommand.Command;

            if (Transaction != null)
            {
                command.Transaction = Transaction;
            }

            return command.ExecuteScalar();
        }

        /// <summary>
        /// コマンドを実行し、結果セットをDataReaderで取得する。
        /// </summary>
        /// <param name="dbCommand">コマンド</param>
        /// <returns>結果セット</returns>
        public DbDataReader ExecuteReader(IDatabaseCommand dbCommand)
        {
            return ExecuteReader((MSSQLDatabaseCommand)dbCommand, CommandBehavior.Default);
        }

        /// <summary>
        /// コマンドを実行し、結果セットをDataReaderで取得する。
        /// </summary>
        /// <param name="command">コマンド</param>
        /// <param name="behavior">結果と影響</param>
        /// <returns>結果セット</returns>
        public DbDataReader ExecuteReader(IDatabaseCommand dbCommand, CommandBehavior behavior)
        {
            DbCommand command = dbCommand.Command;

            if (Transaction != null)
            {
                command.Transaction = Transaction;
            }

            return command.ExecuteReader(behavior);
        }

        /// <summary>
        /// コマンドを実行し、結果セットをDataTableで取得する。
        /// </summary>
        /// <param name="dbCommand">コマンド</param>
        /// <returns>結果セット</returns>
        public DataTable ExecuteTable(IDatabaseCommand dbCommand)
        {
            DataTable table = new DataTable();
            using (DbDataReader reader = ExecuteReader(dbCommand))
            {
                table.Load(reader);
            }

            return table;
        }

        /// <summary>
        /// コマンドを実行し、結果セットをDataSetで取得する。
        /// </summary>
        /// <param name="dbCommand">コマンド</param>
        /// <returns>結果セット</returns>
        public DataSet ExecuteSet(IDatabaseCommand dbCommand)
        {
            DbCommand command = dbCommand.Command;

            if (Transaction != null)
            {
                command.Transaction = Transaction;
            }

            DataSet set = new DataSet();

            using (DbDataAdapter adapter = Factory.CreateDataAdapter())
            {
                adapter.SelectCommand = command;
                adapter.Fill(set);
            }

            return set;
        }

        #endregion

        #region Staticメソッド

        /// <summary>
        /// 検索文言自体にワイルドカードが含まれている場合、エスケープします。
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public static string EscapeForPartialMatch(string searchText)
        {
            // 検索対象文字列が空の場合は空文字を返却
            if (String.IsNullOrEmpty(searchText)) return string.Empty;
            // ワイルドカード文字列を除去した文字列を返却
            return Regex.Replace(searchText, @"[%_\[]", "[$0]");
        }

        /// <summary>
        /// 検索文言にワイルドカードを付与します。
        /// </summary>
        /// <param name="searchText">検索文言</param>
        /// <returns>ワイルドカードを付与した検索文言</returns>
        private static string AppendWildcard(string searchText)
        {
            // 検索対象文字列に指定のワイルドカード文字列を付加した文字列を返却
            return string.Format("%{0}%", searchText);
        }

        #endregion

        #region Privateメソッド
        #region CreateConnection
        private void CreateConnection ()
        {
            // Connectionが存在しない場合は作成する
            if (this.Connection == null)
            {
                DbConnection cn = this.Factory.CreateConnection();
                cn.ConnectionString = this.ConnectionString;

                this.Connection = cn;
            }
        }
        #endregion
        #endregion
    }
}