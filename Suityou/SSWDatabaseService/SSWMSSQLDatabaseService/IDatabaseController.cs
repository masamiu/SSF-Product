using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSW.Service
{
    public interface IDatabaseController
    {
        #region プロパティ
        public DbConnection Connection { get; set; }
        public DbTransaction Transaction { get; set; }
        public ConnectionState ConnectionState { get; }
        #endregion

        #region メソッド
        #region 接続関連
        public void Open();
        public void Close();

        #endregion
        #region トランザクション関連
        public void BeginTransaction();
        public void BeginTransaction(IsolationLevel isolationLevel);
        public void Commit();
        public void Rollback();
        #endregion
        #region コマンド作成
        public IDatabaseCommand CreateCommand(string sqlCommand);
        public IDatabaseCommand CreateSPCommand(string spName);
        #endregion
        #region コマンド実行
        public int ExecuteNonQuery(IDatabaseCommand dbCommand);
        public object ExecuteScaler(IDatabaseCommand dbCommand);
        public DbDataReader ExecuteReader(IDatabaseCommand dbCommand);
        public DbDataReader ExecuteReader(IDatabaseCommand dbCommand, CommandBehavior behavior);
        public DataTable ExecuteTable(IDatabaseCommand dbCommand);
        public DataSet ExecuteSet(IDatabaseCommand dbCommand);
        #endregion
        #endregion
    }
}
