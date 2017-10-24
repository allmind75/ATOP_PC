using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SSM_RemoteControl_Project_Ver2._0
{
    static class Program
    { 
        [STAThread]
        static void Main()
        {
            string file_path;
            FileInfo file_info;
            iniData ini_data;

            string temp_path = @"C:\Atop\config.ini"; // 설정 저장 경로

            string db_ip, db_code, db_join, db_mail;

            Boolean is_join = false;

            AES256 aes = new AES256();
            bool no_instance;
            Mutex mutext = new Mutex(true, "TestForBlockDuplicatedExecution", out no_instance);

            if (no_instance)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                #region 인증코드 최초 가입 확인
                file_path = Path.Combine(Path.GetTempPath(), temp_path);

                ini_data = new iniData(file_path);

                file_info = new FileInfo(file_path);

                if (file_info.Exists) // 우선 ini 파일이 존재해야 한다.
                {
                    db_ip = ini_data.GetIniValue("Remote Control System Information", "UserIp"); // 아이피 가져오기
                    db_code = aes.AES_Decode(ini_data.GetIniValue("Remote Control System Information", "UserCode")); // 코드 확보 해 놓기.
                    db_join = ini_data.GetIniValue("Remote Control System Information", "UserIsJoin"); // 코드 확보 해 놓기.
                    db_mail = ini_data.GetIniValue("Remote Control System Information", "UserEMail"); // 코드 확보 해 놓기.  

                    if (db_join.Equals("joined")) // 가입 되어 있으면 로그인 창 뜸.
                    { 
                        is_join = true;
                    }
                    else // 가입 되어 있지 않으면 가입창 뜸
                    {
                        is_join = false;
                    }
                }
                #endregion 

                try
                {
                    if (is_join == true) // 가입 되어 있으면 메인 폼 열고
                    { 
                        Application.Run(new MainForm());
                    }
                    else if (is_join == false) // 가입되어 있지 않으면 가입 폼을 연다.
                    {
                        Application.Run(new JoinForm());
                    }
                }
                catch (Exception e)
                { 

                }
            }
            else // 프로그램이 이미 실행 중
            {
                MessageBox.Show("이미 실행 중입니다.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }   
        } 
    }
}
