namespace SignalAnalysis
{
    partial class FrmLanguage
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
            this.cboAllCultures = new System.Windows.Forms.ComboBox();
            this.radCurrentCulture = new System.Windows.Forms.RadioButton();
            this.radInvariantCulture = new System.Windows.Forms.RadioButton();
            this.radUserCulture = new System.Windows.Forms.RadioButton();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnAccept = new System.Windows.Forms.Button();
            this.lblCurrentCulture = new System.Windows.Forms.Label();
            this.lblInvariantCulture = new System.Windows.Forms.Label();
            this.lblUserCulture = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cboAllCultures
            // 
            this.cboAllCultures.FormattingEnabled = true;
            this.cboAllCultures.Location = new System.Drawing.Point(45, 141);
            this.cboAllCultures.Name = "cboAllCultures";
            this.cboAllCultures.Size = new System.Drawing.Size(190, 25);
            this.cboAllCultures.TabIndex = 3;
            this.cboAllCultures.SelectionChangeCommitted += AllCultures_SelectionChangeCommitted;
            // 
            // radCurrentCulture
            // 
            this.radCurrentCulture.AutoSize = true;
            this.radCurrentCulture.Location = new System.Drawing.Point(23, 32);
            this.radCurrentCulture.MaximumSize = new System.Drawing.Size(275, 0);
            this.radCurrentCulture.Name = "radCurrentCulture";
            this.radCurrentCulture.Size = new System.Drawing.Size(14, 13);
            this.radCurrentCulture.TabIndex = 0;
            this.radCurrentCulture.TabStop = true;
            this.radCurrentCulture.UseVisualStyleBackColor = true;
            this.radCurrentCulture.CheckedChanged += new System.EventHandler(this.CurrentCulture_CheckedChanged);
            // 
            // radInvariantCulture
            // 
            this.radInvariantCulture.AutoSize = true;
            this.radInvariantCulture.Location = new System.Drawing.Point(23, 73);
            this.radInvariantCulture.MaximumSize = new System.Drawing.Size(275, 0);
            this.radInvariantCulture.Name = "radInvariantCulture";
            this.radInvariantCulture.Size = new System.Drawing.Size(14, 13);
            this.radInvariantCulture.TabIndex = 1;
            this.radInvariantCulture.TabStop = true;
            this.radInvariantCulture.UseVisualStyleBackColor = true;
            this.radInvariantCulture.CheckedChanged += new System.EventHandler(this.InvariantCulture_CheckedChanged);
            // 
            // radUserCulture
            // 
            this.radUserCulture.AutoSize = true;
            this.radUserCulture.Location = new System.Drawing.Point(23, 114);
            this.radUserCulture.MaximumSize = new System.Drawing.Size(275, 0);
            this.radUserCulture.Name = "radUserCulture";
            this.radUserCulture.Size = new System.Drawing.Size(14, 13);
            this.radUserCulture.TabIndex = 2;
            this.radUserCulture.TabStop = true;
            this.radUserCulture.UseVisualStyleBackColor = true;
            this.radUserCulture.CheckedChanged += new System.EventHandler(this.UserCulture_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(117, 190);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 30);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // btnAccept
            // 
            this.btnAccept.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnAccept.Location = new System.Drawing.Point(213, 190);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(90, 30);
            this.btnAccept.TabIndex = 5;
            this.btnAccept.Text = "&Accept";
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.Accept_Click);
            // 
            // lblCurrentCulture
            // 
            this.lblCurrentCulture.AutoSize = true;
            this.lblCurrentCulture.BackColor = System.Drawing.Color.Transparent;
            this.lblCurrentCulture.Location = new System.Drawing.Point(39, 29);
            this.lblCurrentCulture.MaximumSize = new System.Drawing.Size(275, 0);
            this.lblCurrentCulture.Name = "lblCurrentCulture";
            this.lblCurrentCulture.Size = new System.Drawing.Size(102, 19);
            this.lblCurrentCulture.TabIndex = 6;
            this.lblCurrentCulture.Text = "Current culture";
            this.lblCurrentCulture.Click += new System.EventHandler(this.LabelCulture_Click);
            // 
            // lblInvariantCulture
            // 
            this.lblInvariantCulture.AutoSize = true;
            this.lblInvariantCulture.Location = new System.Drawing.Point(39, 70);
            this.lblInvariantCulture.MaximumSize = new System.Drawing.Size(275, 0);
            this.lblInvariantCulture.Name = "lblInvariantCulture";
            this.lblInvariantCulture.Size = new System.Drawing.Size(109, 19);
            this.lblInvariantCulture.TabIndex = 7;
            this.lblInvariantCulture.Text = "Invariant culture";
            this.lblInvariantCulture.Click += new System.EventHandler(this.LabelCulture_Click);
            // 
            // lblUserCulture
            // 
            this.lblUserCulture.AutoSize = true;
            this.lblUserCulture.Location = new System.Drawing.Point(39, 111);
            this.lblUserCulture.MaximumSize = new System.Drawing.Size(275, 0);
            this.lblUserCulture.Name = "lblUserCulture";
            this.lblUserCulture.Size = new System.Drawing.Size(90, 19);
            this.lblUserCulture.TabIndex = 8;
            this.lblUserCulture.Text = "Select culture";
            this.lblUserCulture.Click += new System.EventHandler(this.LabelCulture_Click);
            // 
            // FrmLanguage
            // 
            this.AcceptButton = this.btnAccept;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(315, 232);
            this.Controls.Add(this.lblUserCulture);
            this.Controls.Add(this.lblInvariantCulture);
            this.Controls.Add(this.lblCurrentCulture);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.radUserCulture);
            this.Controls.Add(this.radInvariantCulture);
            this.Controls.Add(this.radCurrentCulture);
            this.Controls.Add(this.cboAllCultures);
            this.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmLanguage";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select language";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComboBox cboAllCultures;
        private RadioButton radCurrentCulture;
        private RadioButton radInvariantCulture;
        private RadioButton radUserCulture;
        private Button btnCancel;
        private Button btnAccept;
        private Label lblCurrentCulture;
        private Label lblInvariantCulture;
        private Label lblUserCulture;
    }
}
