using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Suityou.Framework.Web.Common;
using Suityou.Framework.Web.DataManager;
using Suityou.Framework.Web.DataModel;
using Suityou.Framework.Web.PageUtil;
using System.Data;
using System.Security.Claims;

namespace BlazorAppNoAuth.Pages.WFW
{
    public partial class NewPageEx
    {
        [Inject]
        public IConfiguration configuration { get; set; }
        [Inject]
        public NavigationManager naviMgr { get; set; }
        [Inject]
        public AuthenticationStateProvider authenticationStateProvider { get; set; }

        [Parameter]
        public string DataID { get; set; }

        private NormalDataManager dm;
        private string dataId;
        private string dataName;
        private string appSettingFile;
        private string dataDefinitionFolder;
        private string pageDefinitionFolder;

        private DataRow drTargetData;
        private DataSet dsSubTableData;

        private DataInformation dataInfo;
        private TableDefinition mainTableDef;

        private ClaimsPrincipal loginUser;

        // 多言語項目
        private string title = Properties.Resources.TITLE_ENTRY;
        private string buttonBack = Properties.Resources.BUTTON_BACK;

        #region OnInitialized
        protected override async Task OnInitializedAsync()
        {
            // パラメータ取得
            if (DataID != null)
            {
                dataId = DataID;
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

            // 外部参照データの取得
            dsSubTableData = dm.GetAllSubData();
        }
        #endregion

        #region ボタン押下イベント
        public void NaviButtonClick(string Operation, string Id)
        {
            string uri = string.Empty;
            switch (Operation)
            {
                case SuityouWFWConst.WEB_OPERATION_LIST:
                    uri = string.Format("/list2/{0}", dataId);
                    break;
            }

            // 画面遷移
            naviMgr.NavigateTo(uri);
        }
        #endregion
    }
}
