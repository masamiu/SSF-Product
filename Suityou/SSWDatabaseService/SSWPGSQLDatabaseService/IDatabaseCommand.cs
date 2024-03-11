using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSW.Service
{
    public interface IDatabaseCommand
    {
        #region 変数
        //public NpgsqlConnection Connection
        //{
        //    get;
        //    set;
        //}
        public NpgsqlCommand Command
        {
            get;
            set;
        }
        //public NpgsqlTransaction Transaction
        //{
        //    get;
        //    set;
        //}
        #endregion

        #region メソッド
        public void AddInputParameter(string parameterName, DbType dbType, object value);
        public void AddOutputParameter(string parameterName, DbType dbType);
        public void AddReturnValueParameter();
        #endregion
    }
}
