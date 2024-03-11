namespace BlazorAppNoAuth.Models
{
    public class T_COLUMN_TEST
    {
        public string COLUMN_ID { get; set; }
        public decimal COLUMN_DECIMAL { get; set; }
        public decimal COLUMN_NUMERIC { get; set; }
        public double COLUMN_FLOAT { get; set; }
        public DateTime COLUMN_CREATE_DATE { get; set; }
        public DateTime COLUMN_UPDATE_DATE { get; set; }
        public DateTime COLUMN_DATETIME { get; set; }

        #region KEY_VALUEプロパティ
        public string KEY_VALUE
        {
            get { return string.Format("{0}", COLUMN_ID); }
        }
        #endregion
    }
}
