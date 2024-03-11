namespace BlazorAppNoAuth.Models
{
    public class T_PLACE
    {
        public string PLACE_ID { get; set; }
        public string PLACE_NAME { get; set; }
        public int SYSTEM_USE { get; set; }
        public DateTime CREATE_DATE { get; set; }
        public DateTime UPDATE_DATE { get; set; }

        #region KEY_VALUEプロパティ
        public string KEY_VALUE
        {
            get { return string.Format("{0}", PLACE_ID); }
        }
        #endregion
    }
}

