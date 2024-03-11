using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Suityou.Framework.Web.Common;
using Suityou.Framework.Web.DataManager;
using Suityou.Framework.Web.DataModel;
using System.Data;
using System.Security.Claims;

namespace BlazorAppNoAuth.Pages.WFW
{
    public partial class NewPage
    {
        [Inject]
        public IConfiguration configuration { get; set; }
        [Inject]
        public NavigationManager naviMgr { get; set; }
        [Inject]
        public AuthenticationStateProvider authenticationStateProvider { get; set; }

        [Parameter]
        public string DataID { get; set; }

        NormalDataManager dm;
        public string dataId;
        public string dataName;
        public string appSettingFile;
        public string dataDefinitionFolder;
        public string pageDefinitionFolder;
        public DataRow drTargetData;

        private TableDefinition mainTableDef;

        private ClaimsPrincipal loginUser;

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
            mainTableDef = dm.GetDataInformation().MainTable;
        }
        #endregion

        #region ボタン押下イベント
        public void NaviButtionClick(string Operation, string Id)
        {
            string uri = string.Empty;
            switch (Operation)
            {
                case SuityouWFWConst.WEB_OPERATION_LIST:
                    uri = string.Format("/list/{0}", dataId);
                    break;
            }

            // 画面遷移
            naviMgr.NavigateTo(uri);
        }
        #endregion

        #region サブミットイベント
        public void SubmitForm()
        {

        }
        #endregion
    }
}
