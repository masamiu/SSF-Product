using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.SqlServer.Server;
using Suityou.Framework.Web.Common;
using Suityou.Framework.Web.DataManager;
using Suityou.Framework.Web.DataModel;
using Suityou.Framework.Web.PageUtil;
using Suityou.Framework.Web.ValidationManager;
using System;
using System.Data;
using System.Reflection;
using System.Security.Claims;

namespace BlazorAppNoAuth.Shared.WFW
{
    public partial class EditableFormEx
    {
        [Inject]
        public IConfiguration configuration { get; set; }
        [Inject]
        public NavigationManager naviMgr { get; set; }
        [Inject]
        public AuthenticationStateProvider authenticationStateProvider { get; set; }

        [Parameter]
        public DataInformation? DataInfo { get; set; }
        [Parameter]
        public string? DataKey { get; set; }
        [Parameter]
        public DataRow? DrTarget { get; set; }
        [Parameter]
        public DataSet? DsSubTable { get; set; }
        [Parameter]
        public ClaimsPrincipal? LoginUser { get; set; }

        private NormalDataManager dm;
        private DataInformation dataInfo;
        private string dataId;
        private string datakey;
        private string appSettingFile;
        private string dataDefinitionFolder;
        private string validationDefinitionFolder;

        private DataRow drTargetData;
        private DataSet dsSubTable;

        private ValidationManager vm;

        private TableDefinition mainTableDef;

        private ClaimsPrincipal loginUser;

        // モデル関連
        private Type modelType;
        private object model;

        // フォーム関連
        private PageForm pageForm;
        private EditContext editContext;
        private ValidationMessageStore validationMessageStore;

        // 制御用
        private bool isNew = true;
        Dictionary<string, PageControlInfomation> pageControlDic;

        // 多言語項目
        private string buttonUpdate = Properties.Resources.BUTTON_UPDATE;

        #region イベント
        #region 初期化イベント
        protected override async Task OnInitializedAsync()
        {
            // パラメータ取得
            if (DataInfo != null)
            {
                dataInfo = DataInfo;
            }
            if (DataKey != null)
            {
                datakey = DataKey;
                isNew = false;
            }
            if (DrTarget != null)
            {
                drTargetData = DrTarget;
            }
            if (DsSubTable != null)
            {
                dsSubTable = DsSubTable;
            }
            if (LoginUser != null)
            {
                loginUser = LoginUser;
            }

            appSettingFile = configuration["CustomSetting:AppSettingFile"];
            dataDefinitionFolder = configuration["CustomSetting:DataDefinitionFolderPath"];
            validationDefinitionFolder = configuration["CustomSetting:ValidationDefinitionFolderPath"];

            dataId = dataInfo.DataID;
            mainTableDef = dataInfo.MainTable;

            // バリデーション定義の取得用
            vm = new ValidationManager(validationDefinitionFolder, dataId);

            // DataRow => モデルクラスへの変換
            string modelClassName = "BlazorAppNoAuth.Models." + mainTableDef.TableName;
            modelType = Type.GetType(modelClassName);
            if (isNew)
            {
                model = PageModelManager.GetPageModel(modelType, mainTableDef);
            }
            else
            {
                model = PageModelManager.GetPageModel(modelType, drTargetData);
            }

            // モデル => PageForm
            pageForm = PageModelManager.GetPageFormByModel(model, modelType, mainTableDef);

            editContext = new EditContext(pageForm);
            validationMessageStore = new ValidationMessageStore(editContext);
            editContext.OnValidationRequested += ValidationRequested;

            // ページ制御用情報ディクショナリーの作成
            PageControlManager pm = new PageControlManager(dataInfo, pageForm, dsSubTable);
            pageControlDic = pm.GeneratePageControlInformation();
        }
        #endregion
        #region サブミットイベント
        public void SubmitForm()
        {
            // カラム型Dicを作成
            Dictionary<string, string> colTypeDic = new Dictionary<string, string>();
            foreach (ColumnDefinition colDef in mainTableDef.Columns)
            {
                colTypeDic.Add(colDef.ColumnName, colDef.ColumnType);
            }
            // EditContextからPageFormを再取得
            pageForm = (PageForm)editContext.Model;
            // PageForm => モデル
            model = PageModelManager.GetModelByPageForm(colTypeDic, pageForm, modelType);
            
            // 更新処理
            dm = new NormalDataManager(appSettingFile, dataDefinitionFolder, dataId, loginUser);
            DataRow drOpeTarget = PageModelManager.GetDataRow(modelType, model, mainTableDef);
            if (isNew)
            {
                int rowCount = dm.AddData(drOpeTarget);
            }
            else
            {
                int rowCount = dm.UpdateData(drOpeTarget);
            }

            // 画面遷移
            string uri = string.Format("/list2/{0}", dataId);
            naviMgr.NavigateTo(uri);
        }
        #endregion
        #region 外部参照キー項目入力イベント
        public void OnInputReferenceKeyValue (ChangeEventArgs e, string ColumnName)
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
                        foreach (FormItem fItem in pageForm.FormItems)
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

                                DataTable dtSubTable = dsSubTable.Tables[targetColInfo.ReferenceTable];
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

            //foreach (FormItem fItem in pageForm.FormItems)
            //{
            //    if (fItem.Name.Equals(_columnName))
            //    {
            //        string inputValue = fItem.Value.ToString();
            //    }
            //}
        }
        #endregion
        #endregion

        #region バリデーション
        private void ValidationRequested(object? sender, ValidationRequestedEventArgs args)
        {
            // 以前のエラーメッセージをクリア
            validationMessageStore.Clear();

            // バリデーション
            List<Validation> validList = new List<Validation>();
            foreach (FormItem formItem in pageForm.FormItems)
            {
                validList = vm.GetValidationInformation(formItem.Name);
                List<string> errMsgList = vm.ExecuteValidation(validList, formItem);
                foreach (string errorMessage in errMsgList)
                {
                    validationMessageStore.Add(() => formItem.Value!, errorMessage);
                }
            }
        }
        #endregion
    }
}
