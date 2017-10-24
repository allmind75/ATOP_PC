namespace SSM_RemoteControl_Project_Ver2._0
{
    partial class JoinForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JoinForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label_join_ip = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_join_code1 = new System.Windows.Forms.TextBox();
            this.tb_join_code2 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_join_join = new System.Windows.Forms.Button();
            this.btn_join_init = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.tb_join_email = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 9F);
            this.label1.Location = new System.Drawing.Point(81, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "Ip :";
            // 
            // label_join_ip
            // 
            this.label_join_ip.AutoSize = true;
            this.label_join_ip.Font = new System.Drawing.Font("굴림", 9F);
            this.label_join_ip.Location = new System.Drawing.Point(137, 16);
            this.label_join_ip.Name = "label_join_ip";
            this.label_join_ip.Size = new System.Drawing.Size(65, 12);
            this.label_join_ip.TabIndex = 0;
            this.label_join_ip.Text = "192.168.0.1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(34, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "Cert Code :";
            // 
            // tb_join_code1
            // 
            this.tb_join_code1.Location = new System.Drawing.Point(115, 37);
            this.tb_join_code1.Name = "tb_join_code1";
            this.tb_join_code1.PasswordChar = '*';
            this.tb_join_code1.Size = new System.Drawing.Size(127, 21);
            this.tb_join_code1.TabIndex = 1;
            // 
            // tb_join_code2
            // 
            this.tb_join_code2.Location = new System.Drawing.Point(115, 68);
            this.tb_join_code2.Name = "tb_join_code2";
            this.tb_join_code2.PasswordChar = '*';
            this.tb_join_code2.Size = new System.Drawing.Size(127, 21);
            this.tb_join_code2.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "Re Cert Code :";
            // 
            // btn_join_join
            // 
            this.btn_join_join.Location = new System.Drawing.Point(248, 37);
            this.btn_join_join.Name = "btn_join_join";
            this.btn_join_join.Size = new System.Drawing.Size(68, 52);
            this.btn_join_join.TabIndex = 4;
            this.btn_join_join.Text = "Join";
            this.btn_join_join.UseVisualStyleBackColor = true;
            this.btn_join_join.Click += new System.EventHandler(this.btn_join_join_Click);
            // 
            // btn_join_init
            // 
            this.btn_join_init.Location = new System.Drawing.Point(248, 96);
            this.btn_join_init.Name = "btn_join_init";
            this.btn_join_init.Size = new System.Drawing.Size(68, 23);
            this.btn_join_init.TabIndex = 5;
            this.btn_join_init.Text = "Init";
            this.btn_join_init.UseVisualStyleBackColor = true;
            this.btn_join_init.Click += new System.EventHandler(this.btn_join_init_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(53, 101);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "E-Mail :";
            // 
            // tb_join_email
            // 
            this.tb_join_email.Location = new System.Drawing.Point(115, 96);
            this.tb_join_email.Name = "tb_join_email";
            this.tb_join_email.Size = new System.Drawing.Size(127, 21);
            this.tb_join_email.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(47, 133);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(241, 12);
            this.label5.TabIndex = 10;
            this.label5.Text = "서버 프로그램 최초 인증코드를 등록합니다.";
            // 
            // JoinForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(328, 156);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tb_join_email);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btn_join_init);
            this.Controls.Add(this.btn_join_join);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tb_join_code2);
            this.Controls.Add(this.tb_join_code1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label_join_ip);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "JoinForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Remote Control Join";
            this.Load += new System.EventHandler(this.JoinForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label_join_ip;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_join_code1;
        private System.Windows.Forms.TextBox tb_join_code2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btn_join_join;
        private System.Windows.Forms.Button btn_join_init;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tb_join_email;
        private System.Windows.Forms.Label label5;
    }
}