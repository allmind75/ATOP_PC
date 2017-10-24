using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SSM_RemoteControl_Project_Ver2._0
{
    // 사용자 컴퓨터의 아이피, 맥 어드레스를 나타내주는 폼
    public partial class DataForm : Form
    {  
        public DataForm()
        { 
            InitializeComponent();  

            label_data_ip.Text = Show_IP_Address().ToString();
            label_data_mac.Text = Show_Mac_Address().ToString(); 
        } 

        private string Show_Mac_Address()
        {
            string MacAddress = "";

            // 현재 연결된 네트워크 장비의 정보를 들고 옴
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface adapter in adapters)
            {
                System.Net.NetworkInformation.PhysicalAddress pa = adapter.GetPhysicalAddress();

                if (pa != null && !pa.ToString().Equals(""))
                {
                    MacAddress = pa.ToString(); // 맥 어드레스 출력 해 줌
                    break;
                } 
            } 
            return MacAddress;
        }

        private string Show_IP_Address()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            string myip = string.Empty;
            foreach (IPAddress ia in host.AddressList)
            {
                if (ia.AddressFamily == AddressFamily.InterNetwork)
                {
                    myip = ia.ToString(); break;
                }
            }
            return myip;
        }


    }
}
