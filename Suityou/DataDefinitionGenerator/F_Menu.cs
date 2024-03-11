using DataDefinitionGenerator.Common;

namespace DataDefinitionGenerator
{
    public partial class F_Menu : Form
    {
        public F_Menu()
        {
            InitializeComponent();

            // モード説明テーブルにイベントハンドラを追加
            tbl_mode_description.CellPaint += new TableLayoutCellPaintEventHandler(tbl_mode_description_CellPaint);
        }

        #region イベントハンドラ
        #region モード説明テーブル
        //tbl_mode_descriptionのCellPaintイベントハンドラ
        private void tbl_mode_description_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            // ヘッダ行の背景色を黒にする
            if (e.Row == 0)
            {
                e.Graphics.FillRectangle(Brushes.Black, e.CellBounds);
            }
            // ヘッダ以外の背景色を白にする
            else
            {
                e.Graphics.FillRectangle(Brushes.White, e.CellBounds);
            }
        }
        #endregion
        #region モード選択ボタン
        // 「モード選択ボタン」クリックイベントハンドラ
        private void btnModeSelect_Click(object sender, EventArgs e)
        {
            // 選択モード取得
            string selectMode = cb_ExecMode.Text;

            if (selectMode.Equals(string.Empty))
            {
                // 実行モード未選択時はエラー
                MessageBox.Show("実行モードを選択してください。");
            }
            else
            {
                // 実行モード別処理
                switch (selectMode)
                {
                    // 新規モード
                    case Const.PROC_MODE_NEW:
                        NewModeProc();
                        break;
                    // 修正モード
                    case Const.PROC_MODE_MODIFY:
                        ModifyModeProc();
                        break;
                }
            }
        }
        #endregion
        #endregion

        #region サブルーチン
        #region 新規モード処理
        private void NewModeProc ()
        {

        }
        #endregion
        #region 修正モード処理
        private void ModifyModeProc ()
        {

        }
        #endregion
        #endregion
    }
}
