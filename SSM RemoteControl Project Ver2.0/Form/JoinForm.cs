using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SSM_RemoteControl_Project_Ver2._0
{
    // 사용자 최초 가입 절차를 진행하기 위한 폼
    public partial class JoinForm : Form
    {
        private string login_ip;
        private string login_code;
        private string login_email;
        private string filepath;
        private string is_join; 

        private iniData ini_data;  
        private AES256 aes;

        private string folder_path = @"C:\Atop";
        private string temp_path = @"C:\Atop\config.ini";
        private DirectoryInfo di;

        public JoinForm()
        {
            aes = new AES256();
            di = new DirectoryInfo(folder_path);
            InitializeComponent(); 
        } 

        private void btn_join_init_Click(object sender, EventArgs e)
        {
            tb_join_code1.Text = "";
            tb_join_code2.Text = "";
            tb_join_email.Text = "";
        }

        private void btn_join_join_Click(object sender, EventArgs e)
        {
            this.login_ip = label_join_ip.Text.ToString();
            this.login_code = aes.AES_Encode(tb_join_code1.Text.ToString());
            this.login_email = tb_join_email.Text.ToString();

            if (String.Compare(tb_join_code1.Text, tb_join_code2.Text) != 0) // 사용자가 가입하기 위해 입력한 코드가 다르면
            {
                MessageBox.Show("Check to your code");
                tb_join_code1.Text = "";
                tb_join_code2.Text = "";
                tb_join_code1.Focus();
            }
            else if (String.IsNullOrEmpty(login_email)) // 이메일을 입력하지 않았으면
            {
                MessageBox.Show("이메일을 입력 해 주세요.");
            }
            else if (IsValidEmail(login_email) == false) // 이메일 정규식에 맞지 않으면
            {
                MessageBox.Show("올바른 이메일 형식을 입력하세요.");
            }
            // 사용자가 가입하기 위해 입력한 코드가 같고 이메일 형식이 맞아야 함
            else if (String.Compare(tb_join_code1.Text, tb_join_code2.Text) == 0 && IsValidEmail(login_email))
            { 
                this.is_join = "joined"; // 가입 되었다고 함.

                filepath = Path.Combine(Path.GetTempPath(), temp_path);

                if (di.Exists == false)
                {
                    di.Create(); // 폴더가 없으면 생성
                }

                ini_data = new iniData(filepath);

                ini_data.SetIniValue("Remote Control System Information", "UserIp", login_ip);
                ini_data.SetIniValue("Remote Control System Information", "UserCode", login_code);
                ini_data.SetIniValue("Remote Control System Information", "UserEMail", login_email);
                ini_data.SetIniValue("Remote Control System Information", "UserIsJoin", is_join);

                Application.Restart(); // 회원가입이 진행된 이후 어플리케이션을 다시 실행해야 함.
            }
        }

        private void JoinForm_Load(object sender, EventArgs e)
        {
            label_join_ip.Text = Return_Ip();
        }

        private string Return_Ip()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            string ClientIP = "";
            for (int i = 0; i < host.AddressList.Length; i++)
            {
                if (host.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    ClientIP = host.AddressList[i].ToString();
                }
            }
            return ClientIP;
        }

        private Boolean IsValidEmail(string strIn) // 이메일 정규식 검사.
        {
            return Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }
    }
}
