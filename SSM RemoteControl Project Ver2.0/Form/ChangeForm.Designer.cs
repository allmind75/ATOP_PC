namespace SSM_RemoteControl_Project_Ver2._0
{
    partial class ChangeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangeForm));
            this.label1 = new System.Windows.Forms.Label();
            this.tb_search_ip = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_search_email = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tb_search_pre_code = new System.Windows.Forms.TextBox();
            this.tb_search_post_code = new System.Windows.Forms.TextBox();
            this.tb_search_post_code2 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btn_change_change = new System.Windows.Forms.Button();
            this.btn_change_init = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(77, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "Ip :";
            // 
            // tb_search_ip
            // 
            this.tb_search_ip.Location = new System.Drawing.Point(114, 8);
            this.tb_search_ip.Name = "tb_search_ip";
            this.tb_search_ip.Size = new System.Drawing.Size(135, 21);
            this.tb_search_ip.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(49, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "E-Mail :";
            // 
            // tb_search_email
            // 
            this.tb_search_email.Location = new System.Drawing.Point(114, 33);
            this.tb_search_email.Name = "tb_search_email";
            this.tb_search_email.Size = new System.Drawing.Size(135, 21);
            this.tb_search_email.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "이전 비밀번호 :";
            // 
            // tb_search_pre_code
            // 
            this.tb_search_pre_code.Location = new System.Drawing.Point(114, 58);
            this.tb_search_pre_code.Name = "tb_search_pre_code";
            this.tb_search_pre_code.PasswordChar = '*';
            this.tb_search_pre_code.Size = new System.Drawing.Size(135, 21);
            this.tb_search_pre_code.TabIndex = 2;
            // 
            // tb_search_post_code
            // 
            this.tb_search_post_code.Location = new System.Drawing.Point(114, 83);
            this.tb_search_post_code.Name = "tb_search_post_code";
            this.tb_search_post_code.PasswordChar = '*';
            this.tb_search_post_code.Size = new System.Drawing.Size(135, 21);
            this.tb_search_post_code.TabIndex = 3;
            // 
            // tb_search_post_code2
            // 
            this.tb_search_post_code2.Location = new System.Drawing.Point(112, 106);
            this.tb_search_post_code2.Name = "tb_search_post_code2";
            this.tb_search_post_code2.PasswordChar = '*';
            this.tb_search_post_code2.Size = new System.Drawing.Size(135, 21);
            this.tb_search_post_code2.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(24, 86);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 12);
            this.label4.TabIndex = 10;
            this.label4.Text = "새 비밀번호 :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(48, 109);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 11;
            this.label5.Text = "재 입력 :";
            // 
            // btn_change_change
            // 
            this.btn_change_change.Location = new System.Drawing.Point(256, 8);
            this.btn_change_change.Name = "btn_change_change";
            this.btn_change_change.Size = new System.Drawing.Size(60, 96);
            this.btn_change_change.TabIndex = 5;
            this.btn_change_change.Text = "Change";
            this.btn_change_change.UseVisualStyleBackColor = true;
            this.btn_change_change.Click += new System.EventHandler(this.btn_change_change_Click);
            // 
            // btn_change_init
            // 
            this.btn_change_init.Location = new System.Drawing.Point(256, 108);
            this.btn_change_init.Name = "btn_change_init";
            this.btn_change_init.Size = new System.Drawing.Size(60, 21);
            this.btn_change_init.TabIndex = 6;
            this.btn_change_init.Text = "Init";
            this.btn_change_init.UseVisualStyleBackColor = true;
            this.btn_change_init.Click += new System.EventHandler(this.btn_change_init_Click);
            // 
            // ChangeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(328, 136);
            this.Controls.Add(this.btn_change_init);
            this.Controls.Add(this.btn_change_change);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tb_search_post_code2);
            this.Controls.Add(this.tb_search_post_code);
            this.Controls.Add(this.tb_search_pre_code);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tb_search_email);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tb_search_ip);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ChangeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Remote Control Change Code";
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

    }
}