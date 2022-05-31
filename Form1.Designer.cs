namespace PhotoEditor
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlImgShow = new System.Windows.Forms.Panel();
            this.pbImgShow = new System.Windows.Forms.PictureBox();
            this.btnL90 = new System.Windows.Forms.Button();
            this.btnR90 = new System.Windows.Forms.Button();
            this.btnZoomIn = new System.Windows.Forms.Button();
            this.btnZoomOut = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnPass = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.ckbDrawTime = new System.Windows.Forms.CheckBox();
            this.pnlImgShow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbImgShow)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlImgShow
            // 
            this.pnlImgShow.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlImgShow.Controls.Add(this.pbImgShow);
            this.pnlImgShow.Location = new System.Drawing.Point(12, 64);
            this.pnlImgShow.Name = "pnlImgShow";
            this.pnlImgShow.Size = new System.Drawing.Size(902, 677);
            this.pnlImgShow.TabIndex = 0;
            // 
            // pbImgShow
            // 
            this.pbImgShow.Location = new System.Drawing.Point(0, 0);
            this.pbImgShow.Name = "pbImgShow";
            this.pbImgShow.Size = new System.Drawing.Size(523, 425);
            this.pbImgShow.TabIndex = 0;
            this.pbImgShow.TabStop = false;
            this.pbImgShow.Paint += new System.Windows.Forms.PaintEventHandler(this.pbImgShow_Paint);
            this.pbImgShow.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbImgShow_MouseDown);
            this.pbImgShow.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbImgShow_MouseMove);
            this.pbImgShow.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbImgShow_MouseUp);
            // 
            // btnL90
            // 
            this.btnL90.Location = new System.Drawing.Point(94, 12);
            this.btnL90.Name = "btnL90";
            this.btnL90.Size = new System.Drawing.Size(75, 46);
            this.btnL90.TabIndex = 1;
            this.btnL90.Text = "左90 (Q)";
            this.btnL90.UseVisualStyleBackColor = true;
            this.btnL90.Click += new System.EventHandler(this.btnL90_Click);
            // 
            // btnR90
            // 
            this.btnR90.Location = new System.Drawing.Point(175, 12);
            this.btnR90.Name = "btnR90";
            this.btnR90.Size = new System.Drawing.Size(75, 46);
            this.btnR90.TabIndex = 1;
            this.btnR90.Text = "右90 (E)";
            this.btnR90.UseVisualStyleBackColor = true;
            this.btnR90.Click += new System.EventHandler(this.btnR90_Click);
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.Location = new System.Drawing.Point(256, 12);
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(75, 46);
            this.btnZoomIn.TabIndex = 1;
            this.btnZoomIn.Text = "IN";
            this.btnZoomIn.UseVisualStyleBackColor = true;
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Location = new System.Drawing.Point(337, 12);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(75, 46);
            this.btnZoomOut.TabIndex = 1;
            this.btnZoomOut.Text = "OUT";
            this.btnZoomOut.UseVisualStyleBackColor = true;
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(13, 12);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 46);
            this.btnLoad.TabIndex = 1;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnPass
            // 
            this.btnPass.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPass.Location = new System.Drawing.Point(756, 12);
            this.btnPass.Name = "btnPass";
            this.btnPass.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnPass.Size = new System.Drawing.Size(75, 46);
            this.btnPass.TabIndex = 1;
            this.btnPass.Text = "Pass (A)";
            this.btnPass.UseVisualStyleBackColor = true;
            this.btnPass.Click += new System.EventHandler(this.btnPass_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(837, 12);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 46);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save (S)";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // ckbDrawTime
            // 
            this.ckbDrawTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ckbDrawTime.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckbDrawTime.Location = new System.Drawing.Point(675, 12);
            this.ckbDrawTime.Name = "ckbDrawTime";
            this.ckbDrawTime.Size = new System.Drawing.Size(75, 46);
            this.ckbDrawTime.TabIndex = 2;
            this.ckbDrawTime.Text = "時間印 (T)";
            this.ckbDrawTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckbDrawTime.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(924, 751);
            this.Controls.Add(this.ckbDrawTime);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnPass);
            this.Controls.Add(this.btnZoomOut);
            this.Controls.Add(this.btnR90);
            this.Controls.Add(this.btnZoomIn);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnL90);
            this.Controls.Add(this.pnlImgShow);
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.pnlImgShow.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbImgShow)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlImgShow;
        private System.Windows.Forms.PictureBox pbImgShow;
        private System.Windows.Forms.Button btnL90;
        private System.Windows.Forms.Button btnR90;
        private System.Windows.Forms.Button btnZoomIn;
        private System.Windows.Forms.Button btnZoomOut;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnPass;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.CheckBox ckbDrawTime;
    }
}

