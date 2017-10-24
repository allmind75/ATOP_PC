using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace SSM_RemoteControl_Project_Ver2._0
{
    public partial class MainForm : Form
    {
        private Socket server_socket = null;
        private Socket client_socket = null;
        private Socket file_server_socket = null;
        private Socket file_client_socket = null;
        private Socket mouse_udp_socket = null;
        private Socket capture_server_socket = null;
        private Socket capture_client_socket = null;
        private Socket udp_capture_socket = null;

        private byte[] receive_buffer = new byte[1024];
        private byte[] receive_file_buffer = new byte[1024];
        private byte[] send_file_buffer = new byte[4096];
        private byte[] send_buffer = new byte[1024];
        private byte[] capture_receive_buffer = new byte[1024];
        private byte[] slice_buffer = new byte[7168]; // tcp에서 사이즈 자르는거
        private byte[] udp_receive_buffer = new byte[1024];
        private byte[] mouse_buffer; // 마우스 좌표 수신용 버퍼
        private byte[] check_buffer;
        private byte[] temp_buffer;
        private byte[] capture_byte_buffer;
        private byte[] temp_byte;

        private int cursorX = 0;
        private int cursorY = 0;
        private int drag_x = 0;
        private int drag_y = 0;

        private int pre_capture_x = 0, pre_capture_y = 0;

        private int current_size_x, current_size_y, min_size_x, min_size_y, max_size_x, max_size_y;
        private int mouse_x, mouse_y;

        private int default_resolution_width, default_resolution_height; // 해상도 변경을 위한 기본 사이즈

        private IPEndPoint keyboard_end_point = null;
        private IPEndPoint capture_end_point = null;
        private IPEndPoint file_end_point = null;
        private EndPoint mouse_end_point = null;

        private IPEndPoint udp_capture_local_point = null;
        //private EndPoint close_check_point = null;
        private string local_ip = null;
        private IPAddress address;

        private MemoryStream ms;
        private NetworkStream ns;
        private StreamWriter sw;

        private NetworkStream file_ns;
        private StreamWriter file_sw;

        private NetworkStream capture_ns;
        private StreamWriter capture_sw;

        private Boolean is_connect = false;
        private Boolean is_file_connect = false;
        private Boolean is_server_push = false;
        private Boolean is_data_packet = false;
        private Boolean is_length_packet = false;
        private Boolean is_restart = false;
        private Boolean is_capturable = true;
        private Boolean is_rotate = false;

        // 파일 송,수신
        private Boolean is_file_connect_start = false;

        private Boolean is_shift_drag = false;
        private Boolean is_zoom_limit = false;

        private Boolean is_hangul = false;

        private Boolean is_resolution = false; // 해상도를 변경하는데, 소켓이 연결된 적이 있으면 true;

        private Boolean is_first_stop = false;
        private Boolean is_start = false;

        private System.Drawing.Point pt;

        private string[] fileNames;
        private string strFileSize;
        private string filePath;

        private FileStream fstream;
        private FileInfo fInfo;

        private Size sz;
        private Bitmap bitmap = null;
        private Bitmap cursorBMP;
        private Bitmap desktopBMP;
        private Rectangle cursorR;
        private Graphics g = null;
        private Rectangle r;

        private Thread receive_thread = null;
        private Thread connect_thread = null;

        private Thread capture_thread = null;
        private Thread function_thread = null;

        private Thread file_connect_thread = null;
        private Thread file_receive_thread = null;

        private Thread capture_receive_thread = null;

        private string receive_msg = "";
        private string receive_file_msg = "";
        private string capture_receive_msg = "";

        private int buffer_size = 0;
        private int file_buffer_size = 0;
        private int capture_buffer_size = 0;
        private int capture_size = 0;
        private long compress_size = 20L; // 0이면 가장 큰 압축, 100이면 가장 작은 압축
        private System.Drawing.Imaging.Encoder myEncoder;

        private String capture_string_size = "";
        private String capture_str = "";

        private JoinForm join_form;
        private SearchForm search_form;
        private ChangeForm change_form;
        private DataForm data_form;
        private SettingForm setting_form;

        //private System.Timers.Timer timer = new System.Timers.Timer();
        //delegate void TimerEventFiredDelegate();   

        private EncoderParameters myEncoderParameters;
        private EncoderParameter myEncoderParameter;
        private ImageCodecInfo pngEncoder;

        private AES256 aes;

        // 배경화면 바꾸기 위한 코드
        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDWININICHANGE = 0x02;

        private string darkPath;
        private string defaultPath = ""; // 가장 기초의 배경화면.  

        private const byte Clickkey = 0x2C;
        private const int KEYUP = 0x0002;
        private int Info = 0;
        private int i = 0;
        private int speed = 2; // 마우스 감도

        private string[] saSep = new string[] { "\r\n" }; // 스플릿으로 문자 자르기 위해
        private string[] devideSplit = new string[] { "$04" };
        private string[] mouseSplit = new string[] { "/" };
        private string[] saResult;

        private Point mouseOffset; // 폼 이동을 위해
        private bool isMouseDown = false;

        private Bitmap baseBitmap;
        private GraphicsUnit gUnit;
        private RectangleF rectF;
        private Color key;
        private int xMax;
        private int yMax;
        private GraphicsPath graphicsPath;
        private Region temp_region;

        #region Keyboard_Const
        private const int WM_IME_CONTROL = 0x0283;
        private const int MOUSEEVENTF_WHEEL = 0x0800; // 휠 수직 스크롤
        private const int MOUSEEVENTF_HWHEEL = 0x01000; // 휠 수평 스크롤
        private const int L_BUTTONDOWN = 0x0002;
        private const int L_BUTTONUP = 0x0004;
        private const int L_BUTTONDCLICK = 0x203;
        private const int R_BUTTONDOWN = 0x0008;
        private const int R_BUTTONUP = 0x0010;
        private const int R_BUTTONDCLICK = 0x206;

        // 키보드 상수 정리.
        private const byte VK_BACKSPACE = 0x08; // Back Space
        private const byte VK_TAB = 0x09; // Tab
        private const byte VK_RETURN = 0x0D; // Enter

        private const byte VK_NUMLOCK = 0x90; // Num Lock
        private const byte VK_SCROLL = 0x91; // Scroll Lock

        private const byte VK_SHIFT = 0x10; // Shift
        private const byte VK_CONTROL = 0x11; // Control
        private const byte VK_MENU = 0x12; // Alt
        private const byte VK_PAUSE = 0x13; // Pause Break
        private const byte VK_LWIN = 0x5B; // 왼쪽 윈도우키

        private const byte VK_CAPITAL = 0x14; // Caps Lock
        private const byte VK_HANGUL = 0x15; // 한영키
        private const byte VK_HANJA = 0x19; // 한자키
        private const byte VK_ESCAPE = 0x1B; // Esc
        private const byte VK_SPACE = 0x20; // Space Bar

        private const byte VK_PRIOR = 0x21; // Page Up
        private const byte VK_NEXT = 0x22; // Page Down

        private const byte VK_END = 0x23; // End
        private const byte VK_HOME = 0x24; // Home

        private const byte VK_LEFT = 0x25; // Left
        private const byte VK_UP = 0x26; // Up
        private const byte VK_RIGHT = 0x27; // Right
        private const byte VK_DOWN = 0x28; // Down

        private const byte VK_SNAPSHOT = 0x2C; // PrtScr
        private const byte VK_INSERT = 0x2D; // Insert
        private const byte VK_DELETE = 0x2E; // Delete

        private const byte VK_0 = 0x30; // 0 key
        private const byte VK_1 = 0x31; // 1 key
        private const byte VK_2 = 0x32; // 2 key
        private const byte VK_3 = 0x33; // 3 key
        private const byte VK_4 = 0x34; // 4 key
        private const byte VK_5 = 0x35; // 5 key
        private const byte VK_6 = 0x36; // 6 key
        private const byte VK_7 = 0x37; // 7 key
        private const byte VK_8 = 0x38; // 8 key
        private const byte VK_9 = 0x39; // 9 key

        private const byte VK_A = 0x41; // A key
        private const byte VK_B = 0x42; // B key
        private const byte VK_C = 0x43; // C key
        private const byte VK_D = 0x44; // D key
        private const byte VK_E = 0x45; // E key
        private const byte VK_F = 0x46; // F key
        private const byte VK_G = 0x47; // G key
        private const byte VK_H = 0x48; // H key
        private const byte VK_I = 0x49; // I key
        private const byte VK_J = 0x4A; // J key
        private const byte VK_K = 0x4B; // K key
        private const byte VK_L = 0x4C; // L key
        private const byte VK_M = 0x4D; // M key
        private const byte VK_N = 0x4E; // N key
        private const byte VK_O = 0x4F; // O key
        private const byte VK_P = 0x50; // P key
        private const byte VK_Q = 0x51; // Q key
        private const byte VK_R = 0x52; // R key
        private const byte VK_S = 0x53; // S key
        private const byte VK_T = 0x54; // T key
        private const byte VK_U = 0x55; // U key
        private const byte VK_V = 0x56; // V key
        private const byte VK_W = 0x57; // W key
        private const byte VK_X = 0x58; // X key
        private const byte VK_Y = 0x59; // Y key
        private const byte VK_Z = 0x5A; // Z key  

        private const byte VK_F1 = 0x70; // F1
        private const byte VK_F2 = 0x71; // F2
        private const byte VK_F3 = 0x72; // F3
        private const byte VK_F4 = 0x73; // F4
        private const byte VK_F5 = 0x74; // F5
        private const byte VK_F6 = 0x75; // F6
        private const byte VK_F7 = 0x76; // F7
        private const byte VK_F8 = 0x77; // F8
        private const byte VK_F9 = 0x78; // F9
        private const byte VK_F10 = 0x79; // F10
        private const byte VK_F11 = 0x7A; // F11
        private const byte VK_F12 = 0x7B; // F12 

        private const byte VK_MINUS = 0xBD; // -
        private const byte VK_SEMICOLON = 0xBA; // ;
        private const byte VK_COMMA = 0xBC; // ,
        private const byte VK_EQUALS = 0xBB; // =
        private const byte VK_DOT = 0xBE; // .
        private const byte VK_SLASH = 0xBF; // "/"
        private const byte VK_ACCENT = 0xC0; // `
        private const byte VK_SQUARE_OPEN = 0xDB; // [
        private const byte VK_SQUARE_CLOSE = 0xDD; // ]
        private const byte VK_WON = 0xDC; // \
        private const byte VK_APOSTROPHE = 0xDE; // ' 
        #endregion

        private const int IDC_CROSS = 32515;

        private static LowLevelMouseProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        public delegate void PointFunc(int x, int y);
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        public static event PointFunc LeftMouseDown;
        public static event PointFunc MouseMover;
        public static event PointFunc LeftMouseUp;
        public static event PointFunc WheelClick;
        public static event PointFunc RightMouseDown;
        public static event PointFunc RightMouseUp;

        private IntPtr hWnd, hImc; // 윈도우 핸들의 갯수  
        private POINTAPI current_position;

        private string check_string = "";
        private string temp_string = "";
        private int exit_cnt = 0;

        public struct POINTAPI
        {
            public int x;
            public int y;
        };

        public struct SIZE
        {
            public int cx;
            public int cy;
        }

        #region Class_Fuction

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("imm32.dll")]
        public static extern IntPtr ImmGetContext(IntPtr hWnd);

        [DllImport("imm32.dll")]
        public static extern IntPtr ImmGetDefaultIMEWnd(IntPtr hwnd);

        [DllImport("imm32.dll")]
        public static extern bool ImmGetConversionStatus(IntPtr hImc, out int lpConversion, out int lpSentence);

        [DllImport("imm32.dll", CharSet = CharSet.Auto)]
        public static extern bool ImmSetConversionStatus(IntPtr himc, int dw1, int dw2);

        [DllImport("imm32.dll")]
        public static extern int ImmReleaseContext(IntPtr hwnd, IntPtr himc); // 윈도우 핸들 해제하기

        [DllImport("user32.dll")]
        public static extern int FindWindow(string lpClassName, string lpWindowName); // 윈도우의 핸들을 얻기

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        public static extern int GetCursorPos(ref POINTAPI lpPoint);

        [DllImport("user32.dll", CharSet = CharSet.Auto)] // user32.dll 추가하기.
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte vk, byte scan, int flags, ref int extrainfo);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern short GetKeyState(int keyCode);

        [DllImport("user32.dll")]
        static extern bool SetSystemCursor(IntPtr hcur, uint id);

        [DllImport("user32.dll")]
        public static extern int SetSystemCursor(int hcur, int id);

        [DllImport("user32.dll")]
        static extern IntPtr LoadCursorFromFile(string lpFileName);

        [DllImport("user32.dll")]
        private static extern int CreateCursor(IntPtr hInstance, int nXhotspot, int nYhotspot, int nWidth, int nHeight, byte[] lpANDbitPlane, byte[] lpXORbitPlane);

        private static UInt32 SPI_SETCURSORS = 87;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SystemParametersInfo(UInt32 uiAction, UInt32 uiParam, String pvParam, UInt32 fWinIni);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern static bool DestroyIcon(IntPtr handle);

        [DllImport("user32.dll")]
        public static extern int SetCursor(int hCursor); 

        #endregion

        // 회원 가입 체크 및 로그인 부분
        #region 회원가입
        private string file_path;
        private FileInfo file_info;
        private iniData ini_data;

        private string db_ip = "";
        private string db_mail = "";
        private string db_code = "";
        private string db_join = ""; // 조인이 되어 있는가.

        public Boolean is_join = false;
        private Boolean is_login = false;

        private Socket login_server_socket, login_socket;
        private IPEndPoint login_end;

        private Thread login_connect_thread = null;
        private Thread login_receive_thread = null;

        private int login_buffer_size;
        private String login_receive_msg;
        private byte[] login_receive_buffer = new byte[1024];

        private Boolean login_is_connect = false;

        private NetworkStream login_ns;
        private StreamWriter login_sw;
        #endregion

        // 마우스 캡쳐 부분 

        [DllImport("user32.dll")]
        static extern bool DrawIcon(IntPtr hDC, int X, int Y, IntPtr InthIcon);

        private string temp_path = @"C:\Atop\config.ini";

        // main method 
        public MainForm()
        {
            InitializeComponent();

            this.Region = Converting();
            this.AllowDrop = true; // 드래그 앤 드랍 이벤트 활성화.  

            Tray.ContextMenuStrip = contextMenuStrip1; // 오른쪽 버튼 클릭시 나오는 메뉴 연결(종료) -> 연결이 된 후에 트레이 아이콘 등록

            CheckForIllegalCrossThreadCalls = false; // 크로스 스레드 작업문제 해결 

            Init_Setting();
            Save_Info();
        }

        public void Save_Info() // 정보 저장 해 놓기.
        {
            file_path = Path.Combine(Path.GetTempPath(), temp_path); 

            ini_data = new iniData(file_path); // 객체 생성
            file_info = new FileInfo(file_path);

            if (file_info.Exists) // 우선 ini 파일이 존재해야 한다.
            {
                db_ip = ini_data.GetIniValue("Remote Control System Information", "UserIp"); // 아이피 가져오기
                db_code = aes.AES_Decode(ini_data.GetIniValue("Remote Control System Information", "UserCode")); // 코드 확보 해 놓기.
                db_join = ini_data.GetIniValue("Remote Control System Information", "UserIsJoin"); // 코드 확보 해 놓기.
                db_mail = ini_data.GetIniValue("Remote Control System Information", "UserEMail"); // 코드 확보 해 놓기.     
            }
        }

        private void Set_Login_Socket()
        {
            login_end = new IPEndPoint(IPAddress.Any, 5000); // 로그인 포트 5000

            login_server_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // TCP 통신 구현을 위한 소켓 생성 

            login_server_socket.Bind(login_end); // 소켓을 종단점에 연결
            login_server_socket.Listen(5); // 최대 5대의 클라이언트 연결 허용   

            login_connect_thread.Start();
        }

        private void Make_Thread()
        {
            connect_thread = new Thread(Connect_Thread); // 서버소켓 쓰레드 생성  
            file_connect_thread = new Thread(File_Connect_Thread); // 파일 연결 쓰레드

            receive_thread = new Thread(Receive_Thread); // 메시지 수신 쓰레드 생성 
            file_receive_thread = new Thread(File_Receive_Thread); // 파일 수신 문자열 확인 쓰레드

            capture_thread = new Thread(Capture_Thread); // 캡쳐 쓰레드 생성   

            function_thread = new Thread(Check_Function_Key_Thread); // 한영 상태 확인 쓰레드.

            login_connect_thread = new Thread(Login_Connect_Thread); // 로그인 연결 스레드
            login_receive_thread = new Thread(Login_Receive_Thread); // 로그인 정보 수신 스레드 

            capture_receive_thread = new Thread(Capture_Receive_Thread);
        }

        public void Init_Setting()
        {
            Make_Thread(); // 스레드 객체 생성 메소드
            Set_Login_Socket();  // 로그인 소켓 생성

            aes = new AES256(); // AES256 객체
            ms = new MemoryStream(); // 메모리 스트림 생성   

            // 압축 파트.
            myEncoderParameters = new EncoderParameters(1);
            myEncoder = System.Drawing.Imaging.Encoder.Quality; // 압축 수준 지정
            pngEncoder = GetEncoder(ImageFormat.Jpeg); // jpeg 압축

            myEncoderParameter = new EncoderParameter(myEncoder, compress_size); // 0이면 가장 큰 압축, 100이면 가장 작은 압축
            myEncoderParameters.Param[0] = myEncoderParameter;

            current_position = new POINTAPI(); // 마우스 후킹.   

            default_resolution_width = Screen.PrimaryScreen.Bounds.Width; // 스크린 스크린 얻어 옴
            default_resolution_height = Screen.PrimaryScreen.Bounds.Height;

            max_size_x = mouse_x = current_size_x = 1024; // 사이즈 정함
            max_size_y = mouse_y = current_size_y = 768;

            // 최소 사이즈는 최대 사이즈의 1/3의 크기만 가짐
            min_size_x = max_size_x / 3;
            min_size_y = max_size_y / 3;

            sz = new Size(current_size_x, current_size_y); // 현재 폼 = this.Bounds.Width, this.Bounds.Height 

            defaultPath = GetPathOfWallpaper(); // 기본 바탕화면 경로를 알아옴.   
            MakeBlackBitmap(); // 바탕화면을 까맣게 만듦
        }

        public void Set_Socket() // 소켓 설정 - 클라이언트 연결 대기
        {
            if (is_server_push == false) // 연결 요청이 아직" 안되어 있어야 소켓 설정함.
            {
                try
                {
                    int port = 8000;
                    int udp_port = 9000;
                    mouse_buffer = new byte[60];
                    check_buffer = new byte[10];
                    temp_buffer = new byte[10];

                    is_server_push = true;

                    server_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // TCP 통신 구현을 위한 소켓 생성 
                    file_server_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    keyboard_end_point = new IPEndPoint(IPAddress.Any, port); // 종단점 설정   
                    file_end_point = new IPEndPoint(IPAddress.Any, 6000); // 파일 전송 포트 6000  

                    if (!is_restart) // 다이얼로그 중단 이후 재시작 요청 상태가 아니면.
                    {
                        mouse_udp_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                        capture_server_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        //udp_capture_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                        mouse_end_point = new IPEndPoint(IPAddress.Any, udp_port);
                        capture_end_point = new IPEndPoint(IPAddress.Any, 7000);

                        address = IPAddress.Parse(local_ip);  

                        file_server_socket.Bind(file_end_point);
                        file_server_socket.Listen(5);

                        server_socket.Bind(keyboard_end_point); // 소켓을 종단점에 연결
                        server_socket.Listen(5); // 최대 5대의 클라이언트 연결 허용   

                        capture_server_socket.Bind(capture_end_point);
                        capture_server_socket.Listen(5);

                        mouse_udp_socket.Bind(mouse_end_point);
                        //label_state.Text = "연결 대기 중";  
                    }
                }
                catch (SocketException socketexception)
                {
                    Console.WriteLine("서버 소켓 생성 실패.\n" + socketexception.Message);
                    Console.WriteLine("중단점 오류 " + socketexception.Message);
                }
            }
            else
            {
                Console.WriteLine("서버가 이미 활성화 되어 있습니다.");
            }
        }

        public void Server_Start()
        {
            Set_Socket();
            connect_thread.Start(); // 비동기 서버 소켓 생성    
            Udp_Mouse_Start();
        }

        private void Login_Connect_Thread()
        {
            while (login_is_connect == false)
            {
                login_socket = login_server_socket.Accept();

                login_ns = new NetworkStream(login_socket); // 네트워크 스트림
                login_sw = new StreamWriter(login_ns);

                login_server_socket.Blocking = false;

                login_is_connect = true;

                login_receive_thread.Start();
            }
        }

        private string getIP(Socket c) // 로컬 연결된 ip 정보를 가져옴.
        {
            IPEndPoint ip_point = (IPEndPoint)c.RemoteEndPoint;
            local_ip = ip_point.Address.ToString();
            Console.WriteLine("ip : " + local_ip);
            return local_ip;
        }

        private void Login_Receive_Thread()
        {
            while (login_is_connect)
            {
                try
                {
                    login_buffer_size = login_socket.Receive(login_receive_buffer); // 수신 메시지를 버퍼에 넣고 사이즈 파악  

                    if (login_buffer_size != 0) // 메시지가 0보다 크다면
                    {
                        login_receive_msg = Encoding.GetEncoding(65001).GetString(login_receive_buffer, 0, login_buffer_size); // 버퍼에 들어있는 내용을 string으로 변환.     
                        login_receive_msg = (aes.AES_Decode(login_receive_msg));

                        if (login_receive_msg.Equals("quit")) // 종료
                        {
                            Login_Close_Socket();
                        }
                        else if (login_receive_msg.Equals(db_code)) // 비밀번호가 맞으면 자동 로그인
                        {
                            Login_Send_Message("True");
                            getIP(login_socket); // 연결된 로컬과의 아이피를 찾아낸다.
                            Login();
                        }
                        else
                        {
                            Login_Send_Message("False");
                        }
                    }
                    else
                    {
                        login_receive_msg = "";
                    }
                }

                catch (NullReferenceException nullexception)
                {
                    Console.WriteLine("Null Reference 에러 : " + nullexception.Message);
                }

                catch (SocketException socketexceptione)
                {
                    Console.WriteLine("로그인 receive 소켓 에러 : " + socketexceptione.Message);
                }
            }
        }

        private void Login_Send_Message(string msg)
        {
            string temp_msg = Base64Encode(msg);
            login_sw.WriteLine(temp_msg);
            login_sw.Flush();
        }

        private void Login() // 자동 로그인
        {
            Server_Start();

            if (login_is_connect)
                Login_Close_Socket();
        }

        private void Login_Close_Socket()
        {
            login_is_connect = false;

            if (login_socket != null)
            {
                login_socket.Close();
                login_socket = null;
            }

            if (login_server_socket != null)
            {
                login_server_socket.Close();
                login_server_socket = null;
            }

            login_connect_thread.Abort();
            login_receive_thread.Abort();
        }

        public void Receive_Thread() // 메시지 수신 스레드.
        {
            while (is_connect == true)
            {
                try
                {
                    buffer_size = client_socket.Receive(receive_buffer); // 수신 메시지를 버퍼에 넣고 사이즈 파악  

                    if (buffer_size != 0) // 메시지가 0보다 크다면
                    {
                        receive_msg = Encoding.GetEncoding(65001).GetString(receive_buffer, 0, buffer_size); // 버퍼에 들어있는 내용을 string으로 변환.   

                        saResult = receive_msg.Split(saSep, StringSplitOptions.None); // \r\n 구분자로 패킷을 나눔. 

                        for (i = 0; i < saResult.Length; i++)
                        {
                            Search_Packet(saResult[i]);
                        }
                    }
                    else
                    {
                        receive_msg = "";
                    }
                }
                catch (NullReferenceException nullexception)
                {
                    Console.WriteLine("리시브 " + nullexception.Message);
                }
                catch (SocketException socket)
                {
                    if (socket.NativeErrorCode.Equals(10035))
                        Console.WriteLine("Still Connected, but the Send would block");
                    else
                    {
                        Console.WriteLine("Disconnected: error code {0}!", socket.NativeErrorCode);
                    }
                }
                catch (IOException ioexception)
                {
                    Console.WriteLine("ioexception " + ioexception);
                }
            }
        }

        public void Capture_Receive_Thread()
        {
            while (is_connect == true)
            {
                try
                {
                    capture_buffer_size = capture_client_socket.Receive(capture_receive_buffer);

                    if (capture_buffer_size != 0)
                    {
                        capture_receive_msg = Encoding.GetEncoding(65001).GetString(capture_receive_buffer, 0, capture_buffer_size);

                        if (capture_receive_msg.StartsWith("Size")) // 길이 수신 
                        {
                            is_length_packet = true;
                            capture_receive_msg = "";
                        }
                        if (capture_receive_msg.StartsWith("Data")) // 데이터를 수신
                        {
                            is_data_packet = true;
                            capture_receive_msg = "";
                        }
                    }
                }
                catch (NullReferenceException nullexception)
                {
                    MessageBox.Show("" + nullexception.Message);
                }
                catch (SocketException socketexceptione)
                {

                }
            }
        }

        private void File_Connect_Thread() // 사실 이건 한번만 돌아도 된다. 연결 될 때까지만 돌면 되기 때문에
        {
            while (is_file_connect_start == true && is_file_connect == false && is_connect == true) // 파일 수신 요청이 들어오면
            {
                file_client_socket = file_server_socket.Accept();
                file_server_socket.Blocking = false;

                file_ns = new NetworkStream(file_client_socket);
                file_sw = new StreamWriter(file_ns);

                is_file_connect = true;

                file_receive_thread.Start();
            }
        }

        public void File_Receive_Thread()
        {
            while (is_file_connect == true && is_connect == true)
            {
                file_buffer_size = file_client_socket.Receive(receive_file_buffer); // 에러

                if (file_buffer_size != 0)
                {
                    receive_file_msg = Encoding.GetEncoding(65001).GetString(receive_file_buffer, 0, file_buffer_size);

                    if (receive_file_msg.Equals("Open")) // 포트가 다 열렸으면
                    {
                        File_Name_Send(filePath);
                    }
                    else if (receive_file_msg.Equals("Size")) // 사이즈 보냄.
                    {
                        File_Size_Send();
                    }
                    else if (receive_file_msg.Equals("Ready")) // 파일 받을 준비가 다 되면
                    {
                        File_Send();
                    }

                    else if (receive_file_msg.Equals("End")) // "랑 같은지를 표현할 때 \"를 적으면 됨.
                    {
                        File_Socket_Close();
                    }
                }
            }
        }

        public void File_Size_Send() // 파일 사이즈 전송
        {
            byte[] sendSize = System.Text.Encoding.UTF8.GetBytes(strFileSize);
            file_ns.Write(sendSize, 0, sendSize.Length);
        }

        public void File_Name_Send(String msg) // 파일 이름 전송
        {
            String[] file_names;
            String file_name;
            Byte[] sendByte;

            try
            {
                file_names = msg.Split('\\'); // \로 나눔
                file_name = file_names[file_names.Length - 1];

                sendByte = System.Text.Encoding.UTF8.GetBytes(file_name);

                file_ns.Write(sendByte, 0, sendByte.Length);

                fstream = new FileStream(msg, FileMode.Open, FileAccess.Read);
            }
            catch (NullReferenceException nullexception)
            {
                Console.WriteLine("" + nullexception.Message);
            }
            catch (SocketException socketexception2)
            {
                Console.WriteLine("" + socketexception2.Message);
            }
        }

        public void File_Send() // 파일 전송 부분
        {
            int count = 0;

            try
            {
                while ((count = fstream.Read(send_file_buffer, 0, send_file_buffer.Length)) > 0) // 파일 스트림에서 파일을 읽어 옮
                {
                    file_ns.Write(send_file_buffer, 0, count); // 특정 사이즈 만큼 잘라서 전송함
                }

                MessageBox.Show("파일 전송 완료");
                filePath = "";
            }
            catch (IOException ioexception)
            {
                Console.WriteLine("Socket Exception");
            }
        }

        public void File_Socket_Close()
        {
            is_file_connect = false;
            is_file_connect_start = false;

            file_receive_thread.Abort();
            file_connect_thread.Abort();

            file_client_socket.Close();
            file_server_socket.Close();
        }

        public void Search_Packet(String temp_msg)
        {
            String receive_msg;

            // 한글, 영어, 특수문자, 기능키 4부분으로 함수를 나눔.
            if (temp_msg.StartsWith("한"))
            {
                receive_msg = temp_msg.Substring(1); // 2번쨰 문자부터 모두 추출
                Han_Packet(receive_msg);
            }
            else if (temp_msg.StartsWith("영")) 
            {
                receive_msg = temp_msg.Substring(1);
                Eng_Packet(receive_msg);
            }
            else if (temp_msg.StartsWith("특"))
            {
                receive_msg = temp_msg.Substring(1);
                Func_Packet(receive_msg);
            }
            else if (temp_msg.StartsWith("조"))
            {
                receive_msg = temp_msg.Substring(1);
                Sum_Packet(receive_msg);
            }
        }
         
        public void Han_Packet(String receive_msg)
        {
            if (receive_msg.Equals("ㄱ"))
            {
                if (Check_HANGUL() == false)
                    Make_HANGUL();
                Key_Press(VK_R);
                receive_msg = "";
            }
            else if (receive_msg.Equals("ㄴ"))
            {
                if (Check_HANGUL() == false)
                    Make_HANGUL();
                Key_Press(VK_S);
                receive_msg = "";
            }
            else if (receive_msg.Equals("ㄷ"))
            {
                if (Check_HANGUL() == false)
                    Make_HANGUL();
                Key_Press(VK_E);
                receive_msg = "";
            }
            else if (receive_msg.Equals("ㄹ"))
            {
                if (Check_HANGUL() == false)
                    Make_HANGUL();
                Key_Press(VK_F);
                receive_msg = "";
            }
            else if (receive_msg.Equals("ㅁ"))
            {
                if (Check_HANGUL() == false)
                    Make_HANGUL();
                Key_Press(VK_A);
                receive_msg = "";
            }
            else if (receive_msg.Equals("ㅂ"))
            {
                if (Check_HANGUL() == false)
                    Make_HANGUL();
                Key_Press(VK_Q);
                receive_msg = "";
            }
            else if (receive_msg.Equals("ㅅ"))
            {
                if (Check_HANGUL() == false)
                    Make_HANGUL();
                Key_Press(VK_T);
                receive_msg = "";
            }
            else if (receive_msg.Equals("ㅇ"))
            {
                if (Check_HANGUL() == false)
                    Make_HANGUL();
                Key_Press(VK_D);
                receive_msg = "";
            }
            else if (receive_msg.Equals("ㅈ"))
            {
                if (Check_HANGUL() == false)
                    Make_HANGUL();
                Key_Press(VK_W);
                receive_msg = "";
            }
            else if (receive_msg.Equals("ㅊ"))
            {
                if (Check_HANGUL() == false)
                    Make_HANGUL();
                Key_Press(VK_C);
                receive_msg = "";
            }
            else if (receive_msg.Equals("ㅋ"))
            {
                if (Check_HANGUL() == false)
                    Make_HANGUL();
                Key_Press(VK_Z);
                receive_msg = "";
            }
            else if (receive_msg.Equals("ㅌ"))
            {
                if (Check_HANGUL() == false)
                    Make_HANGUL();
                Key_Press(VK_X);
                receive_msg = "";
            }
            else if (receive_msg.Equals("ㅍ"))
            {
                if (Check_HANGUL() == false)
                    Make_HANGUL();
                Key_Press(VK_V);
                receive_msg = "";
            }
            else if (receive_msg.Equals("ㅎ"))
            {
                if (Check_HANGUL() == false)
                    Make_HANGUL();
                Key_Press(VK_G);
                receive_msg = "";
            }
            else if (receive_msg.Equals("ㅏ"))
            {
                if (Check_HANGUL() == false)
                    Make_HANGUL();
                Key_Press(VK_K);
                receive_msg = "";
            }
            else if (receive_msg.Equals("ㅑ"))
            {
                if (Check_HANGUL() == false)
                    Make_HANGUL();
                Key_Press(VK_I);
                receive_msg = "";
            }
            else if (receive_msg.Equals("ㅓ"))
            {
                if (Check_HANGUL() == false)
                    Make_HANGUL();
                Key_Press(VK_J);
                receive_msg = "";
            }
            else if (receive_msg.Equals("ㅕ"))
            {
                if (Check_HANGUL() == false)
                    Make_HANGUL();
                Key_Press(VK_U);
                receive_msg = "";
            }
            else if (receive_msg.Equals("ㅗ"))
            {
                if (Check_HANGUL() == false)
                    Make_HANGUL();
                Key_Press(VK_H);
                receive_msg = "";
            }
            else if (receive_msg.Equals("ㅛ"))
            {
                if (Check_HANGUL() == false)
                    Make_HANGUL();
                Key_Press(VK_Y);
                receive_msg = "";
            }
            else if (receive_msg.Equals("ㅜ"))
            {
                if (Check_HANGUL() == false)
                    Make_HANGUL();
                Key_Press(VK_N);
                receive_msg = "";
            }
            else if (receive_msg.Equals("ㅠ"))
            {
                if (Check_HANGUL() == false)
                    Make_HANGUL();
                Key_Press(VK_B);
                receive_msg = "";
            }
            else if (receive_msg.Equals("ㅡ"))
            {
                if (Check_HANGUL() == false)
                    Make_HANGUL();
                Key_Press(VK_M);
                receive_msg = "";
            }
            else if (receive_msg.Equals("ㅣ"))
            {
                if (Check_HANGUL() == false)
                    Make_HANGUL();
                Key_Press(VK_L);
                receive_msg = "";
            }
            else if (receive_msg.Equals("ㅔ"))
            {
                if (Check_HANGUL() == false)
                    Make_HANGUL();
                Key_Press(VK_P);
                receive_msg = "";
            }
            else if (receive_msg.Equals("ㅐ"))
            {
                if (Check_HANGUL() == false)
                    Make_HANGUL();
                Key_Press(VK_O);
                receive_msg = "";
            }
            else if (receive_msg.Equals("ㅒ"))
            {
                if (Check_HANGUL() == false)
                    Make_HANGUL();
                Key_Press(VK_O, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("ㅖ"))
            {
                if (Check_HANGUL() == false)
                    Make_HANGUL();
                Key_Press(VK_P, VK_SHIFT);
                receive_msg = "";
            }

            else if (receive_msg.Equals("ㅃ"))
            {
                if (Check_HANGUL() == false)
                    Make_HANGUL();
                Key_Press(VK_Q, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("ㅉ"))
            {
                if (Check_HANGUL() == false)
                    Make_HANGUL();
                Key_Press(VK_W, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("ㄸ"))
            {
                if (Check_HANGUL() == false)
                    Make_HANGUL();
                Key_Press(VK_E, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("ㄲ"))
            {
                if (Check_HANGUL() == false)
                    Make_HANGUL();
                Key_Press(VK_R, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("ㄲ"))
            {
                if (Check_HANGUL() == false)
                    Make_HANGUL();
                Key_Press(VK_R, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("ㅆ"))
            {
                if (Check_HANGUL() == false)
                    Make_HANGUL();
                Key_Press(VK_T, VK_SHIFT);
                receive_msg = "";
            }
        }

        public void Eng_Packet(String receive_msg)
        {
            if (receive_msg.Equals("a"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_A);
                receive_msg = "";
            }
            else if (receive_msg.Equals("b"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_B);
                receive_msg = "";
            }
            else if (receive_msg.Equals("c"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_C);
                receive_msg = "";
            }
            else if (receive_msg.Equals("d"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_D);
                receive_msg = "";
            }
            else if (receive_msg.Equals("e"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_E);
                receive_msg = "";
            }
            else if (receive_msg.Equals("f"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_F);
                receive_msg = "";
            }
            else if (receive_msg.Equals("g"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_G);
                receive_msg = "";
            }
            else if (receive_msg.Equals("h"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_H);
                receive_msg = "";
            }
            else if (receive_msg.Equals("i"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_I);
                receive_msg = "";
            }
            else if (receive_msg.Equals("j"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_J);
                receive_msg = "";
            }
            else if (receive_msg.Equals("k"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_K);
                receive_msg = "";
            }
            else if (receive_msg.Equals("l"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_L);
                receive_msg = "";
            }
            else if (receive_msg.Equals("m"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_M);
                receive_msg = "";
            }
            else if (receive_msg.Equals("n"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_N);
                receive_msg = "";
            }
            else if (receive_msg.Equals("o"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_O);
                receive_msg = "";
            }
            else if (receive_msg.Equals("p"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_P);
                receive_msg = "";
            }
            else if (receive_msg.Equals("q"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_Q);
                receive_msg = "";
            }
            else if (receive_msg.Equals("r"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_R);
                receive_msg = "";
            }
            else if (receive_msg.Equals("s"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_S);
                receive_msg = "";
            }
            else if (receive_msg.Equals("t"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_T);
                receive_msg = "";
            }
            else if (receive_msg.Equals("u"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_U);
                receive_msg = "";
            }
            else if (receive_msg.Equals("v"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_V);
                receive_msg = "";
            }
            else if (receive_msg.Equals("w"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_W);
                receive_msg = "";
            }
            else if (receive_msg.Equals("x"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_X);
                receive_msg = "";
            }
            else if (receive_msg.Equals("y"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_Y);
                receive_msg = "";
            }
            else if (receive_msg.Equals("z"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_Z);
                receive_msg = "";
            }
            else if (receive_msg.Equals("A"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_A, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("B"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_B, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("C"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_C, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("D"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_D, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("E"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_E, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("F"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_F, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("G"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_G, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("H"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_H, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("I"))
            {
                if (Check_HANGUL() == false)
                    Make_ENGLISH();
                Key_Press(VK_I, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("J"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_J, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("K"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_K, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("L"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_L, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("M"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_M, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("N"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_N, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("O"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_O, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("P"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_P, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("Q"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_Q, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("R"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_R, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("S"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_S, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("T"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_T, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("U"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_U, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("V"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_V, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("W"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_W, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("X"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_X, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("Y"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_Y, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("Z"))
            {
                if (Check_HANGUL() == true)
                    Make_ENGLISH();
                Key_Press(VK_Z, VK_SHIFT);
                receive_msg = "";
            }
        }

        public void Func_Packet(String receive_msg)
        {
            if (receive_msg.Equals("Click_on")) // 밑에 마우스 클릭으로, 드래그를 시작하는 것임
            {
                Drag_On();
            }
            else if (receive_msg.Equals("Click_off")) // 드래그를 종료하는 의미임
            {
                Drag_Off();
            }
            else if (receive_msg.Equals("Scroll_up")) // 드래그를 종료하는 의미임
            {
                Scroll_Up();
            }
            else if (receive_msg.Equals("Scroll_down")) // 드래그를 종료하는 의미임
            {
                Scroll_Down();
            }
            else if (receive_msg.Equals("Scroll_left")) // 드래그를 종료하는 의미임
            {
                Scroll_Left();
            }
            else if (receive_msg.Equals("Scroll_right")) // 드래그를 종료하는 의미임
            {
                Scroll_Right();
            }
            else if (receive_msg.Equals("Shift_on")) // Shift_on 되면
            {
                Shift_On(VK_SHIFT);
                receive_msg = "";
                //label_shift.Text = "On";
            }
            else if (receive_msg.Equals("Shift_off")) // Shift_on 되면
            {
                Shift_Off(VK_SHIFT);
                receive_msg = "";
                //label_shift.Text = "Off";
            }
            else if (receive_msg.Equals("Ctrl_on")) // Control 눌러지면
            {
                Ctrl_On(VK_CONTROL);
                receive_msg = "";
                //label_control.Text = "On";
            }
            else if (receive_msg.Equals("Ctrl_off")) // Control 눌러지면
            {
                Ctrl_Off(VK_CONTROL);
                receive_msg = "";
                //label_control.Text = "Off";
            }
            else if (receive_msg.Equals("Rclick"))
            {
                RClick();
            }
            else if (receive_msg.Equals("Copy"))
            {
                Copy();
            }
            else if (receive_msg.Equals("Paste"))
            {
                Paste();
            }
            else if (receive_msg.Equals("zoom_in"))
            {
                Capture_Zoom_In();
            }
            else if (receive_msg.Equals("zoom_out"))
            {
                Capture_Zoom_Out();
            }
            else if (receive_msg.Equals("Alt"))
            {
                Key_Press(VK_MENU);
                receive_msg = "";
            }
            else if (receive_msg.Equals("한/영")) // 한/영 눌러지면
            {
                Key_Press(VK_HANGUL);
                receive_msg = "";
            }
            else if (receive_msg.Equals("Ent")) // Enter가 눌러지면
            {
                Key_Press(VK_RETURN);
                receive_msg = "";
            }
            else if (receive_msg.Equals("Space")) // 스페이스 바 눌러지면
            {
                Key_Press(VK_SPACE);
                receive_msg = "";
            }
            else if (receive_msg.Equals("ESC")) // Esc가 눌러지면
            {
                Key_Press(VK_ESCAPE);
                receive_msg = "";
            }
            else if (receive_msg.Equals("Home")) // Home이 눌러지면
            {
                Key_Press(VK_HOME);
                receive_msg = "";
            }
            else if (receive_msg.Equals("End")) // End가 눌러지면
            {
                Key_Press(VK_END);
                receive_msg = "";
            }
            else if (receive_msg.Equals("ScrLK")) // Scroll Lock이 눌러지면
            {
                Key_Press(VK_SCROLL);
                receive_msg = "";
            }
            else if (receive_msg.Equals("PrtSc")) // PrtSc이 눌러지면
            {
                Key_Press(VK_SNAPSHOT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("Insert")) // Insert이 눌러지면
            {
                Key_Press(VK_INSERT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("Del")) // Del이 눌러지면
            {
                Key_Press(VK_DELETE);
                receive_msg = "";
            }
            else if (receive_msg.Equals("Pause")) // Pause 눌러지면
            {
                Key_Press(VK_PAUSE);
                receive_msg = "";
            }
            else if (receive_msg.Equals("WIN")) // Window 눌러지면
            {
                Key_Press(VK_LWIN);
                receive_msg = "";
            }
            else if (receive_msg.Equals("한자")) // 한자 눌러지면
            {
                Key_Press(VK_HANJA);
                receive_msg = "";
            }
            else if (receive_msg.Equals("PgUp")) // 페이지 업 눌러지면
            {
                Key_Press(VK_PRIOR);
                receive_msg = "";
            }
            else if (receive_msg.Equals("PgDn")) // 페이지 다운 눌러지면
            {
                Key_Press(VK_NEXT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("Up")) // ↑ 눌러지면
            {
                Key_Press(VK_UP);
                receive_msg = "";
            }
            else if (receive_msg.Equals("Down")) // ↓ 눌러지면
            {
                Key_Press(VK_DOWN);
                receive_msg = "";
            }
            else if (receive_msg.Equals("Left")) // ← 눌러지면
            {
                Key_Press(VK_LEFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("Right")) // → 눌러지면
            {
                Key_Press(VK_RIGHT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("Back")) // backspace가 눌러지면
            {
                Key_Press(VK_BACKSPACE);
                receive_msg = "";
            }
            else if (receive_msg.Equals("Tab")) // tab이 눌러지면
            {
                Key_Press(VK_TAB);
                receive_msg = "";
            }
            else if (receive_msg.Equals("0")) // 0이 눌러지면
            {
                Key_Press(VK_0);
                receive_msg = "";
            }
            else if (receive_msg.Equals("1")) // 1이 눌러지면
            {
                Key_Press(VK_1);
                receive_msg = "";
            }
            else if (receive_msg.Equals("2")) // 2이 눌러지면
            {
                Key_Press(VK_2);
                receive_msg = "";
            }
            else if (receive_msg.Equals("3")) // 3이 눌러지면
            {
                Key_Press(VK_3);
                receive_msg = "";
            }
            else if (receive_msg.Equals("4")) // 4이 눌러지면
            {
                Key_Press(VK_4);
                receive_msg = "";
            }
            else if (receive_msg.Equals("5")) // 5이 눌러지면
            {
                Key_Press(VK_5);
                receive_msg = "";
            }
            else if (receive_msg.Equals("6")) // 6이 눌러지면
            {
                Key_Press(VK_6);
                receive_msg = "";
            }
            else if (receive_msg.Equals("7")) // 7이 눌러지면
            {
                Key_Press(VK_7);
                receive_msg = "";
            }
            else if (receive_msg.Equals("8")) // 8이 눌러지면
            {
                Key_Press(VK_8);
                receive_msg = "";
            }
            else if (receive_msg.Equals("9")) // 9가 눌러지면
            {
                Key_Press(VK_9);
                receive_msg = "";
            }
            else if (receive_msg.Equals("F1"))
            {
                Key_Press(VK_F1);
                receive_msg = "";
            }
            else if (receive_msg.Equals("F2"))
            {
                Key_Press(VK_F2);
                receive_msg = "";
            }
            else if (receive_msg.Equals("F3"))
            {
                Key_Press(VK_F3);
                receive_msg = "";
            }
            else if (receive_msg.Equals("F4"))
            {
                Key_Press(VK_F4);
                receive_msg = "";
            }
            else if (receive_msg.Equals("F5"))
            {
                Key_Press(VK_F5);
                receive_msg = "";
            }
            else if (receive_msg.Equals("F6"))
            {
                Key_Press(VK_F6);
                receive_msg = "";
            }
            else if (receive_msg.Equals("F7"))
            {
                Key_Press(VK_F7);
                receive_msg = "";
            }
            else if (receive_msg.Equals("F8"))
            {
                Key_Press(VK_F8);
                receive_msg = "";
            }
            else if (receive_msg.Equals("F9"))
            {
                Key_Press(VK_F9);
                receive_msg = "";
            }
            else if (receive_msg.Equals("F10"))
            {
                Key_Press(VK_F10);
                receive_msg = "";
            }
            else if (receive_msg.Equals("F11"))
            {
                Key_Press(VK_F11);
                receive_msg = "";
            }
            else if (receive_msg.Equals("F12"))
            {
                Key_Press(VK_F12);
                receive_msg = "";
            }
            else if (receive_msg.Equals("!"))
            {
                Key_Press(VK_1, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("@"))
            {
                Key_Press(VK_2, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("#"))
            {
                Key_Press(VK_3, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("$"))
            {
                Key_Press(VK_4, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("%"))
            {
                Key_Press(VK_5, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("^"))
            {
                Key_Press(VK_6, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("&"))
            {
                Key_Press(VK_7, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("*"))
            {
                Key_Press(VK_8, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("("))
            {
                Key_Press(VK_9, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals(")"))
            {
                Key_Press(VK_0, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("-"))
            {
                Key_Press(VK_MINUS);
                receive_msg = "";
            }
            else if (receive_msg.Equals("_"))
            {
                Key_Press(VK_MINUS, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals(";"))
            {
                Key_Press(VK_SEMICOLON);
                receive_msg = "";
            }
            else if (receive_msg.Equals(":"))
            {
                Key_Press(VK_SEMICOLON, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals(","))
            {
                Key_Press(VK_COMMA);
                receive_msg = "";
            }
            else if (receive_msg.Equals("<"))
            {
                Key_Press(VK_COMMA, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("."))
            {
                Key_Press(VK_DOT);
                receive_msg = "";
            }
            else if (receive_msg.Equals(">"))
            {
                Key_Press(VK_DOT, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("="))
            {
                Key_Press(VK_EQUALS);
                receive_msg = "";
            }
            else if (receive_msg.Equals("+"))
            {
                Key_Press(VK_EQUALS, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("/"))
            {
                Key_Press(VK_SLASH);
                receive_msg = "";
            }
            else if (receive_msg.Equals("?"))
            {
                Key_Press(VK_SLASH, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("`"))
            {
                Key_Press(VK_ACCENT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("~"))
            {
                Key_Press(VK_ACCENT, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("["))
            {
                Key_Press(VK_SQUARE_OPEN);
                receive_msg = "";
            }
            else if (receive_msg.Equals("{"))
            {
                Key_Press(VK_SQUARE_OPEN, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("]"))
            {
                Key_Press(VK_SQUARE_CLOSE);
                receive_msg = "";
            }
            else if (receive_msg.Equals("}"))
            {
                Key_Press(VK_SQUARE_CLOSE, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("\\"))
            {
                Key_Press(VK_WON);
                receive_msg = "";
            }
            else if (receive_msg.Equals("|"))
            {
                Key_Press(VK_WON, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("'"))
            {
                Key_Press(VK_APOSTROPHE);
                receive_msg = "";
            }
            else if (receive_msg.Equals("\"")) // "랑 같은지를 표현할 때 \"를 적으면 됨.
            {
                Key_Press(VK_APOSTROPHE, VK_SHIFT);
                receive_msg = "";
            }
            else if (receive_msg.Equals("Original"))
            {
                is_rotate = false;
                receive_msg = "";
            }
            else if (receive_msg.Equals("Rotate"))
            {
                is_rotate = true;
                receive_msg = "";
            }
            else if (receive_msg.Equals("quit"))
            { 
                Tray_Exit(); // 트레이 잔상 지우고

                DefaultResolution(); // 해상도 돌려주고
                SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, defaultPath, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);  // 바탕화면도 돌려줘야 함.

                Application.ExitThread();
                Application.Restart();
                Environment.Exit(0);
            }
        }

        public void Sum_Packet(String receive_msg)
        {
            if (receive_msg.Equals("바탕화면")) // 바탕화면 키
            {
                Key_Press(VK_D, VK_LWIN);
            }
            else if (receive_msg.Equals("실행"))
            {
                Key_Press(VK_R, VK_LWIN);
            }
            else if (receive_msg.Equals("우정렬"))
            {
                Key_Press(VK_RIGHT, VK_LWIN);
            }
            else if (receive_msg.Equals("좌정렬"))
            {
                Key_Press(VK_LEFT, VK_LWIN);
            }
            else if (receive_msg.Equals("작업"))
            {
                Key_Press(VK_CONTROL, VK_SHIFT, VK_ESCAPE);
            }
            else if (receive_msg.Equals("모두선택"))
            {
                Key_Press(VK_A, VK_CONTROL);
            }
            else if (receive_msg.Equals("실행취소"))
            {
                Key_Press(VK_Z, VK_CONTROL);
            }
            else if (receive_msg.Equals("저장"))
            {
                Key_Press(VK_S, VK_CONTROL);
            }
            else if (receive_msg.Equals("완앞"))
            {
                Key_Press(VK_HOME, VK_CONTROL);
            }
            else if (receive_msg.Equals("완뒤"))
            {
                Key_Press(VK_END, VK_CONTROL);
            }
            else if (receive_msg.Equals("찾기"))
            {
                Key_Press(VK_F, VK_CONTROL);
            }
            else if (receive_msg.Equals("우단어"))
            {
                Key_Press(VK_CONTROL, VK_SHIFT, VK_RIGHT);
            }
            else if (receive_msg.Equals("좌단어"))
            {
                Key_Press(VK_CONTROL, VK_SHIFT, VK_LEFT);
            }
            else if (receive_msg.Equals("Alt_On"))
            {
                Alt_On();
            }
            else if (receive_msg.Equals("Alt_Move"))
            {
                Alt_Move();
            }
            else if (receive_msg.Equals("Alt_Off"))
            {
                Alt_Off();
            }
            else if (receive_msg.Equals("주소창"))
            {
                Key_Press(VK_D, VK_MENU);
            }
            else if (receive_msg.Equals("종료"))
            {
                Key_Press(VK_F4, VK_MENU);
            }
            else if (receive_msg.Equals("우페이지"))
            {
                Key_Press(VK_RIGHT, VK_MENU);
            }
            else if (receive_msg.Equals("좌페이지"))
            {
                Key_Press(VK_LEFT, VK_MENU);
            }
            else if (receive_msg.Equals("창최소"))
            {
                Key_Press(VK_MENU, VK_SPACE, VK_N);
            }
            else if (receive_msg.Equals("창최대"))
            {
                Key_Press(VK_MENU, VK_SPACE, VK_X);
            }
            else if (receive_msg.Equals("창이전"))
            {
                Key_Press(VK_MENU, VK_SPACE, VK_R);
            }
        }


        public void Connect_Thread() // 서버 연결 요청, 커넥트 되어 있지 않은 상태에서만.
        {
            while (is_connect == false && is_server_push == true)
            {
                try
                {
                    client_socket = server_socket.Accept(); // 서버 소켓은 클라이언트 연결을 대기한다.  
                    server_socket.Blocking = false;

                    capture_client_socket = capture_server_socket.Accept();
                    capture_server_socket.Blocking = false;

                    ns = new NetworkStream(client_socket); // 네트워크 스트림
                    sw = new StreamWriter(ns);

                    capture_ns = new NetworkStream(capture_client_socket); // 스트림 만들어 줌.
                    capture_sw = new StreamWriter(capture_ns);

                    is_connect = true;
                    is_server_push = false; 

                    is_resolution = true;
                    is_login = true;

                    darkPath = Path.Combine(Path.GetTempPath(), @"D:\Temp.bmp");

                    ChangeResolution(); // 해상도 변경 

                    SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, darkPath, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE); // 배경화면 검정색으로 바꾸기.  

                    function_thread.Start(); // 한영 검사 
                    capture_thread.Start(); // 캡쳐해서 이미지 보내는 스레드
                    receive_thread.Start(); // 키보드 메시지 수신  
                    capture_receive_thread.Start(); // 캡쳐 응답 메시지 수신

                    return;
                }
                catch (Exception e)
                {
                    Console.WriteLine("클라이언트 연결이 실패했습니다.\n" + e.Message);
                    Console.WriteLine("Accept Fail " + e.Message);
                }
            }
        }

        public void Capture_Thread()
        {
            while (is_connect == true)
            {
                FromCapture();
            }
        }

        static byte[] Compress(byte[] data) // gzipstream으로 압축함.
        {
            using (var compressedStream = new MemoryStream())
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
            {
                zipStream.Write(data, 0, data.Length);
                zipStream.Close();
                return compressedStream.ToArray();
            }
        }

        public void FromCapture() // 현재 폼을 캡쳐 = this.Bounds.X, this.Bounds.Y
        {
            Bitmap bitmap = null;

            byte[] sendSize;
            int count = 0, remain_size = 0;

            try
            {
                if (is_capturable == true)
                {
                    bitmap = CaptureDesktopWithCursor(current_size_x, current_size_y); // 마우스와 바탕화면을 같이 찍음   

                    if (is_rotate == true)
                    {
                        bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone); // 90도 돌려서 보내줌.
                        bitmap.Save(ms, pngEncoder, myEncoderParameters);
                    }
                    else
                    {
                        bitmap.Save(ms, pngEncoder, myEncoderParameters);
                    }

                    ms.Position = 0;

                    temp_byte = ms.ToArray();   // 내가 찍은 비트맵이 들어가있는 버퍼 v 
                    ms.SetLength(0); // 이걸 안하면 메모리 스트림의 사이즈가 점점 늘어남. 

                    capture_byte_buffer = Compress(temp_byte); // 메모리 스트림의 내용을 바이트 배열에 씀! (중요) -> 압축 함.      
                    capture_size = (int)capture_byte_buffer.Length; // 압축 후 사이즈를 계산
                }
            }
            catch (NullReferenceException nullexception)
            {
                Console.WriteLine("캡쳐 null exception");
            }

            if (is_connect == true) // 연결이 되어 있어야 함.
            {
                try
                {
                    if (is_length_packet == true)
                    {
                        sendSize = System.Text.Encoding.UTF8.GetBytes(capture_byte_buffer.Length.ToString());
                        capture_client_socket.Send(sendSize, 0, sendSize.Length, SocketFlags.None);

                        is_length_packet = false;

                        is_capturable = false;
                    }

                    if (is_data_packet == true)
                    {
                        int index = 0;  //몇번돌렷는지확인
                        int cnt = 0;  //지금 현재 버퍼에서 얼만큼 돌고있는지

                        count = capture_size / slice_buffer.Length;  //몇번돌리는지 
                        remain_size = capture_size % slice_buffer.Length;  //남은거고  

                        while (index < count) // 바탕화면을 여러 번 보냄
                        {
                            capture_client_socket.Send(capture_byte_buffer, cnt, slice_buffer.Length, SocketFlags.None);
                            cnt += slice_buffer.Length;
                            index++;
                        }
                        if (remain_size > 0)
                        {

                            capture_client_socket.Send(capture_byte_buffer, cnt, remain_size, SocketFlags.None);
                        }

                        is_capturable = true;
                        is_data_packet = false;
                    } 
                }

                catch (IOException ioexception) // 클라이언트 접속 해제시
                {
                    Console.WriteLine("IO Exception : " + ioexception.Message);
                }
                catch (SocketException socket)
                {
                    Console.WriteLine("socket Exception : " + socket.Message);
                }
            }
        }

        public void Capture_Zoom_In() // 전체 사이즈의 1/6 만큼 줄임. 화면 크게 보고 싶어요
        {
            current_size_x = current_size_x * 5 / 6;
            current_size_y = current_size_y * 5 / 6;

            if (current_size_x < min_size_x) // 계속 줄였는데 최소 사이즈보다 작아지면
                current_size_x = min_size_x; // 최소 사이즈로 고정함.
            if (current_size_y < min_size_y)
                current_size_y = min_size_y;

            pre_capture_x = current_position.x - (current_size_x / 2); // current_size : 해상도의 현재 사이즈
            pre_capture_y = current_position.y - (current_size_y / 2); // current_position : 마우스의 현재 좌표

            // 줌이 됬을 때 x의 좌표를 확대된 화면의 가운데로 옮김
            if (pre_capture_x < 0 && (current_size_x / 2) > pre_capture_x) // 까만 배경이 왼쪽으로 남을 때
            {
                pre_capture_x = 0;
            }
            else if (pre_capture_x + current_size_x > max_size_y && (max_size_x - current_size_x) <= pre_capture_x) // 까만 배경이 오른쪽으로 남을 때 <= pre 이부분은 포커스를 오른쪽에서 왼쪽으로 당겨옴
            {
                pre_capture_x = max_size_x - current_size_x; // 포커스 좌표 바꿔줌. 
            }

            // 줌이 됬을 때 y의 좌표를 확대된 화면의 가운데로 옮김
            if (pre_capture_y < 0 && (current_size_y / 2) > pre_capture_y) // 까만 배경이 위로 남을 때
            {
                pre_capture_y = 0;
            }
            else if (pre_capture_y + current_size_y > max_size_y && (max_size_y - current_size_y) <= pre_capture_y) // 까만 배경이 아래로 남을 때
            {
                pre_capture_y = max_size_y - current_size_y;
            }
        }

        public void Capture_Zoom_Out() // 전체 사이즈의 1/6 만큼 늘림. 화면을 더 크게 보기 위함
        {
            is_zoom_limit = true;

            current_size_x = current_size_x * 7 / 6;
            current_size_y = current_size_y * 7 / 6;

            if (current_size_x > max_size_x) current_size_x = max_size_x;
            if (current_size_y > max_size_y) current_size_y = max_size_y;

            pre_capture_x = current_position.x - (current_size_x / 2);
            pre_capture_y = current_position.y - (current_size_y / 2);

            // 멀티 터치 드래그 부분
            if (pre_capture_x < 0 && (current_size_x / 2) > pre_capture_x) // 까만 배경이 왼쪽으로 남을 때
            {
                pre_capture_x = 0;
            }
            else if (pre_capture_x + current_size_x > max_size_y && (max_size_x - current_size_x) <= pre_capture_x) // 까만 배경이 오른쪽으로 남을 때
            {
                pre_capture_x = max_size_x - current_size_x;
            }

            if (pre_capture_y < 0 && (current_size_y / 2) > pre_capture_y) // 까만 배경이 위로 남을 때
            {
                pre_capture_y = 0;
            }
            else if (pre_capture_y + current_size_y > max_size_y && (max_size_y - current_size_y) <= pre_capture_y) // 까만 배경이 아래로 남을 때
            {
                pre_capture_y = max_size_y - current_size_y;
            }
        }

        public Bitmap CaptureDesktopWithCursor(int current_size_x, int current_size_y)
        {
            try
            {
                desktopBMP = CaptureDesktop(current_size_x, current_size_y, current_position.x, current_position.y); // 데스크탑 찍어옴.  
                cursorBMP = CaptureCursor(desktopBMP, ref cursorX, ref cursorY); 
            }
            catch (Exception e)
            {

            } 

            return cursorBMP;
        }

        public Bitmap CaptureCursor(Bitmap temp_bitmap, ref int x, ref int y)
        {
            Bitmap bmp = temp_bitmap;

            try
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    Win32Stuff.CURSORINFO ci;
                    ci.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(Win32Stuff.CURSORINFO));

                    if (Win32Stuff.GetCursorInfo(out ci))
                    {
                        if (ci.flags == Win32Stuff.CURSOR_SHOWING)
                        {

                            x = ci.ptScreenPos.x - pre_capture_x; // 커서의 현재 위치
                            y = ci.ptScreenPos.y - pre_capture_y;

                            DrawIcon(g.GetHdc(), x, y, ci.hCursor);
                             
                            g.ReleaseHdc(); 
                        }
                    }
                }
            }
            catch (Exception e)
            {
                bmp = null;
            }

            return bmp; 
        }


        public Bitmap CaptureDesktop(int current_size_x, int current_size_y, int mouse_x, int mouse_y)
        {
            SIZE size; // size 구조체
            IntPtr hBitmap;

            // DC란 구조체를 의미하며 hDC가 이 구조체를 핸들링할 수 있는 번호, 즉 DC의 핸들링임.  
            IntPtr hDC = Win32Stuff.GetDC(Win32Stuff.GetDesktopWindow()); // 현재 윈도우의 핸들을 반환함. DC가 없기 때문에 검정색으로 표현
            IntPtr hMemDC = GDIStuff.CreateCompatibleDC(hDC); // 현재 윈도우의 DC(Device Context)값을 받음. 메모리 DC 생성  

            if (is_zoom_limit == true && (mouse_x + current_size_x / 2) > max_size_x) // 줌 아웃이 되고 마우스의 포커스가 x 경계(max_size_x)를 넘어가면.
            {
                pre_capture_x = pre_capture_x - max_size_x - mouse_x;

                if (pre_capture_x < 0 && (current_size_x / 2) > pre_capture_x) // 까만 배경이 왼쪽으로 남을 때
                {
                    pre_capture_x = 0;
                }
                else if (pre_capture_x + current_size_x > max_size_y && (max_size_x - current_size_x) <= pre_capture_x) // 까만 배경이 오른쪽으로 남을 때
                {
                    pre_capture_x = max_size_x - current_size_x;
                }
            }

            if (is_zoom_limit == true && (mouse_y + current_size_y / 2) > max_size_y) // 줌 아웃이 되고 마우스의 포커스가 y 경계(max_size_y)를 넘어가면.
            {
                pre_capture_y = pre_capture_y - max_size_y - mouse_y; // 마우스 포인터의 좌표인 mouse_y를 찍는 사각형의 포커스를 올리기 위해 최대 사이즈로 넘어가면 최대 값에서 y 값을 빼면 됨.

                if (pre_capture_y < 0 && (current_size_y / 2) > pre_capture_y) // 까만 배경이 위로 남을 때
                {
                    pre_capture_y = 0;
                }
                else if (pre_capture_y + current_size_y > max_size_y && (max_size_y - current_size_y) <= pre_capture_y) // 까만 배경이 아래로 남을 때
                {
                    pre_capture_y = max_size_y - current_size_y;
                }
            }

            is_zoom_limit = false;

            size.cx = current_size_x;
            size.cy = current_size_y;

            hBitmap = GDIStuff.CreateCompatibleBitmap(hDC, size.cx, size.cy); // 컬러 비트맵 만들기. 까만색 도화지를 의미함.

            if (hBitmap != IntPtr.Zero)
            {
                IntPtr hOld = (IntPtr)GDIStuff.SelectObject(hMemDC, hBitmap); // 읽어온 비트맵으로 DC에 선택함.

                // BitbBlt는 인수로 지정된 해당 DC의 특정영역을 Bitmap으로 복사하는 함수.
                // 1은 복사할 대상, 2,3번째는 복사할 위치, 4,5번째는 복사대상의 복사 영역을 의미함. 6은 복사대상 7,8은 복사된 비트맵을 표시할 대상 DC의 위치, 마지막은 어떤방식으로? (복사)
                GDIStuff.BitBlt(hMemDC, 0, 0, size.cx, size.cy, hDC, pre_capture_x, pre_capture_y, GDIStuff.SRCCOPY); // 메모리에 그려있는 비트맵을 화면 DC로 복사한다. 실제 사진찍어서 얹는부분.

                GDIStuff.SelectObject(hMemDC, hOld); // 생성한 DC와 이미지 연결
                GDIStuff.DeleteDC(hMemDC);
                Win32Stuff.ReleaseDC(Win32Stuff.GetDesktopWindow(), hDC);

                Bitmap bmp = System.Drawing.Image.FromHbitmap(hBitmap);
                GDIStuff.DeleteObject(hBitmap); // 반드시 오브젝트를 삭제해야함.
                GC.Collect();
                return bmp;
            }
            return null;
        }

        private void Display_Drag(int current_size_x, int current_size_y, int mouse_delta_x, int mouse_delta_y)
        {
            // current_position.x = 마우스 위치
            pre_capture_x = (current_position.x + mouse_delta_x) - (current_size_x / 2); // 포커스 상자의 x
            pre_capture_y = (current_position.y + mouse_delta_y) - (current_size_y / 2); // 포커스 상자의 y 좌표를 찾음.

            // 멀티 터치 드래그 부분
            if (pre_capture_x < 0 && (current_size_x / 2) > pre_capture_x) // 까만 배경이 왼쪽으로 남을 때
            {
                pre_capture_x = 0;
            }
            else if (pre_capture_x + current_size_x > max_size_y && (max_size_x - current_size_x) <= pre_capture_x) // 까만 배경이 오른쪽으로 남을 때
            {
                pre_capture_x = max_size_x - current_size_x;
            }

            if (pre_capture_y < 0 && (current_size_y / 2) > pre_capture_y) // 까만 배경이 위로 남을 때
            {
                pre_capture_y = 0;
            }
            else if (pre_capture_y + current_size_y > max_size_y && (max_size_y - current_size_y) <= pre_capture_y) // 까만 배경이 아래로 남을 때
            {
                pre_capture_y = max_size_y - current_size_y;
            }
        }

        public void Send_Method(string msg)
        {
            String temp_msg;

            try
            {
                temp_msg = Base64Encode(msg); // base64encoding으로 string 만듦
                sw.WriteLine(temp_msg);
                sw.Flush();
            }

            catch (NullReferenceException nullexception)
            {
                MessageBox.Show("" + nullexception.Message);
            }
            catch (SocketException socketexception2)
            {
                Debug.WriteLine("" + socketexception2.Message);
            }
        }

        private static Bitmap ConvertToBitmap(byte[] imagesSource) // byte[] -> bitmap
        {
            var imageConverter = new ImageConverter();
            var image = (Image)imageConverter.ConvertFrom(imagesSource);
            return new Bitmap(image);
        }

        public static string Base64Encode(string data) // string을 base64encoding함
        {
            try
            {
                byte[] encData_byte = new byte[data.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(data);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception e)
            {
                throw new Exception("Error in base64Encode" + e.Message);
            }
        }

        private void Check_Function_Key_Thread()
        {
            while (true)
            {
                GetCursorPos(ref current_position); // 마우스의 현재 포지션 확인   

                Check_HANGUL();
            }
        }

        public bool Check_HANGUL() // 지속적으로 한/영 상태 확인함.
        {
            hWnd = GetForegroundWindow(); // 윈도우 핸들 얻어냄.
            hImc = ImmGetDefaultIMEWnd(hWnd);
            int ret = SendMessage(hImc, WM_IME_CONTROL, 0x05, 0);

            if (ret == 1)
            {
                is_hangul = true;
                ImmReleaseContext(hWnd, hImc);
                return true;
            }
            else
            {
                is_hangul = false;
                ImmReleaseContext(hWnd, hImc);
                return false;
            } 
        }

        public void Make_HANGUL() // 현재 한/영 상태를 확인하여 한글로 전환함.
        {
            if (is_hangul == false) // 지금 만약 영어라면
            {
                Key_Press(VK_HANGUL);
            }
        }

        public void Make_ENGLISH() // 현재 한/영 상태를 확인하여 영어로 전환함.
        {
            if (is_hangul == true)
            {
                Key_Press(VK_HANGUL);
            }
        } 

        private void Udp_Mouse_Start() 
        {
            mouse_udp_socket.BeginReceiveFrom(mouse_buffer, 0, mouse_buffer.Length, SocketFlags.None, ref mouse_end_point, new AsyncCallback(MessageReceiveCallback), (object)this);
        }

        private void MessageReceiveCallback(IAsyncResult result) // 마우스 콜백
        {
            string pos = "";
            pt = System.Windows.Forms.Cursor.Position;    // set new point

            try
            {
                pos = Encoding.UTF8.GetString(mouse_buffer).Trim();

                if (pos.StartsWith("click"))
                {
                    Click();
                }

                else if (pos.StartsWith("d.click"))
                {
                    Click();
                    Thread.Sleep(80);
                    Click();
                }

                else if (pos.StartsWith("drag"))
                {
                    pos = pos.Substring(5);
                    drag_x = (int)float.Parse(pos.Substring(0, pos.IndexOf(",")));
                    drag_y = (int)float.Parse(pos.Substring(pos.IndexOf(",") + 1, pos.IndexOf("\0") + 1 - pos.IndexOf(",") + 1));

                    Display_Drag(current_size_x, current_size_y, drag_x, drag_y); // 화면 드래그
                    SetCursorPos(pt.X + drag_x, pt.Y + drag_y); // 화면이 옮겨지면 마우스 포인터도 옮겨야함.
                }

                else
                {
                    int deltaX = (int)float.Parse(pos.Substring(0, pos.IndexOf(","))) * this.speed;
                    int deltaY = (int)float.Parse(pos.Substring(pos.IndexOf(",") + 1, pos.IndexOf("\0") + 1 - pos.IndexOf(",") + 1)) * this.speed;

                    SetCursorPos(pt.X + deltaX, pt.Y + deltaY); // 마우스 이벤트 변경 
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            this.mouse_udp_socket.EndReceiveFrom(result, ref mouse_end_point);
            mouse_udp_socket.BeginReceiveFrom(mouse_buffer, 0, mouse_buffer.Length, SocketFlags.None, ref mouse_end_point, new AsyncCallback(MessageReceiveCallback), (object)this);
        }

        private void Tray_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Visible = true;
        }

        private void ReStart_Socket()
        { 
            Close_Socket(1);
        }

        public static void HookStart()
        {
            _hookID = SetHook(_proc);
        }
        public static void Hookend()
        {
            UnhookWindowsHookEx(_hookID);
        }

        // 후킹 callback  부분
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                switch ((MouseMessages)wParam)
                {
                    case MouseMessages.WM_LBUTTONDOWN:
                        if (LeftMouseDown != null)
                        {
                            MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                            LeftMouseDown(hookStruct.pt.x, hookStruct.pt.y);
                        }
                        break;
                    case MouseMessages.WM_LBUTTONUP:
                        if (LeftMouseUp != null)
                        {
                            MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                            LeftMouseUp(hookStruct.pt.x, hookStruct.pt.y);
                        }
                        break;
                    case MouseMessages.WM_MOUSEMOVE:
                        if (MouseMover != null)
                        {
                            MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                            MouseMover(hookStruct.pt.x, hookStruct.pt.y);
                        }
                        break;
                    case MouseMessages.WM_MOUSEWHEEL:
                        if (WheelClick != null)
                        {
                            MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                            WheelClick(hookStruct.pt.x, hookStruct.pt.y);
                        }
                        break;
                    case MouseMessages.WM_RBUTTONDOWN:
                        if (RightMouseDown != null)
                        {
                            MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                            RightMouseDown(hookStruct.pt.x, hookStruct.pt.y);
                        }
                        break;
                    case MouseMessages.WM_RBUTTONUP:
                        if (RightMouseUp != null)
                        {
                            MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                            RightMouseUp(hookStruct.pt.x, hookStruct.pt.y);
                        }
                        break;
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private const int WH_MOUSE_LL = 14;

        private enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205
        }

        private enum MouseEvent_Messages
        {
            LBUTTONDOWN = 0x0002,
            LBUTTONUP = 0x0004,
            RBUTTONDOWN = 0x0008,
            RBUTTONUP = 0x0010
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        public void Click() // 당장 클릭
        {
            if (is_shift_drag == false)
            {
                mouse_event(L_BUTTONDOWN, 0, 0, 0, 0);
            }

            mouse_event(L_BUTTONUP, 0, 0, 0, 0);
        }

        public void Click(int x, int y)
        {
            SetCursorPos(x, y);
            mouse_event(L_BUTTONDOWN, 0, 0, 0, 0);
            Thread.Sleep(100);
            mouse_event(L_BUTTONUP, 0, 0, 0, 0);
        }

        public void Scroll_Up()
        {
            mouse_event(MOUSEEVENTF_WHEEL, 0, 0, 120, 0); // 휠 위로 
        }

        public void Scroll_Down()
        {
            mouse_event(MOUSEEVENTF_WHEEL, 0, 0, -120, 0); // 휠 아래로
        }

        public void Scroll_Left()
        {
            mouse_event(MOUSEEVENTF_HWHEEL, 0, 0, -120, 0); // 왼쪽
        }

        public void Scroll_Right()
        {
            mouse_event(MOUSEEVENTF_HWHEEL, 0, 0, 120, 0); // 휠 오른쪽
        }

        public void Key_Press(byte key)
        {
            keybd_event(key, 0, 0, ref Info);   // Click key 다운
            keybd_event(key, 0, KEYUP, ref Info);
        }

        public void Key_Press(byte key, byte command)
        {
            keybd_event(command, 0, 0, ref Info);  // command 입력 
            keybd_event(key, 0, 0, ref Info);   // Click key 다운 
            keybd_event(key, 0, KEYUP, ref Info);
            keybd_event(command, 0, KEYUP, ref Info);
        }

        public void Key_Press(byte key1, byte key2, byte key3)
        {
            keybd_event(key1, 0, 0, ref Info);  // command 입력  
            keybd_event(key2, 0, 0, ref Info);   // Click key 다운  
            keybd_event(key3, 0, 0, ref Info);
            Thread.Sleep(20);
            keybd_event(key3, 0, KEYUP, ref Info);
            keybd_event(key2, 0, KEYUP, ref Info);
            keybd_event(key1, 0, KEYUP, ref Info);
        }

        private void Alt_On()
        {
            keybd_event(VK_MENU, 0, 0, ref Info);
            keybd_event(VK_TAB, 0, 0, ref Info);
            keybd_event(VK_TAB, 0, KEYUP, ref Info);
        }

        private void Alt_Move()
        {
            Key_Press(VK_TAB);
        }

        private void Alt_Off()
        {
            keybd_event(VK_MENU, 0, KEYUP, ref Info);
        }

        public void Drag_On()
        {
            mouse_event((int)MouseEvent_Messages.LBUTTONDOWN, 0, 0, 0, 0);
            is_shift_drag = true;
        }

        public void Drag_Off()
        {
            mouse_event((int)MouseEvent_Messages.LBUTTONUP, 0, 0, 0, 0);
            is_shift_drag = false;
        }

        public void Shift_On(byte shift)
        {
            keybd_event(shift, 0, 0, ref Info);   // Shift key 다운
        }

        public void Shift_Off(byte shift)
        {
            keybd_event(shift, 0, KEYUP, ref Info);   // Shift key 다운
        }

        public void Ctrl_On(byte ctrl)
        {
            keybd_event(ctrl, 0, 0, ref Info);
        }

        public void Ctrl_Off(byte ctrl)
        {
            keybd_event(ctrl, 0, KEYUP, ref Info);
        }

        private void Paste()
        {
            keybd_event(VK_CONTROL, 0, 0, ref Info); // 컨트롤키 누름
            keybd_event(VK_V, 0, 0, ref Info);
            Thread.Sleep(80);
            keybd_event(VK_V, 0, KEYUP, ref Info);
            keybd_event(VK_CONTROL, 0, KEYUP, ref Info);
        }

        private void Copy()
        {
            keybd_event(VK_CONTROL, 0, 0, ref Info); // 컨트롤키 누름
            keybd_event(VK_C, 0, 0, ref Info);
            Thread.Sleep(80);
            keybd_event(VK_C, 0, KEYUP, ref Info);
            keybd_event(VK_CONTROL, 0, KEYUP, ref Info);
        }

        public static void RClick()
        {
            mouse_event((int)MouseEvent_Messages.RBUTTONDOWN, 0, 0, 0, 0);
            Thread.Sleep(100);
            mouse_event((int)MouseEvent_Messages.RBUTTONUP, 0, 0, 0, 0);
        }

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private string GetFileSize(double byteCount) // 사이즈를 가시적으로 출력
        {
            string size = "0 Bytes";

            if (byteCount >= 1073741824.0)
                size = String.Format("{0:##.##}", byteCount / 1073741824.0) + " GB";
            else if (byteCount >= 1048576.0)
                size = String.Format("{0:##.##}", byteCount / 1048576.0) + " MB";
            else if (byteCount >= 1024.0)
                size = String.Format("{0:##.##}", byteCount / 1024.0) + " KB";
            else if (byteCount > 0 && byteCount < 1024.0)
                size = byteCount.ToString() + " Bytes";

            return size;
        }

        protected override void OnDragDrop(DragEventArgs drgevent) // 드래그 앤 드랍해서 파일의 경로를 가져오기.
        {
            try
            {
                if (is_connect == true)
                {
                    if (drgevent.Data.GetDataPresent(DataFormats.FileDrop))
                    {
                        fileNames = (string[])drgevent.Data.GetData(DataFormats.FileDrop);

                        foreach (string filepath in fileNames)
                        {
                            fInfo = new FileInfo(filepath);
                            strFileSize = (fInfo.Length).ToString();
                            filePath = filepath.ToString(); // 드랍 한 파일 경로 읽어줌. 
                        }
                    }

                    Send_Method("File"); // 8000 포트로 날림 

                    // 클라이언트에서 파일 전송 알림이 왔고 우선적으로 키보드 TCP가 연결되어 있어야 함. (최초 연결)
                    if (is_file_connect_start == false && is_file_connect == false)
                    {
                        is_file_connect_start = true;
                        file_connect_thread.Start();  // 파일 연결 준비 시작
                    }
                }
                else // 연결이 안되어 있으면 종료함.
                {
                    MessageBox.Show("로그인을 먼저 해 주세요.");
                    return;
                }
            }

            catch (Exception path_exception)
            {
                Debug.WriteLine(path_exception.Message);
                MessageBox.Show("파일 전송 실패");
            }
        }

        protected override void OnDragEnter(DragEventArgs drgevent) // 드래그앤드랍 이벤트. 파일을 폼 위로 들고오면 마우스 커서 모양을 네모로 바꾼다.
        {
            if (drgevent.Data.GetDataPresent(DataFormats.FileDrop))
            {
                drgevent.Effect = DragDropEffects.Copy; // 모양으로 띄워줌.
            }

            else
            {
                drgevent.Effect = DragDropEffects.None;
            }
            //base.OnDragEnter(drgevent); // 이미 base 클래스이니 따로 호출하지 않아도 된다. 
        }

        private void Tray_MouseClick(object sender, MouseEventArgs e) // 트레이 아이콘 마우스 클릭시
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (this.WindowState == FormWindowState.Normal)
                {
                    this.Visible = false;
                    this.WindowState = FormWindowState.Minimized;
                }
                else
                {
                    this.Visible = true;
                    this.WindowState = FormWindowState.Normal;
                }
            }
        }

        private void MakeBlackBitmap() // 블랙 비트맵 만듦
        {
            SolidBrush mySolidBrush = new SolidBrush(Color.Black);
            System.Drawing.Bitmap b = new System.Drawing.Bitmap(sz.Width, sz.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            b.Save("D:\\Temp.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            b.Dispose();
        }

        private void DeleteBlackBitmap() // 컴퓨터에서 까만 화면 지움
        {
            if (System.IO.File.Exists(@"D:\Temp.bmp"))
            {
                try
                {
                    System.IO.File.Delete(@"D:\Temp.bmp");
                }

                catch (System.IO.IOException ioexception)
                {
                    Debug.WriteLine("" + ioexception.Message);
                }
            }
            else
            {
                return;
            }
        }

        private string GetPathOfWallpaper() // 기본 배경화면 얻어오기.
        {
            string pathWallpaper = "";
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", false);
            if (regKey != null)
            {
                pathWallpaper = regKey.GetValue("WallPaper").ToString();
                regKey.Close();
            }
            return pathWallpaper;
        }

        private void ChangeResolution() // 해상도 바꾸기
        {
            Resolution change_resolution = new Resolution(1024, 768);
        }

        private void DefaultResolution() // 기본 해상도로
        {
            Resolution default_resolution = new Resolution(default_resolution_width, default_resolution_height);
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) // 화면 최소화
        {
            e.Cancel = true;
            this.Visible = false;
        }

        public void Tray_Exit()
        {
            this.Tray.Dispose(); // 리소스를 해제해야 트레이 아이콘 잔상이 제거된다.
        }

        private void Tray_DoubleClick_1(object sender, EventArgs e)
        {
            this.Visible = true;
            this.Activate(); //  폼을 활성화한다.
        }

        public void Close_Thread()
        {
            //label_state.Text = "연결 종료";

            try
            {
                if (connect_thread != null)
                    connect_thread.Abort();

                if (capture_thread != null)
                    capture_thread.Abort();

                if (receive_thread != null)
                    receive_thread.Abort();

                if (function_thread != null)
                    function_thread.Abort();

                if (capture_receive_thread != null)
                    capture_receive_thread.Abort();

                if (is_file_connect == true)
                {
                    if (file_connect_thread != null)
                        file_connect_thread.Abort();

                    if (file_receive_thread != null)
                        file_receive_thread.Abort();
                }

                // 로그인 관련 스레드는 이미 죽은 상태~ 

                is_connect = false;
                is_server_push = false;
                is_file_connect = false;
            }
            catch (Exception server_exception)
            {
                Console.WriteLine("스레드 종료가 실패하였습니다." + server_exception.Message);
            }
        }

        private void Close_Socket(int flag) // 1을 flag주면 모든 소켓 종료
        { 
            if (flag == 1)
            {
                if (file_client_socket != null)
                { 
                    file_client_socket.Close();
                }
                if (capture_server_socket != null)
                { 
                    capture_server_socket.Close();
                }
                if (capture_client_socket != null)
                { 
                    capture_client_socket.Close();
                }
            }

            if (is_file_connect)
            {
                if (file_server_socket != null)
                { 
                    file_server_socket.Close();
                }
            }
            try
            {
                if (server_socket != null)
                { 
                    server_socket.Close();
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show("server소켓 종료 실패");
            } 
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tray_Exit();
            DeleteBlackBitmap(); // 까만 비트맵 D드라이브에서 제거

            if (is_resolution == true)
                DefaultResolution(); // 해상도 원래대로

            if (is_connect == true) // 연결이 되어 있으면
            {
                SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, defaultPath, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE); // 바탕화면 돌려주기
                Close_Thread();
                Close_Socket(1); // 모든 소켓들을 닫는다.  
                Application.ExitThread();
                Environment.Exit(0);
            }
            else // 그렇지 않으면
            {
                SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, defaultPath, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE); // 바탕화면 돌려주기.
                Close_Thread();
                Close_Socket(0); // 열려있던 기존 소켓들만 닫기. 
                Application.ExitThread(); // 프로세스 종료
                Environment.Exit(0);
            }

            this.Close(); // 폼 닫기. 
        }

        private void SearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            search_form = new SearchForm();
            search_form.Show();
        }

        private void ChangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            change_form = new ChangeForm();
            change_form.Show();
        }

        private void 사용자정보ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            data_form = new DataForm();
            data_form.Show();
        }

        private void 환경설정ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setting_form = new SettingForm();
            setting_form.Show();
        }

        private void 사용자탈퇴ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ini_data = new iniData(file_path); 
            file_info = new FileInfo(file_path);

            if (file_info.Exists) // 우선 ini 파일이 존재해야 한다.
            {
                if (is_login) // 로그인이 된 상태라면
                {
                    if (MessageBox.Show("회원 탈퇴가 진행됩니다. 계속 하시겠습니까?", "시스템 메시지", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        System.IO.File.Delete(@"D:\config.ini");
                        MessageBox.Show("탈퇴되었습니다.");
                    }
                    else // 회원 탈퇴를 취소한다면
                    {

                    }
                }
                else if (!is_login)
                {
                    MessageBox.Show("로그인을 먼저 해 주세요.");
                }
            }
        }

        public Region Converting() // 서버 모양을 만들기 위해 컨버팅 함
        {
            try
            {
                baseBitmap = new Bitmap(SSM_RemoteControl_Project_Ver2._0.Properties.Resources.ATOP);
                gUnit = GraphicsUnit.Pixel;
                graphicsPath = new GraphicsPath();
            }
            catch (Exception e)
            {
                MessageBox.Show("경로 탐색 실패");
            }

            key = baseBitmap.GetPixel(1, 1);
            rectF = baseBitmap.GetBounds(ref gUnit);
            xMax = (int)rectF.Width;
            yMax = (int)rectF.Height;

            for (int y = 0; y < yMax; y++)
            {
                int xStart = 0;
                int xEnd = 0;

                for (int x = 0; x < xMax; x++)
                {
                    if (baseBitmap.GetPixel(x, y) == key)
                        continue;
                    xStart = x;
                    break;
                }
                for (int x = xMax - 1; x > 0; x--)
                {
                    if (baseBitmap.GetPixel(x, y) == key)
                        continue;
                    xEnd = x;
                    break;
                }
                graphicsPath.AddRectangle(new Rectangle(xStart, y, xEnd - xStart + 1, 1));
            }

            Region region = new Region(graphicsPath);
            graphicsPath.Dispose();
            return region;
        }

        private void MainForm_Load(object sender, EventArgs e)
        { 
            Size si = SystemInformation.PrimaryMonitorSize;

            int nWidth = si.Width;
            int nHeight = si.Height;

            this.Location = new Point(si.Width - (this.Size.Width - 20), nHeight - 150);
        }

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(mouseOffset.X, mouseOffset.Y);
                Location = mousePos;
            }
        }

        private void MainForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = false;
            }
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            int xOffset;
            int yOffset;

            if (e.Button == MouseButtons.Left)
            {
                xOffset = -e.X - SystemInformation.FrameBorderSize.Width;
                yOffset = -e.Y - SystemInformation.CaptionHeight -
                    SystemInformation.FrameBorderSize.Height;
                mouseOffset = new Point(xOffset, yOffset);
                isMouseDown = true;
            }
        } 
    }
}
