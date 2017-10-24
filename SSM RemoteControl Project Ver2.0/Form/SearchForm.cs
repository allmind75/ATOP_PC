using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SSM_RemoteControl_Project_Ver2._0
{
    // 인증코드를 찾기 위한 폼
    public partial class SearchForm : Form
    { 
        private string input_ip;
        private string input_email;
        private iniData ini_data;

        private string db_ip;
        private string db_email;
        private string db_code;

        private string cb_user_mail;
        private string cb_temp;

        private MailMessage message; // 메일 보내기
        private SmtpClient smtp;
        private string send; // 보내는 사람
        private string senderName = "박현상";
        private string receive; // 받는 사람
        private string subject = "Remote Control 애플리케이션 Cert Code 발급입니다.";
        private string body;

        private string file_path;
        private string temp_path = @"C:\Atop\config.ini";

        private AES256 aes;

        public SearchForm()
        {
            aes = new AES256();
            InitializeComponent();
            file_path = Path.Combine(Path.GetTempPath(), temp_path);
            ini_data = new iniData(file_path);
        }

        private void btn_search_send_Click(object sender, EventArgs e)
        {
            if (tb_search_email.Text != "" || tb_search_ip.Text != "")  
            {
                try
                {
                    cb_user_mail = cb_mail.SelectedItem.ToString(); // 메일에 선택된 주소
                }
                catch(Exception ex) 
                {
                    cb_user_mail = cb_mail.Text.ToString();
                }

                input_ip = tb_search_ip.Text.ToString(); 
                input_email = tb_search_email.Text.ToString()+"@"+cb_user_mail; 

                db_ip = ini_data.GetIniValue("Remote Control System Information", "UserIp"); // 아이피 가져오기
                db_email = ini_data.GetIniValue("Remote Control System Information", "UserEMail"); // 아이피 가져오기  

                if (input_ip.Equals(db_ip) && input_email.Equals(db_email)) // 모든 정보가 맞아야
                {
                    db_code = aes.AES_Decode(ini_data.GetIniValue("Remote Control System Information", "UserCode")); // 코드 가져오기. 가장 중요한 부분.

                    message = new MailMessage(); 
                    message.From = new MailAddress("imgosari@naver.com"); // 보내는 사람
                    message.To.Add(input_email); // 받는 사람W

                    message.BodyEncoding = System.Text.Encoding.UTF8;
                    message.SubjectEncoding = System.Text.Encoding.UTF8;

                    message.Subject = subject;
                    message.Body = db_code;

                    //message.Priority = MailPriority.High; // 긴급으로 전송할 때

                    //전자 메일 메시지 본문의 형식이 Html인지 여부를 나타내는 값을 가져오거나 설정합니다
                    message.IsBodyHtml = true;

                    //응용 프로그램에서 SMTP(Simple Mail Transfer Protocol)를 사용하여 전자 메일을 보낼 수 있도록 합니다.
                    SmtpClient client = new SmtpClient("smtp.naver.com", 587);

                    //요청에 DefaultCredentials를 보낼지 여부를 제어하는 Boolean 값을 가져오거나 설정합니다.
                    client.UseDefaultCredentials = false;
                    //SmtpClient 에서 SSL(Secure Sockets Layer)을 사용하여 연결을 암호화할지 여부를 지정합니다.
                    client.EnableSsl = true;
                    //보낸 사람을 인증하는 데 사용되는 자격 증명을 가져오거나 설정합니다.
                    client.Credentials = new System.Net.NetworkCredential("imgosari", "ghatwoi2!");

                    try
                    {
                        client.Send(message);
                        MessageBox.Show("인증코드를 메일로 전송했습니다.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("인증코드 전송에 실패하였습니다.\n"+ex.Message); 
                    }

                    this.Close();
                }
                else
                {
                    MessageBox.Show("입력한 정보를 확인 해 주세요.");
                }
            }
        }
    }
}
