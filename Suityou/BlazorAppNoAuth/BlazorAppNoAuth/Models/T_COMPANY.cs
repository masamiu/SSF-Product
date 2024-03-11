namespace BlazorAppNoAuth.Models
{
    public class T_COMPANY
    {
        public int COMPANY_ID { get; set; }
        public string COMPANY_CODE { get; set; }
        public string COMPANY_NAME { get; set; }
        public int STATUS { get; set; }

        #region KEY_VALUEプロパティ
        public string KEY_VALUE
        {
            get { return string.Format("{0}", COMPANY_ID); }
        }
        #endregion
    }
}
