using Microsoft.AspNetCore.Components;
using Microsoft.Fast.Components.FluentUI;
using Microsoft.JSInterop;
using Suityou.Framework.Web.Common;
using Suityou.Framework.Web.DataManager;
using Suityou.Framework.Web.DataModel;
using System.Data;

namespace BlazorAppNoAuth.Pages.WFW
{
    public partial class UploadDialog
    {
        [Parameter]
        public UploadData Content { get; set; } = default!;

        [CascadingParameter]
        public FluentDialog? Dialog { get; set; }

        FluentInputFileEventArgs[] Files = Array.Empty<FluentInputFileEventArgs>();
        int progressPercent = 0;

        // 多言語項目
        private string buttonExecute = Properties.Resources.BUTTON_EXECUTE;
        private string buttonClose = Properties.Resources.BUTTON_CLOSE;
        private string messageInputFile = Properties.Resources.MESSAGE_INPUT_FILE;
        private string messageUploaded = Properties.Resources.MESSAGE_UPLOADED;
        private string messageReadDataErrorFormat = Properties.Resources.MESSAGE_READFILE_ERROR_FORMAT;
        private string messageReadDataErrorNoData = Properties.Resources.MESSAGE_READFILE_ERROR_NODATA;
        private string messageUploadErrorClear = Properties.Resources.MESSAGE_UPLOAD_ERROR_CLEAR;
        private string messageUploadErrorAdd = Properties.Resources.MESSAGE_UPLOAD_ERROR_ADD;
        private string radioUploadReplace = Properties.Resources.RADIO_UPLOAD_REPLACE;
        private string radioUploadAdd = Properties.Resources.RADIO_UPLOAD_ADD;

        // ボタン制御
        private string executeButtonClassValue = "disabled btn btn-dialog-primary";

        // アップロード種別
        private string uploadType = "replace";

        // アップロードエラー判定
        private bool uploadErrorExists = false;
        private List<string> uploadErrors = new List<string>();

        private async Task OnCompletedAsync(IEnumerable<FluentInputFileEventArgs> files)
        {
            Files = files.ToArray();

            // 実行ボタンを活性化
            executeButtonClassValue = "btn btn-dialog-primary";

            // 進捗バーのクリア(アップロード完了1.5秒後)
            await Task.Delay(1500);
            progressPercent = 0;
        }

        private async Task UploadExecuteAsync ()
        {
            // アップロードファイルの格納パス
            string dataFilePath = Files[0].LocalFile.FullName;
            
            // エラーリスト
            List<int> readFileErrors = new List<int>();
            Dictionary<int, string> uploadExecErrors = new Dictionary<int, string>();

            try
            {
                // アップロードファイル読み込み
                DataTable dtUploadData = Content.dm.ReadUploadFile(dataFilePath, ref readFileErrors);
                // エラー判定
                if (readFileErrors.Count > 0)
                {
                    foreach (int errorCode in readFileErrors)
                    {
                        switch (errorCode)
                        {
                            case SuityouWFWConst.ERRORCODE_READFILE_NO_DATA:
                                uploadErrors.Add(messageReadDataErrorNoData);
                                break;
                            case SuityouWFWConst.ERRORCODE_READFILE_FORMAT_ERROR:
                                uploadErrors.Add(messageReadDataErrorFormat);
                                break;
                        }
                    }

                    uploadErrorExists = true;
                }
                else
                {
                    // アップロード処理
                    bool dataClear = uploadType.Equals("replace") ? true : false;
                    int uploadedCount = Content.dm.UploadData(dtUploadData, dataClear, ref uploadExecErrors);

                    // エラー判定
                    if (uploadedCount == -1)
                    {
                        // データクリア失敗時
                        uploadErrors.Add(messageUploadErrorClear);

                        uploadErrorExists = true;
                    }
                    else
                    {
                        if (uploadExecErrors.Keys.Count > 0)
                        {
                            // データ登録失敗
                            foreach (int rowIndex in uploadExecErrors.Keys)
                            {
                                uploadErrors.Add(string.Format(messageUploadErrorAdd, rowIndex.ToString()));
                            }

                            uploadErrorExists = true;
                        }
                        else
                        {
                            // アップロード成功
                            uploadErrorExists = false;
                        }
                    }
                }
            }
            finally
            {
                // ファイル削除
                if (Files.Length > 0)
                {
                    foreach (var file in Files)
                    {
                        file.LocalFile?.Delete();
                    }
                }
                Files = Array.Empty<FluentInputFileEventArgs>();

                if (!uploadErrorExists)
                {
                    // アップロード成功の場合はUploadDialogを閉じてリロード
                    await Dialog.CloseAsync();
                    await JsRuntime.InvokeVoidAsync("eval", "location.reload()");
                }
            }
        }

        private async Task CloseAsync ()
        {
            // ファイル選択済みの場合は削除する
            if (Files.Length > 0)
            {
                foreach (var file in Files)
                {
                    file.LocalFile?.Delete();
                }
            }

            await Dialog.CloseAsync();
        }
    }
}
