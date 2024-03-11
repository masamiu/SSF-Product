using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Npgsql;

namespace SSW.Service
{
    public class PGSQLDatabaseController : IDatabaseController, IDisposable
    {

        private const int DEFAULT_DB_TIMEOUT = 300;
        public NpgsqlConnection Connection { get; set; }
        public NpgsqlTransaction Transaction { get; set; }
        public NpgsqlFactory Factory { get; private set; }
        public int DbTimeOut { get; set; }

        #region コンストラクタ
        /// <summary>
        /// クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="connect">接続文字列</param>
        public PGSQLDatabaseController(string connect)
        {
            NpgsqlConnection cn = new NpgsqlConnection();
            cn.ConnectionString = connect;

            this.Connection = cn;
            this.Transaction = null;
            this.DbTimeOut = DEFAULT_DB_TIMEOUT;
        }

        /// <summary>
        /// クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="connect">接続文字列</param>
        public PGSQLDatabaseController(string connect, int timeout)
        {
            NpgsqlConnection cn = new NpgsqlConnection();
            cn.ConnectionString = connect;

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
            PGSQLDatabaseCommand retCmd = new PGSQLDatabaseCommand();
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd = this.Connection.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sqlCommand;
            cmd.CommandTimeout = this.DbTimeOut;
            retCmd.Command = cmd;

            return retCmd;
        }

        /// <summary>
        /// SqlDataAdapterを初期化する。
        /// </summary>
        /// <returns>SqlDataAdapterオブジェクト</returns>
        public NpgsqlDataAdapter CreateAdapter()
        {
            return new NpgsqlDataAdapter();
        }
        
        /// <summary>
        /// DBに接続する。
        /// </summary>
        public void Open()
        {
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
        /// コネクションの状態を取得する。
        /// </summary>
        public ConnectionState ConnectionState
        {
            get { return Connection.State; }
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
        public int ExecuteNonQuery(IDatabaseCommand Command)
        {
            NpgsqlCommand dbCommand = (NpgsqlCommand)Command;
            dbCommand.Connection = this.Connection;

            if (Transaction != null)
            {
                dbCommand.Transaction = Transaction;
            }

            return dbCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// コマンドを実行し、結果セットの1行目1列目を取得する。
        /// </summary>
        /// <param name="dbCommand">コマンド</param>
        /// <returns>結果セットの1行目1列目</returns>
        public object ExecuteScaler(IDatabaseCommand Command)
        {
            NpgsqlCommand dbCommand = (NpgsqlCommand)Command;
            dbCommand.Connection = this.Connection;

            if (Transaction != null)
            {
                dbCommand.Transaction = Transaction;
            }

            return dbCommand.ExecuteScalar();
        }

        /// <summary>
        /// コマンドを実行し、結果セットをDataReaderで取得する。
        /// </summary>
        /// <param name="dbCommand">コマンド</param>
        /// <returns>結果セット</returns>
        public NpgsqlDataReader ExecuteReader(IDatabaseCommand dbCommand)
        {
            return ExecuteReader((PGSQLDatabaseCommand)dbCommand, CommandBehavior.Default);
        }

        /// <summary>
        /// コマンドを実行し、結果セットをDataReaderで取得する。
        /// </summary>
        /// <param name="command">コマンド</param>
        /// <param name="behavior">結果と影響</param>
        /// <returns>結果セット</returns>
        public NpgsqlDataReader ExecuteReader(IDatabaseCommand Command, CommandBehavior behavior)
        {
            NpgsqlCommand dbCommand = (NpgsqlCommand)Command;
            dbCommand.Connection = this.Connection;

            if (Transaction != null)
            {
                dbCommand.Transaction = Transaction;
            }

            return dbCommand.ExecuteReader(behavior);
        }

        /// <summary>
        /// コマンドを実行し、結果セットをDataTableで取得する。
        /// </summary>
        /// <param name="dbCommand">コマンド</param>
        /// <returns>結果セット</returns>
        public DataTable ExecuteTable(IDatabaseCommand dbCommand)
        {
            DataTable table = new DataTable();
            using (NpgsqlDataReader reader = ExecuteReader(dbCommand))
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
        public DataSet ExecuteSet(IDatabaseCommand Command)
        {
            NpgsqlCommand dbCommand = (NpgsqlCommand)Command;
            dbCommand.Connection = this.Connection;

            if (Transaction != null)
            {
                dbCommand.Transaction = Transaction;
            }

            DataSet set = new DataSet();

            using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(dbCommand))
            {
                adapter.SelectCommand = dbCommand;
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

    }
}