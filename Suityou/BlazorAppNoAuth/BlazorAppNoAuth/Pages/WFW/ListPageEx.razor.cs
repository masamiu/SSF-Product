using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Suityou.Framework.Web.Common;
using Suityou.Framework.Web.DataManager;
using Suityou.Framework.Web.DataModel;
using Suityou.Framework.Web.PageUtil;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Security.Claims;
using System.Globalization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Components.Forms;
using System.Linq;
using Microsoft.JSInterop;
using System.Text;
using Microsoft.Fast.Components.FluentUI;

namespace BlazorAppNoAuth.Pages.WFW
{
    public partial class ListPageEx
    {
        [Inject]
        public IConfiguration configuration { get; set; }
        [Inject]
        public NavigationManager naviMgr { get; set; }
        [Inject]
        public AuthenticationStateProvider authenticationStateProvider { get; set; }
        [Inject]
        public IJSRuntime jsRuntime { get; set; }
        [Inject]
        public IDialogService DialogService { get; set; }

        [Parameter]
        public string DataID { get; set; }

        NormalDataManager dm;
        public string dataId;
        public string dataName;
        public DataInformation dataInfo;
        public string appSettingFile;
        public string dataDefinitionFolder;
        public string pageDefinitionFolder;
        public DataTable dtTargetData;
        private DataSet dsSubTableData;

        // モデル関連
        private Type modelType;
        private object model;
        private List<object> modelList = new List<object>();

        // フォーム関連
        private PageForm searchForm;
        private EditContext editContext;
        private FormItem[] formItemArr;
        private int searchItemCount;
        private int searchFormRowCount;
        private int searchFormColCount;

        private TableDefinition mainTableDef;
        private Dictionary<string, string> colDispNameDic = new Dictionary<string, string>();
        private Dictionary<string, PageControlInfomation> pageControlDic = new Dictionary<string, PageControlInfomation>();

        private ClaimsPrincipal loginUser;

        // アップロードダイアログ連携用
        private UploadData uploadData;

        // 制御用
        private bool isReadOnly;
        private bool useDownload;
        private bool useUpload;
        private bool notUseSearch;

        // 多言語項目
        private string title = Properties.Resources.TITLE_LIST;
        private string titleUpload = Properties.Resources.TITLE_UPLOAD;
        private string buttonEntry = Properties.Resources.BUTTON_ENTRY;
        private string buttonDetail = Properties.Resources.BUTTON_DETAIL;
        private string buttonEdit = Properties.Resources.BUTTON_EDIT;
        private string buttonDelete = Properties.Resources.BUTTON_DELETE;
        private string buttonDownload = Properties.Resources.BUTTON_DOWNLOAD;
        private string buttonUpload = Properties.Resources.BUTTON_UPLOAD;
        private string buttonSearch = Properties.Resources.BUTTON_SEARCH;
        private string operation = Properties.Resources.OPERATION;
        private string filter = Properties.Resources.FILTER;
        private string labelFilterNotset = Properties.Resources.LABEL_FILTER_NOTSET;

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
            dataInfo = dm.GetDataInformation();
            if (dataInfo.IsReadOnly != null && dataInfo.IsReadOnly == true)
            {
                isReadOnly = true;
            }
            else
            {
                isReadOnly = false;
            }
            if (dataInfo.NotUseSearch != null && dataInfo.NotUseSearch == true)
            {
                notUseSearch = true;
            }
            else
            {
                notUseSearch = false;
            }
            if (!isReadOnly)
            {
                if (dataInfo.UseDownload != null && dataInfo.UseDownload == true)
                {
                    useDownload = true;
                }
                else
                {
                    useDownload = false;
                }

                if (dataInfo.UseUpload != null && dataInfo.UseUpload == true)
                {
                    useUpload = true;
                }
                else
                {
                    useUpload = false;
                }
            }
            else
            {
                useDownload = false;
                useUpload = false;
            }

            mainTableDef = dataInfo.MainTable;
            DataSet dsData = dm.GetData();
            dtTargetData = dsData.Tables[0];

            // 外部参照データの取得
            dsSubTableData = dm.GetAllSubData();

            // DataTable => モデルクラスのリストへの変換
            string modelClassName = "BlazorAppNoAuth.Models." + mainTableDef.TableName;
            modelType = Type.GetType(modelClassName);
            modelList = PageModelManager.GetPageModelList(modelType, dtTargetData);

            // モデル
            model = PageModelManager.GetPageModel(modelType, mainTableDef);

            // モデル => PageForm（検索用）
            searchForm = PageModelManager.GetPageFormForSearchByModel(model, modelType, mainTableDef);
            editContext = new EditContext(searchForm);

            formItemArr = searchForm.FormItems.ToArray();

            // ページ制御用情報ディクショナリーの作成
            PageControlManager pm = new PageControlManager(dataInfo, searchForm, dsSubTableData);
            pageControlDic = pm.GeneratePageControlInformation();

            // 項目表示名ディクショナリーの作成
            foreach (ColumnDefinition colDef in mainTableDef.Columns)
            {
                colDispNameDic.Add(colDef.ColumnName, (colDef.ExtAttrs.DisplayName == null) ? colDef.ColumnName : colDef.ExtAttrs.DisplayName);
            }

            // 検索領域設定
            searchItemCount = formItemArr.Length;
            searchFormColCount = 2;
            searchFormRowCount = searchItemCount / searchFormColCount;
            if (searchItemCount % searchFormColCount > 0)
            {
                searchFormRowCount++;
            }
        }
        #endregion

        #region 外部参照キー項目入力イベント
        public void OnInputReferenceKeyValue(ChangeEventArgs e, string ColumnName)
        {
            // 外部参照キー項目の項目名、値を取得
            string _columnName = ColumnName;
            string _inputValue = e.Value.ToString();

            // 当該項目がキー項目となっている外部参照データについて、変更後のキー値で再取得
            foreach (string keyColName in pageControlDic.Keys)
            {
                PageControlInfomation targetColInfo = pageControlDic[keyColName];

                // 親キー項目に含まれる
                if (targetColInfo.ParentKeys != null && targetColInfo.ParentKeys.Exists(x => x.Equals(_columnName)))
                {
                    Dictionary<string, object> colDic = new Dictionary<string, object>();
                    // 親キー項目が当該項目のみまたは当該項目が最後のキー項目の場合
                    if (targetColInfo.ParentKeys.Count == 1 || targetColInfo.ParentKeys.IndexOf(_columnName) == targetColInfo.ParentKeys.Count - 1)
                    {
                        // 外部参照データの絞り込みを行い、結果を設定
                        foreach (FormItem fItem in searchForm.FormItems)
                        {
                            if (targetColInfo.ParentKeys.Exists(x => x.Equals(fItem.Name)))
                            {
                                switch (targetColInfo.ColumnType)
                                {
                                    case SuityouWFWConst.COLUMN_TYPE_STRING:
                                        if (string.IsNullOrEmpty(_inputValue))
                                        {
                                            fItem.Value = string.Empty;
                                            colDic.Add(string.Format("{0}@{1}", _columnName, SuityouWFWConst.COLUMN_TYPE_STRING), string.Empty);
                                        }
                                        else
                                        {
                                            fItem.Value = _inputValue;
                                            colDic.Add(string.Format("{0}@{1}", _columnName, SuityouWFWConst.COLUMN_TYPE_STRING), _inputValue);
                                        }
                                        break;
                                    case SuityouWFWConst.COLUMN_TYPE_INT:
                                        if (string.IsNullOrEmpty(_inputValue))
                                        {
                                            fItem.Value = null;
                                            colDic.Add(string.Format("{0}@{1}", _columnName, SuityouWFWConst.COLUMN_TYPE_INT), null);
                                        }
                                        else
                                        {
                                            fItem.Value = int.Parse(_inputValue);
                                            colDic.Add(string.Format("{0}@{1}", _columnName, SuityouWFWConst.COLUMN_TYPE_INT), int.Parse(_inputValue));
                                        }
                                        break;
                                    case SuityouWFWConst.COLUMN_TYPE_BOOL:
                                        if (string.IsNullOrEmpty(_inputValue))
                                        {
                                            fItem.Value = null;
                                            colDic.Add(string.Format("{0}@{1}", _columnName, SuityouWFWConst.COLUMN_TYPE_BOOL), null);
                                        }
                                        else
                                        {
                                            fItem.Value = bool.Parse(_inputValue);
                                            colDic.Add(string.Format("{0}@{1}", _columnName, SuityouWFWConst.COLUMN_TYPE_BOOL), bool.Parse(_inputValue));
                                        }
                                        break;
                                    case SuityouWFWConst.COLUMN_TYPE_DATETIME:
                                        if ((DateTime)fItem.Value == null)
                                        {
                                            fItem.Value = null;
                                            colDic.Add(string.Format("{0}@{1}", _columnName, SuityouWFWConst.COLUMN_TYPE_DATETIME), null);
                                        }
                                        else
                                        {
                                            fItem.Value = DateTime.Parse(_inputValue);
                                            colDic.Add(string.Format("{0}@{1}", _columnName, SuityouWFWConst.COLUMN_TYPE_DATETIME), DateTime.Parse(_inputValue));
                                        }
                                        break;
                                }

                                DataTable dtSubTable = dsSubTableData.Tables[targetColInfo.ReferenceTable];
                                var query = dtSubTable.AsEnumerable().Where(CommonUtil.CreateExpressionTreeWhereAnd(colDic));
                                if (query.Any())
                                {
                                    targetColInfo.ReferenceData = query.CopyToDataTable();
                                }
                                else
                                {
                                    targetColInfo.ReferenceData = new DataTable();
                                }
                            }
                        }
                    }
                    // それ以外
                    else
                    {
                        // 外部参照データは空に設定
                        targetColInfo.ReferenceData = new DataTable();
                    }
                }
            }
        }
        #endregion
        #region 検索ボタン押下イベント
        public async Task SearchButtonClickAsync()
        {
            // カラム型Dicを作成
            Dictionary<string, string> colTypeDic = new Dictionary<string, string>();
            foreach (ColumnDefinition colDef in mainTableDef.Columns)
            {
                colTypeDic.Add(colDef.ColumnName, colDef.ColumnType);
            }
            // EditContextからPageFormを再取得
            searchForm = (PageForm)editContext.Model;
            // PageForm => フィルタDictionary
            Dictionary<string, object> filterDic = new Dictionary<string, object>();
            filterDic = PageModelManager.GetFilterDicFromPageForm(colTypeDic, searchForm);

            // 検索実行
            DataSet dsData = dm.GetData(filterDic);
            // 検索結果をモデルリストに変換
            dtTargetData = dsData.Tables[0];
            modelList = PageModelManager.GetPageModelList(modelType, dtTargetData);
        }
        #endregion
        #region ダウンロードボタン押下イベント
        public async Task DownloadButtonClickAsync()
        {
            Dictionary<string, object> filterDic = new Dictionary<string, object>();

            // カラム型Dicを作成
            Dictionary<string, string> colTypeDic = new Dictionary<string, string>();
            foreach (ColumnDefinition colDef in mainTableDef.Columns)
            {
                colTypeDic.Add(colDef.ColumnName, colDef.ColumnType);
            }

            if (!notUseSearch)
            {
                // EditContextからPageFormを再取得
                searchForm = (PageForm)editContext.Model;
                // PageForm => フィルタDictionary
                filterDic = PageModelManager.GetFilterDicFromPageForm(colTypeDic, searchForm);
            }
            else
            {
                // 検索未使用時はフィルタDictionaryは未設定
            }

            // 検索実行
            DataSet dsData = dm.GetData(filterDic);
            // ダウンロードファイル名
            string dlFileName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".csv";

            // 検索結果をCSV形式に変換後、byteデータに変換
            byte[] byteDatas = CommonUtil.GenerateDownloadCSVDataFromDataTable(dsData.Tables[0]);

            await jsRuntime.InvokeVoidAsync("saveAsFile", dlFileName, Convert.ToBase64String(byteDatas));
        }
        #endregion
        #region アップロードボタン押下イベント
        public async Task UploadButtonClickAsync()
        {
            // ダイアログ表示準備
            DialogParameters parameters = new()
            {
                Title = titleUpload,
                PrimaryAction = "Yes",
                PrimaryActionEnabled = false,
                SecondaryAction = "No",
                Width = "70%",
                TrapFocus = true,
                Modal = false,
                PreventScroll = true
            };

            uploadData = new UploadData(dm);
            IDialogReference dialog = await DialogService.ShowDialogAsync<UploadDialog>(uploadData, parameters);
            DialogResult? result = await dialog.Result;
        }
        #endregion
        #region 遷移ボタン押下イベント
        public void NaviButtonClick(string Operation, string Id)
        {
            string uri = string.Empty;
            switch (Operation)
            {
                case SuityouWFWConst.WEB_OPERATION_ADD:
                    uri = string.Format("/new2/{0}", dataId);
                    break;
                case SuityouWFWConst.WEB_OPERATION_MOD:
                    uri = string.Format("/edit2/{0}/{1}/{2}", dataId, Id, SuityouWFWConst.FROM_TYPE_LIST);
                    break;
                case SuityouWFWConst.WEB_OPERATION_DEL:
                    uri = string.Format("/delete2/{0}/{1}/{2}", dataId, Id, SuityouWFWConst.FROM_TYPE_LIST);
                    break;
                case SuityouWFWConst.WEB_OPERATION_DETAIL:
                    uri = string.Format("/detail2/{0}/{1}", dataId, Id);
                    break;
            }

            // 画面遷移
            naviMgr.NavigateTo(uri);
        }
        #endregion
    }
}
