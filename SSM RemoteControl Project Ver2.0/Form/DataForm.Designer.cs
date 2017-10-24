namespace SSM_RemoteControl_Project_Ver2._0
{
    partial class DataForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataForm));
            this.label6 = new System.Windows.Forms.Label();
            this.label_data_mac = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label_data_ip = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(50, 39);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "Mac Address :";
            // 
            // label_data_mac
            // 
            this.label_data_mac.AutoSize = true;
            this.label_data_mac.Location = new System.Drawing.Point(165, 37);
            this.label_data_mac.Name = "label_data_mac";
            this.label_data_mac.Size = new System.Drawing.Size(103, 12);
            this.label_data_mac.TabIndex = 1;
            this.label_data_mac.Text = "AA:BB:CC:DD:EE";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(64, 16);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(75, 12);
            this.label7.TabIndex = 2;
            this.label7.Text = "IP Address :";
            // 
            // label_data_ip
            // 
            this.label_data_ip.AutoSize = true;
            this.label_data_ip.Location = new System.Drawing.Point(165, 16);
            this.label_data_ip.Name = "label_data_ip";
            this.label_data_ip.Size = new System.Drawing.Size(65, 12);
            this.label_data_ip.TabIndex = 3;
            this.label_data_ip.Text = "192.168.0.1";
            // 
            // DataForm
            // 
            this.ClientSize = new System.Drawing.Size(314, 65);
            this.Controls.Add(this.label_data_ip);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label_data_mac);
            this.Controls.Add(this.label6);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DataForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Remote Control User Info";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb_search_ip;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_search_email;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tb_search_pre_code;
        private System.Windows.Forms.TextBox tb_search_post_code;
        private System.Windows.Forms.TextBox tb_search_post_code2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btn_change_change;
        private System.Windows.Forms.Button btn_change_init;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label_data_mac;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label_data_ip;

    }
}