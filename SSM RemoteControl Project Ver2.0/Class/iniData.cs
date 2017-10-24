using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SSM_RemoteControl_Project_Ver2._0
{
    class iniData
    {
        [DllImport("kernel32.dll")] // GetIniValue 를 위해
        private static extern int GetPrivateProfileString(String section, String key, String def, StringBuilder retVal, int size, String filePath); 

        [DllImport("kernel32.dll")] // SetIniValue를 위해
        private static extern long WritePrivateProfileString(String section, String key, String val, String filePath);

        private String iniPath;

        public iniData(String path)
        {
            this.iniPath = path;
        } 

        public String GetIniValue(String Section, String Key) // ini 값을 읽어 온다. 
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, iniPath);
            return temp.ToString();
        }

        public void SetIniValue(String Section, String Key, String Value) // ini 값을 셋팅
        {  
            WritePrivateProfileString(Section, Key, Value, iniPath);
        }

    }
}
