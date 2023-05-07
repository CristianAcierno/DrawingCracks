namespace Cracks
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnProcess = new System.Windows.Forms.Button();
            this.btnSelect = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbxPicture = new System.Windows.Forms.TextBox();
            this.pbxMain = new System.Windows.Forms.PictureBox();
            this.pbxResult = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbxMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxResult)).BeginInit();
            this.SuspendLayout();
            // 
            // btnProcess
            // 
            this.btnProcess.Location = new System.Drawing.Point(768, 33);
            this.btnProcess.Name = "btnProcess";
            this.btnProcess.Size = new System.Drawing.Size(117, 32);
            this.btnProcess.TabIndex = 9;
            this.btnProcess.Text = "Process";
            this.btnProcess.UseVisualStyleBackColor = true;
            this.btnProcess.Click += new System.EventHandler(this.btnProcess_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(645, 33);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(117, 32);
            this.btnSelect.TabIndex = 8;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 15);
            this.label1.TabIndex = 7;
            this.label1.Text = "Image";
            // 
            // tbxPicture
            // 
            this.tbxPicture.Location = new System.Drawing.Point(14, 39);
            this.tbxPicture.Margin = new System.Windows.Forms.Padding(5);
            this.tbxPicture.Name = "tbxPicture";
            this.tbxPicture.Size = new System.Drawing.Size(603, 23);
            this.tbxPicture.TabIndex = 6;
            // 
            // pbxMain
            // 
            this.pbxMain.Location = new System.Drawing.Point(14, 72);
            this.pbxMain.Margin = new System.Windows.Forms.Padding(5);
            this.pbxMain.Name = "pbxMain";
            this.pbxMain.Size = new System.Drawing.Size(360, 420);
            this.pbxMain.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxMain.TabIndex = 5;
            this.pbxMain.TabStop = false;
            // 
            // pbxResult
            // 
            this.pbxResult.Location = new System.Drawing.Point(525, 73);
            this.pbxResult.Margin = new System.Windows.Forms.Padding(5);
            this.pbxResult.Name = "pbxResult";
            this.pbxResult.Size = new System.Drawing.Size(360, 420);
            this.pbxResult.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxResult.TabIndex = 10;
            this.pbxResult.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(904, 509);
            this.Controls.Add(this.pbxResult);
            this.Controls.Add(this.btnProcess);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbxPicture);
            this.Controls.Add(this.pbxMain);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbxMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxResult)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button btnProcess;
        private Button btnSelect;
        private Label label1;
        private TextBox tbxPicture;
        private PictureBox pbxMain;
        private PictureBox pbxResult;
    }
}