using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SSM_RemoteControl_Project_Ver2._0
{
    // 비밀번호를 변경하기 위한 폼
    public partial class ChangeForm : Form
    {
        private iniData ini_data;
        private string file_path;

        private string input_ip;
        private string input_email;
        private string input_pre_code;
        private string input_post_code;
        private string input_post_code2;

        private string db_ip;
        private string db_email;
        private string db_pre_code;
        private string db_post_code;
        private string db_post_code2;

        private AES256 aes;

        private string temp_path = @"C:\Atop\config.ini";

        public ChangeForm()
        { 
            InitializeComponent();
            aes = new AES256(); // 비밀번호 클래스 초기화
            file_path = Path.Combine(Path.GetTempPath(), temp_path);
            ini_data = new iniData(file_path); // 내부 경로 저장을 위해
        }

        private void btn_change_init_Click(object sender, EventArgs e)
        {
            tb_search_email.Text = "";
            tb_search_ip.Text = "";
            tb_search_pre_code.Text = "";
            tb_search_post_code.Text = "";
            tb_search_post_code2.Text = "";
        }

        private void btn_change_change_Click(object sender, EventArgs e) // 변경 버튼 클릭
        {
            // 사용자가 입력한 정보를 들고 옴
            input_ip = tb_search_ip.Text.ToString();
            input_email = tb_search_email.Text.ToString();
            input_pre_code = tb_search_pre_code.Text.ToString();
            input_post_code = tb_search_post_code.Text.ToString();
            input_post_code2 = tb_search_post_code2.Text.ToString(); 

            db_ip = ini_data.GetIniValue("Remote Control System Information", "UserIp"); // 아이피 가져오기
            db_email = ini_data.GetIniValue("Remote Control System Information", "UserEmail"); // 이메일
            db_pre_code = aes.AES_Decode(ini_data.GetIniValue("Remote Control System Information", "UserCode")); // 코드 가져오기

            if (input_ip.Equals(db_ip) && input_email.Equals(db_email) && input_pre_code.Equals(db_pre_code)) // 아이피, 이메일, 이전 코드가 DB 정보와 일치해야 함.
            {
                if (input_post_code.Equals(input_post_code2)) // 재 입력한 비밀번호가 일치해야 함.
                { 
                    ini_data.SetIniValue("Remote Control System Information", "Usercode", aes.AES_Encode(input_post_code2));
                    MessageBox.Show("Change you Cert code", "비밀번호 변경 완료.");
                    this.Hide();
                }
                else // 재 입력한 비밀번호가 다를 때
                {
                    MessageBox.Show("변경 비밀번호를 확인 해 주세요.", "Check post code");
                }
            }
            else // 사용자가 입력한 정보와 다를 때
            {
                MessageBox.Show("아이피와 비밀번호를 확인 해 주세요.", "Check Information");
            }
        }
    }
}
