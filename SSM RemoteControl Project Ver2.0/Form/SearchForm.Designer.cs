namespace SSM_RemoteControl_Project_Ver2._0
{
    partial class SearchForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchForm));
            this.label1 = new System.Windows.Forms.Label();
            this.tb_search_ip = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_search_email = new System.Windows.Forms.TextBox();
            this.btn_search_send = new System.Windows.Forms.Button();
            this.cb_mail = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(46, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "Input IP :";
            // 
            // tb_search_ip
            // 
            this.tb_search_ip.Location = new System.Drawing.Point(110, 26);
            this.tb_search_ip.Name = "tb_search_ip";
            this.tb_search_ip.Size = new System.Drawing.Size(181, 21);
            this.tb_search_ip.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "Input Email :";
            // 
            // tb_search_email
            // 
            this.tb_search_email.Location = new System.Drawing.Point(110, 56);
            this.tb_search_email.Name = "tb_search_email";
            this.tb_search_email.Size = new System.Drawing.Size(81, 21);
            this.tb_search_email.TabIndex = 1;
            // 
            // btn_search_send
            // 
            this.btn_search_send.Location = new System.Drawing.Point(54, 100);
            this.btn_search_send.Name = "btn_search_send";
            this.btn_search_send.Size = new System.Drawing.Size(237, 23);
            this.btn_search_send.TabIndex = 3;
            this.btn_search_send.Text = "Send Code to Email";
            this.btn_search_send.UseVisualStyleBackColor = true;
            this.btn_search_send.Click += new System.EventHandler(this.btn_search_send_Click);
            // 
            // cb_mail
            // 
            this.cb_mail.FormattingEnabled = true;
            this.cb_mail.Items.AddRange(new object[] {
            "naver.com",
            "daum.net",
            "gmail.com"});
            this.cb_mail.Location = new System.Drawing.Point(209, 56);
            this.cb_mail.Name = "cb_mail";
            this.cb_mail.Size = new System.Drawing.Size(82, 20);
            this.cb_mail.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(191, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "@";
            // 
            // SearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(328, 136);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cb_mail);
            this.Controls.Add(this.btn_search_send);
            this.Controls.Add(this.tb_search_email);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tb_search_ip);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SearchForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Remote Control Search";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb_search_ip;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_search_email;
        private System.Windows.Forms.Button btn_search_send;
        private System.Windows.Forms.ComboBox cb_mail;
        private System.Windows.Forms.Label label3;
    }
}