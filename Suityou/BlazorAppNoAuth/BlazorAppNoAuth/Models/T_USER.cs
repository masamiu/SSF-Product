namespace BlazorAppNoAuth.Models
{
    public class T_USER
    {
        public string USER_ID { get; set; }
        public string FIRST_NAME_EN { get; set; }
        public string LAST_NAME_EN { get; set; }
        public string FIRST_NAME_LCL { get; set; }
        public string LAST_NAME_LCL { get; set; }
        public string COMPANY_CODE { get; set; }
        public string ORGANIZATION_CODE { get; set; }
        public int RETIRE_FLAG { get; set; }

        #region KEY_VALUEプロパティ
        public string KEY_VALUE
        {
            get { return string.Format("{0}", USER_ID); }
        }
        #endregion
    }
}
