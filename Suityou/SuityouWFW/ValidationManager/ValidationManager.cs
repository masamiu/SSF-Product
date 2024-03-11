using Suityou.Framework.Web.Common;
using Suityou.Framework.Web.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Suityou.Framework.Web.ValidationManager
{
    public class ValidationManager
    {
        #region プロパティ
        protected string DataID = string.Empty;
        protected ValidationInformation validInfo;
        #endregion

        #region コンストラクタ
        #region ValidationManager()
        public ValidationManager() { }
        #endregion
        #region ValidationManager(string)
        public ValidationManager(string ValidationDefinitionFolder, string DID)
        {
            DataID = DID;
            validInfo = SettingsManager.GetValidationInformation(ValidationDefinitionFolder, DataID);
        }
        #endregion
        #endregion

        #region メソッド
        #region publicメソッド
        #region GetAllValidationInformation : 全バリデーション情報取得
        public ValidationInformation GetAllValidationInformation()
        {
            return validInfo;
        }
        #endregion

        #region GetValidationInformation(string) : カラムのバリデーション情報取得
        public List<Validation> GetValidationInformation (string ColumnName)
        {
            List<Validation> retValidList = new List<Validation>();

            if (validInfo.Columns != null)
            {
                foreach (ColumnValidationDefinition colValidDef in validInfo.Columns)
                {
                    if (colValidDef.ColumnName.Equals(ColumnName))
                    {
                        if (colValidDef.Validations != null)
                        {
                            foreach (Validation valid in colValidDef.Validations)
                            {
                                retValidList.Add(valid);
                            }
                        }
                    }
                }
            }

            return retValidList;
        }
        #endregion

        #region ExecuteValidation : バリデーション
        public List<string> ExecuteValidation (List<Validation> ValidList, FormItem Item)
        {
            List<string> retErrorMsgList = new List<string>();

            foreach (Validation validProc in ValidList)
            {
                string errMsg = string.Empty;
                switch (validProc.Type)
                {
                    case SuityouWFWConst.VALIDATION_TYPE_REQUIRED:
                        // 必須チェック
                        errMsg = CheckRequired(Item);
                        break;
                    case SuityouWFWConst.VALIDATION_TYPE_ISINTEGER:
                        // 整数チェック
                        errMsg = CheckInteger(Item);
                        break;
                    case SuityouWFWConst.VALIDATION_TYPE_ISDECIMAL:
                        // 小数チェック
                        errMsg = CheckDecimal(Item);
                        break;
                    case SuityouWFWConst.VALIDATION_TYPE_STRINGLENGTH:
                        // 長さチェック
                        errMsg = CheckStringLength(Item, validProc);
                        break;
                    case SuityouWFWConst.VALIDATION_TYPE_SELECTVALUE:
                        // 選択値チェック
                        errMsg = CheckSelectValue(Item, validProc);
                        break;
                }

                if (!errMsg.Equals(string.Empty))
                {
                    retErrorMsgList.Add(errMsg);
                }
            }

            return retErrorMsgList;
        }
        #endregion
        #endregion

        #region privateメソッド
        #region 必須チェック
        private string CheckRequired(FormItem Item)
        {
            string retMsg = string.Empty;

            if (string.IsNullOrWhiteSpace(Item.Value.ToString()))
            {
                retMsg = string.Format("{0} is required.", Item.Name);
            }

            return retMsg;
        }
        #endregion
        #region 整数チェック
        private string CheckInteger(FormItem Item)
        {
            string retMsg = string.Empty;

            try
            {
                int.Parse(Item.Value.ToString());
            }
            catch
            {
                retMsg = string.Format("{0} is enterd by numeric value.", Item.Name);
            }

            return retMsg;
        }
        #endregion
        #region 小数チェック
        private string CheckDecimal(FormItem Item)
        {
            string retMsg = string.Empty;

            try
            {
                double.Parse(Item.Value.ToString());
            }
            catch
            {
                retMsg = string.Format("{0} is enterd by float value.", Item.Name);
            }

            return retMsg;
        }
        #endregion
        #region 長さチェック
        private string CheckStringLength(FormItem Item, Validation ValidDef)
        {
            string retMsg = string.Empty;
            int? checkLength = ValidDef.Length;

            if (checkLength != null)
            {
                if (Item.Value.ToString().Length > checkLength)
                {
                    retMsg = string.Format("{0} is required up to {1} digit.", Item.Name, ValidDef.Length.ToString());
                }
            }

            return retMsg;
        }
        #endregion
        #region 選択値チェック
        private string CheckSelectValue(FormItem Item, Validation ValidDef)
        {
            string retMsg = string.Empty;

            if (ValidDef.Values != null)
            {
                List<string> valueList = new List<string>(ValidDef.Values.Split(','));
                if (valueList.Count > 0)
                {
                    if (!valueList.Contains(Item.Value.ToString()))
                    {
                        retMsg = string.Format("{0} is entered from following values.\r\n{1}", Item.Name, ValidDef.Values);
                    }
                }
            }

            return retMsg;
        }
        #endregion
        #endregion

        #endregion
    }
}
