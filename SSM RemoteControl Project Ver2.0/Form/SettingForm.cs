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
using Microsoft.Win32;

namespace SSM_RemoteControl_Project_Ver2._0
{
    // 윈도우 시작시 자동으로 실행할 것인지 설정할 폼
    public partial class SettingForm : Form
    {
        private string appName = "SSM RemoteControl Project Ver2.0";
         
        private string filepath;
        private string db_autorun;
        private string db_topmost;
        private iniData ini_data;

        private string temp_path = @"C:\Atop\config.ini";
         
        public SettingForm()
        {
            init_setting(); 
            InitializeComponent();
            Check();
        }

        private void init_setting()
        {
            filepath = Path.Combine(Path.GetTempPath(), temp_path);
            ini_data = new iniData(filepath);
        }

        private void Check()
        {
            db_autorun = ini_data.GetIniValue("Remote Control System Information", "Is_AutoRun"); // 아이피 가져오기
            db_topmost = ini_data.GetIniValue("Remote Control System Information", "Is_TopMost"); // 아이피 가져오기

            if (db_autorun == "True")
            {
                cb_setting_auto.Checked = true;
            }
            else 
            {
                cb_setting_auto.Checked = false;
            } 
        }

        private void cb_setting_auto_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_setting_auto.Checked == true)
            {
                SetStartUp(appName, true);
            }
            else
            {
                SetStartUp(appName, false);
            }    
        }

        private void SetStartUp(string appName, bool enable)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

            if (enable) // 시작 프로그램 등록
            {
                if (registryKey.GetValue(appName) == null)
                {
                    registryKey.SetValue(appName, Application.ExecutablePath.ToString());
                    ini_data.SetIniValue("Remote Control System Information", "Is_AutoRun", "True"); 
                }
            }
            else // 시작 프로그램 해제
            {
                registryKey.DeleteValue(appName, false);
                ini_data.SetIniValue("Remote Control System Information", "Is_AutoRun", "False"); 
            }
        } 

        private bool CheckStartUp(string appName) // 등록 확인
        {
            string runKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
            Microsoft.Win32.RegistryKey startupKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(runKey);

            if (startupKey.GetValue(appName) == null)
                return false;
            else
                return true;
        }  
    }
}
