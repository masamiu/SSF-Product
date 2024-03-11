namespace BlazorAppNoAuth.Models
{
    public class T_ORGANIZATION
    {
        public string COMPANY_CODE { get; set; }
        public string ORGANIZATION_CODE { get; set;}
        public string ORGANIZATION_NAME { get; set;}
        public int STATUS { get; set;}

        #region KEY_VALUEプロパティ
        public string KEY_VALUE
        {
            get { return string.Format("{0}|{1}", COMPANY_CODE, ORGANIZATION_CODE); }
        }
        #endregion
    }
}
