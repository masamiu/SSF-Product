using System;
using System.Data;
using System.Data.Common;

namespace SSW.Service
{
    public class MSSQLDatabaseCommand : IDatabaseCommand, IDisposable
    {
        #region 定数
        private const string RETURN_VALUE = "@RETURN_VALUE";
        #endregion

        #region プロパティ
        public DbCommand Command { get; set; }

        /// <summary>
        /// コマンドタイムアウト時間を取得または設定する
        /// </summary>
        public int CommandTimeOut
        {
            get
            {
                return Command.CommandTimeout;
            }
            set
            {
                Command.CommandTimeout = value;
            }
        }
        #endregion

        #region Publicメソッド
        /// <summary>
        /// SQL文に渡すインプット用パラメータを作成する。
        /// </summary>
        /// <param name="parameterName">パラメータ名</param>
        /// <param name="dbType">パラメータの型</param>
        /// <param name="value">パラメータの値</param>
        public void AddInputParameter(string parameterName, DbType dbType, object value)
        {
            DbParameter parameter = Command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.DbType = dbType;

            if (value == null || (value != null && string.IsNullOrEmpty(value.ToString())))
            {
                parameter.Value = DBNull.Value;
            }
            else
            {
                parameter.Value = value;
            }

            parameter.Direction = ParameterDirection.Input;
            Command.Parameters.Add(parameter);
        }

        /// <summary>
        /// SQL文に渡すアウトプット用パラメータを作成する。
        /// </summary>
        /// <param name="parameterName">パラメータ名</param>
        /// <param name="dbType">パラメータの型</param>
        public void AddOutputParameter(string parameterName, DbType dbType)
        {
            DbParameter parameter = Command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.DbType = dbType;
            parameter.Size = 255;
            parameter.Direction = ParameterDirection.Output;

            Command.Parameters.Add(parameter);
        }

        /// <summary>
        /// SQL文に渡す戻り値用パラメータを作成する。
        /// </summary>
        public void AddReturnValueParameter()
        {
            DbParameter parameter = Command.CreateParameter();
            parameter.ParameterName = RETURN_VALUE;
            parameter.DbType = DbType.Int32;
            parameter.Direction = ParameterDirection.ReturnValue;

            Command.Parameters.Add(parameter);
        }

        /// <summary>
        /// コマンドオブジェクトのリソースを開放する
        /// </summary>
        public void Dispose()
        {
            if (Command != null)
            {
                Command.Dispose();
                Command = null;
            }
        }

        #endregion

    }
}