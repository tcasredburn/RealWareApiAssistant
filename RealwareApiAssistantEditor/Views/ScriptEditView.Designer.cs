namespace RealwareApiAssistantEditor.Views
{
    partial class ScriptEditView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            this.xtraTabPage1 = new DevExpress.XtraTab.XtraTabPage();
            this.xtraTabPage2 = new DevExpress.XtraTab.XtraTabPage();
            this.scriptTreeEditView1 = new RealwareApiAssistantEditor.Views.ScriptTreeEditView();
            this.scriptTextEditView1 = new RealwareApiAssistantEditor.Views.ScriptTextEditView();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
            this.xtraTabControl1.SuspendLayout();
            this.xtraTabPage1.SuspendLayout();
            this.xtraTabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // xtraTabControl1
            // 
            this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraTabControl1.HeaderLocation = DevExpress.XtraTab.TabHeaderLocation.Left;
            this.xtraTabControl1.HeaderOrientation = DevExpress.XtraTab.TabOrientation.Horizontal;
            this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
            this.xtraTabControl1.Name = "xtraTabControl1";
            this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
            this.xtraTabControl1.Size = new System.Drawing.Size(658, 575);
            this.xtraTabControl1.TabIndex = 0;
            this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.xtraTabPage1,
            this.xtraTabPage2});
            // 
            // xtraTabPage1
            // 
            this.xtraTabPage1.Controls.Add(this.scriptTreeEditView1);
            this.xtraTabPage1.Name = "xtraTabPage1";
            this.xtraTabPage1.Size = new System.Drawing.Size(611, 573);
            this.xtraTabPage1.Text = "Simple";
            // 
            // xtraTabPage2
            // 
            this.xtraTabPage2.Controls.Add(this.scriptTextEditView1);
            this.xtraTabPage2.Name = "xtraTabPage2";
            this.xtraTabPage2.Size = new System.Drawing.Size(611, 573);
            this.xtraTabPage2.Text = "Script";
            // 
            // scriptTreeEditView1
            // 
            this.scriptTreeEditView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scriptTreeEditView1.Location = new System.Drawing.Point(0, 0);
            this.scriptTreeEditView1.Name = "scriptTreeEditView1";
            this.scriptTreeEditView1.Size = new System.Drawing.Size(611, 573);
            this.scriptTreeEditView1.TabIndex = 0;
            // 
            // scriptTextEditView1
            // 
            this.scriptTextEditView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scriptTextEditView1.Location = new System.Drawing.Point(0, 0);
            this.scriptTextEditView1.Name = "scriptTextEditView1";
            this.scriptTextEditView1.Size = new System.Drawing.Size(611, 573);
            this.scriptTextEditView1.TabIndex = 0;
            // 
            // ScriptEditView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.xtraTabControl1);
            this.Name = "ScriptEditView";
            this.Size = new System.Drawing.Size(658, 575);
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
            this.xtraTabControl1.ResumeLayout(false);
            this.xtraTabPage1.ResumeLayout(false);
            this.xtraTabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage1;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage2;
        private ScriptTreeEditView scriptTreeEditView1;
        private ScriptTextEditView scriptTextEditView1;
    }
}
