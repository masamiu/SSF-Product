using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Suityou.Framework.Web.Common;
using Suityou.Framework.Web.DataManager;
using Suityou.Framework.Web.DataModel;
using System.Data;
using System.Security.Claims;

namespace BlazorAppNoAuth.Pages.WFW
{
    public partial class ListPage
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
        public DataTable dtTargetData;

        private TableDefinition mainTableDef;
        private string targetId;

        private ClaimsPrincipal loginUser;

        #region OnInitialized
        protected override async Task OnInitializedAsync()
        {
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
            DataSet dsData = dm.GetData();
            dtTargetData = dsData.Tables[0];
        }
        #endregion

        #region ボタン押下イベント
        public void NaviButtionClick(string Operation, string Id)
        {
            string uri = string.Empty;
            switch (Operation)
            {
                case SuityouWFWConst.WEB_OPERATION_ADD:
                    uri = string.Format("/new/{0}", dataId);
                    break;
                case SuityouWFWConst.WEB_OPERATION_MOD:
                    uri = string.Format("/edit/{0}/{1}", dataId, Id);
                    break;
                case SuityouWFWConst.WEB_OPERATION_DEL:
                    uri = string.Format("/delete/{0}/{1}", dataId, Id);
                    break;
                case SuityouWFWConst.WEB_OPERATION_DETAIL:
                    uri = string.Format("/detail/{0}/{1}", dataId, Id);
                    break;
            }

            // 画面遷移
            naviMgr.NavigateTo(uri);
        }
        #endregion
    }
}
