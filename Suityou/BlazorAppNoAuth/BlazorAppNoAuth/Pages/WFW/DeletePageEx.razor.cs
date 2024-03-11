using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Suityou.Framework.Web.Common;
using Suityou.Framework.Web.DataManager;
using Suityou.Framework.Web.DataModel;
using Suityou.Framework.Web.PageUtil;
using System.Data;
using System.Reflection;
using System.Security.Claims;

namespace BlazorAppNoAuth.Pages.WFW
{
    public partial class DeletePageEx
    {
        [Inject]
        public IConfiguration configuration { get; set; }
        [Inject]
        public NavigationManager naviMgr { get; set; }
        [Inject]
        public AuthenticationStateProvider authenticationStateProvider { get; set; }

        [Parameter]
        public string DataID { get; set; }
        [Parameter]
        public string DataKey { get; set; }
        [Parameter]
        public string FromType { get; set; }

        private NormalDataManager dm;
        private string dataId;
        private string datakey;
        private string fromType;

        private string dataName;
        private string appSettingFile;
        private string dataDefinitionFolder;
        private string pageDefinitionFolder;
        private DataRow drTargetData;
        private DataSet dsSubTableData;

        private DataInformation dataInfo;
        private TableDefinition mainTableDef;

        private Type modelType;
        private object model;

        private ClaimsPrincipal loginUser;

        // 多言語項目
        private string title = Properties.Resources.TITLE_DELETE;
        private string buttonDelete = Properties.Resources.BUTTON_DELETE;
        private string buttonBack = Properties.Resources.BUTTON_BACK;

        #region OnInitialized
        protected override async Task OnInitializedAsync()
        {
            // パラメータ取得
            if (DataID != null)
            {
                dataId = DataID;
            }
            if (DataKey != null)
            {
                datakey = DataKey;
            }
            if (FromType != null)
            {
                fromType = FromType;
            }

            var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
            loginUser = authState?.User;

            appSettingFile = configuration["CustomSetting:AppSettingFile"];
            dataDefinitionFolder = configuration["CustomSetting:DataDefinitionFolderPath"];
            pageDefinitionFolder = configuration["CustomSetting:PageDefinitionFolderPath"];

            dm = new NormalDataManager(appSettingFile, dataDefinitionFolder, dataId, loginUser);

            dataName = dm.DataName;
            dataInfo = dm.GetDataInformation();
            mainTableDef = dataInfo.MainTable;

            Dictionary<string, object> filterDic = new Dictionary<string, object>();
            string[] keyArr = datakey.Split("|");
            int keySeq = 0;
            foreach (ColumnDefinition colDef in mainTableDef.Columns)
            {
                if (colDef.IsPrimaryKey)
                {
                    filterDic.Add(colDef.ColumnName, keyArr[keySeq]);
                    keySeq++;
                }
            }
            DataSet dsData = dm.GetData(filterDic);
            drTargetData = dsData.Tables[0].Rows[0];

            // DataTable => モデルクラスへの変換
            string modelClassName = "BlazorAppNoAuth.Models." + mainTableDef.TableName;
            modelType = Type.GetType(modelClassName);
            model = PageModelManager.GetPageModel(modelType, drTargetData);

            // 外部参照データの取得
            dsSubTableData = dm.GetAllSubData();
        }
        #endregion

        #region 遷移ボタン押下イベント
        public void NaviButtonClick(string Operation, string Id)
        {
            string uri = string.Empty;
            switch (Operation)
            {
                case SuityouWFWConst.WEB_OPERATION_BACK:
                    if (fromType.Equals(SuityouWFWConst.FROM_TYPE_LIST))
                    {
                        uri = string.Format("/list2/{0}", dataId);
                    }
                    else if (fromType.Equals(SuityouWFWConst.FROM_TYPE_DETAIL))
                    {
                        uri = string.Format("/detail2/{0}/{1}", dataId, datakey);
                    }
                    break;
            }

            // 画面遷移
            naviMgr.NavigateTo(uri);
        }
        #endregion

        #region 削除ボタン押下イベント
        public void DeleteButtonClick ()
        {
            // 削除処理
            dm = new NormalDataManager(appSettingFile, dataDefinitionFolder, dataId, loginUser);
            dm.DeleteData(drTargetData);

            // 画面遷移
            string uri = string.Format("/list2/{0}", dataId);
            naviMgr.NavigateTo(uri);
        }
        #endregion
    }
}
