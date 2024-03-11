namespace DataDefinitionGenerator
{
    partial class F_Menu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            groupBox1 = new GroupBox();
            btnModeSelect = new Button();
            cb_ExecMode = new ComboBox();
            label2 = new Label();
            groupBox2 = new GroupBox();
            tbl_mode_description = new TableLayoutPanel();
            label9 = new Label();
            label8 = new Label();
            label7 = new Label();
            label6 = new Label();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            tbl_mode_description.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Meiryo UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(189, 19);
            label1.TabIndex = 1;
            label1.Text = "実行モードを選択してください。";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btnModeSelect);
            groupBox1.Controls.Add(cb_ExecMode);
            groupBox1.Controls.Add(label2);
            groupBox1.Location = new Point(31, 40);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(349, 67);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            // 
            // btnModeSelect
            // 
            btnModeSelect.BackColor = Color.Black;
            btnModeSelect.FlatStyle = FlatStyle.Popup;
            btnModeSelect.Font = new Font("Yu Gothic UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            btnModeSelect.ForeColor = Color.White;
            btnModeSelect.Location = new Point(245, 21);
            btnModeSelect.Name = "btnModeSelect";
            btnModeSelect.Size = new Size(89, 35);
            btnModeSelect.TabIndex = 3;
            btnModeSelect.Text = "モード選択";
            btnModeSelect.UseVisualStyleBackColor = false;
            btnModeSelect.Click += btnModeSelect_Click;
            // 
            // cb_ExecMode
            // 
            cb_ExecMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cb_ExecMode.FormattingEnabled = true;
            cb_ExecMode.Items.AddRange(new object[] { "新規モード", "修正モード" });
            cb_ExecMode.Location = new Point(104, 24);
            cb_ExecMode.Name = "cb_ExecMode";
            cb_ExecMode.Size = new Size(121, 27);
            cb_ExecMode.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Meiryo UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label2.Location = new Point(16, 27);
            label2.Name = "label2";
            label2.Size = new Size(73, 19);
            label2.TabIndex = 2;
            label2.Text = "実行モード";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(tbl_mode_description);
            groupBox2.Controls.Add(label3);
            groupBox2.Location = new Point(31, 132);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(592, 188);
            groupBox2.TabIndex = 5;
            groupBox2.TabStop = false;
            groupBox2.Text = "注意事項";
            // 
            // tbl_mode_description
            // 
            tbl_mode_description.BackColor = Color.White;
            tbl_mode_description.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tbl_mode_description.ColumnCount = 2;
            tbl_mode_description.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tbl_mode_description.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75F));
            tbl_mode_description.Controls.Add(label9, 1, 2);
            tbl_mode_description.Controls.Add(label8, 1, 1);
            tbl_mode_description.Controls.Add(label7, 0, 2);
            tbl_mode_description.Controls.Add(label6, 0, 1);
            tbl_mode_description.Controls.Add(label5, 1, 0);
            tbl_mode_description.Controls.Add(label4, 0, 0);
            tbl_mode_description.Cursor = Cursors.No;
            tbl_mode_description.Location = new Point(35, 70);
            tbl_mode_description.Name = "tbl_mode_description";
            tbl_mode_description.RowCount = 3;
            tbl_mode_description.RowStyles.Add(new RowStyle(SizeType.Percent, 34F));
            tbl_mode_description.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));
            tbl_mode_description.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));
            tbl_mode_description.Size = new Size(533, 100);
            tbl_mode_description.TabIndex = 6;
            // 
            // label9
            // 
            label9.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label9.Font = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label9.ForeColor = Color.Black;
            label9.Location = new Point(137, 68);
            label9.Name = "label9";
            label9.Size = new Size(392, 28);
            label9.TabIndex = 10;
            label9.Text = "バリデーション定義JSONファイルが格納されているフォルダ";
            label9.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label8
            // 
            label8.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label8.Font = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label8.ForeColor = Color.Black;
            label8.Location = new Point(137, 35);
            label8.Name = "label8";
            label8.Size = new Size(392, 28);
            label8.TabIndex = 9;
            label8.Text = "データ定義JSONファイルが格納されているフォルダ";
            label8.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            label7.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label7.Font = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label7.ForeColor = Color.Black;
            label7.Location = new Point(4, 68);
            label7.Name = "label7";
            label7.Size = new Size(126, 28);
            label7.TabIndex = 8;
            label7.Text = "Validation";
            label7.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label6.Font = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label6.ForeColor = Color.Black;
            label6.Location = new Point(4, 35);
            label6.Name = "label6";
            label6.Size = new Size(126, 28);
            label6.TabIndex = 7;
            label6.Text = "Data";
            label6.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label5.BackColor = Color.Black;
            label5.Font = new Font("Yu Gothic UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            label5.ForeColor = Color.White;
            label5.Location = new Point(137, 3);
            label5.Name = "label5";
            label5.Size = new Size(392, 28);
            label5.TabIndex = 7;
            label5.Text = "フォルダ説明";
            label5.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label4.BackColor = Color.Black;
            label4.Font = new Font("Yu Gothic UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            label4.ForeColor = Color.White;
            label4.Location = new Point(4, 3);
            label4.Name = "label4";
            label4.Size = new Size(126, 28);
            label4.TabIndex = 0;
            label4.Text = "フォルダ名";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            label3.Font = new Font("Meiryo UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label3.Location = new Point(16, 23);
            label3.Name = "label3";
            label3.RightToLeft = RightToLeft.No;
            label3.Size = new Size(466, 44);
            label3.TabIndex = 5;
            label3.Text = "修正モードの場合は、「モード選択」ボタン押下後に表示されるダイアログにて、以下のファイルが配下に含まれるフォルダを選択してください。";
            // 
            // F_Menu
            // 
            AutoScaleDimensions = new SizeF(8F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(660, 345);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(label1);
            Name = "F_Menu";
            Text = "suityou定義ファイル作成";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            tbl_mode_description.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private GroupBox groupBox1;
        private ComboBox cb_ExecMode;
        private Label label2;
        private GroupBox groupBox2;
        private TableLayoutPanel tbl_mode_description;
        private Label label3;
        private Label label4;
        private Label label6;
        private Label label5;
        private Label label7;
        private Label label8;
        private Button btnModeSelect;
        private Label label9;
    }
}