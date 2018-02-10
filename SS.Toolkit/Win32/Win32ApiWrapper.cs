using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SS.Toolkit.Win32
{
    //参考文章
    //http://en.verysource.com/code/2755298_1/sendkey.cs.html
    //https://www.cnblogs.com/zzh1236/archive/2013/07/18/3198517.html
    //https://www.cnblogs.com/Joetao/articles/5844131.html
    public class Win32ApiWrapper
    {

        public static int IDM_VIEWSOURCE = 2139;

        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;
        public const int VK_CONTROL = 0x11;
        public const int VK_F5 = 0x74;
        public const int KEYEVENTF_KEYUP = 0x2;
        public const int VK_MENU = 0x12;
        public const int WM_SETTEXT = 0xC;
        public const int WM_CLEAR = 0x303;
        public const int BN_CLICKED = 0;
        public const int WM_LBUTTONDOWN = 0x201;
        public const int WM_LBUTTONUP = 0x202;
        public const int WM_LBUTTONDBLCLK = 0x0203;//双击鼠标左键
        public const int WM_MOUSEMOVE = 0x0200;
        public const int WM_MOUSEWHEEL = 0x020A;
        public const int WM_RBUTTONDOWN = 0x0204;
        public const int WM_RBUTTONUP = 0x0205;
        public const int WM_CLOSE = 0x10;
        public const int WM_COMMAND = 0x111;
        public const int WM_SYSKEYDOWN = 0x104;
        public const int GW_HWNDNEXT = 2;
        public const int WM_CLICK = 0x00F5;

        //鼠标移动
        //dX和dy保留移动的信息。给出的信息是绝对或相对整数值。
        public static readonly int MOUSEEVENTF_MOVE = 0x0001;
        //绝对位置
        //则dX和dy含有标准化的绝对坐标，其值在0到65535之间。
        //事件程序将此坐标映射到显示表面。
        //坐标（0，0）映射到显示表面的左上角，（65535，65535）映射到右下角
        //如果没指定MOUSEEVENTF_ABSOLUTE，dX和dy表示相对于上次鼠标事件产生的位置（即上次报告的位置）的移动。
        //正值表示鼠标向右（或下）移动；负值表示鼠标向左（或上）移动。
        public static readonly int MOUSEEVENTF_ABSOLUTE = 0x8000;
        public static readonly int MOUSEEVENTF_LEFTDOWN = 0x0002;//左键按下
        public static readonly int MOUSEEVENTF_LEFTUP = 0x0004;//左键抬起
        public static readonly int MOUSEEVENTF_RIGHTDOWN = 0x0008; //右键按下 
        public static readonly int MOUSEEVENTF_RIGHTUP = 0x0010; //右键抬起 
        public static readonly int MOUSEEVENTF_MIDDLEDOWN = 0x0020; //中键按下 
        public static readonly int MOUSEEVENTF_MIDDLEUP = 0x0040;// 中键抬起 

        private static Dictionary<string, byte> keycode = new Dictionary<string, byte>();

        public delegate bool WNDENUMPROC(IntPtr hWnd, int lParam);

        static Win32ApiWrapper()
        {
            keycode.Add("A", 65);
            keycode.Add("B", 66);
            keycode.Add("C", 67);
            keycode.Add("D", 68);
            keycode.Add("E", 69);
            keycode.Add("F", 70);
            keycode.Add("G", 71);
            keycode.Add("H", 72);
            keycode.Add("I", 73);
            keycode.Add("J", 74);
            keycode.Add("K", 75);
            keycode.Add("L", 76);
            keycode.Add("M", 77);
            keycode.Add("N", 78);
            keycode.Add("O", 79);
            keycode.Add("P", 80);
            keycode.Add("Q", 81);
            keycode.Add("R", 82);
            keycode.Add("S", 83);
            keycode.Add("T", 84);
            keycode.Add("U", 85);
            keycode.Add("V", 86);
            keycode.Add("W", 87);
            keycode.Add("X", 88);
            keycode.Add("Y", 89);
            keycode.Add("Z", 90);
            keycode.Add("0", 48);
            keycode.Add("1", 49);
            keycode.Add("2", 50);
            keycode.Add("3", 51);
            keycode.Add("4", 52);
            keycode.Add("5", 53);
            keycode.Add("6", 54);
            keycode.Add("7", 55);
            keycode.Add("8", 56);
            keycode.Add("9", 57);
            keycode.Add(".", 0x6E);
            keycode.Add("LEFT", 0x25);
            keycode.Add("UP", 0x26);
            keycode.Add("RIGHT", 0x27);
            keycode.Add("DOWN", 0x28);
            keycode.Add("-", 0x6D);
        }

        /// <summary>
        /// 定义结构体
        /// </summary>
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData; //可以是任意值
            public int cbData;    //指定lpData内存区域的字节数
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData; //发送给目录窗口所在进程的数据
        }

        /// <summary>
        /// 鼠标 移动
        /// </summary>
        /// <param name="dwFlags"></param>
        /// <param name="dx">DX=需要移动的X*65536/取屏幕宽度 ()+1</param>
        /// <param name="dy">DY=需要移动的Y*65536/取屏幕高度 ()+1</param>
        /// <param name="cButtons"></param>
        /// <param name="dwExtraInfo"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern int mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);


        [DllImport("Gdi32.dll")]
        public extern static int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public extern static IntPtr GetWindow(IntPtr hWnd, int wCmd);

        [DllImport("user32.dll", EntryPoint = "GetParent")]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll ")]
        public static extern IntPtr FindWindowEx(IntPtr parent, IntPtr childe, string strclass, string FrmText);

        [DllImport("user32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int GetWindowTextW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, [Out, MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, int lParam);

        [DllImport("user32.dll")]
        public static extern int GetClassNameW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern bool EnumChildWindows(IntPtr hWndParent, WNDENUMPROC lpEnumFunc, int lParam);

        [DllImport("user32.dll", EntryPoint = "WindowFromPoint", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr WindowFromPoint(Point pt);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowRect(IntPtr hwnd, ref Rectangle rc);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClientRect(IntPtr hwnd, ref Rectangle rc);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int MoveWindow(IntPtr hwnd, int x, int y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int ScreenToClient(IntPtr hWnd, ref Rectangle rect);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int Width, int Height, int flags);
        /// <summary>   
        /// 得到当前活动的窗口   
        /// </summary>   
        /// <returns></returns>   
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern System.IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern int SendTxtMessage(int hWnd,
            // handle to destination window               
            int Msg, // message               
            int wParam, // first message parameter               
            char[] lParam             // int  lParam // second message parameter           
          );

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern int SendTxtMessage(int hWnd,
          // handle to destination window               
          int Msg, // message               
          int wParam, // first message parameter               
          string lParam             // int  lParam // second message parameter           
          );

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(int hWnd, int Msg, int wParam, ref COPYDATASTRUCT lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(
            int hWnd, // handle to destination window               
            int Msg, // message               
            int wParam, // first message parameter                
            int lParam // second message parameter         
            );

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        /// <summary>
        /// 得到光标的位置
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "GetCursorPos")]
        public static extern bool GetCursorPos(out Point pt);

        /// <summary>
        /// 移动光标的位置
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        [DllImport("user32.dll")]
        public static extern void SetCursorPos(int x, int y);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int Width, int Height, uint flags);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern bool SetWindowPos(IntPtr hWnd, int HWND_TOPMOST, int x, int y, int Width, int Height, uint flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "PostMessage")]
        public static extern IntPtr PostMessage(IntPtr hwndParent, int hwndChildAfter, IntPtr wParam, string lpszWindow);

    }

    public enum WM_SYSCOMMAND_WPARAM
    {
        SC_FIRST = 0xF000,

        // Sizes the window.
        SC_SIZE = SC_FIRST,

        // Moves the window.
        SC_MOVE = SC_FIRST + 0x10,

        // Minimizes the window.
        SC_MINIMIZE = SC_FIRST + 0x20,

        // Maximizes the window.
        SC_MAXIMIZE = SC_FIRST + 0x30,

        // Moves to the next window.
        SC_NEXTWINDOW = SC_FIRST + 0x40,

        // Moves to the previous window.
        SC_PREVWINDOW = SC_FIRST + 0x50,

        // Closes the window.
        SC_CLOSE = SC_FIRST + 0x60,

        //Scrolls vertically
        SC_VSCROLL = SC_FIRST + 0x70,

        // Scrolls horizontally.
        SC_HSCROLL = SC_FIRST + 0x80,

        // Retrieves the window menu as a result of a mouse click.
        SC_MOUSEMENU = SC_FIRST + 0x90,

        // Retrieves the window menu as a result of a keystroke.
        // For more information, see the Remarks section.
        SC_KEYMENU = SC_FIRST + 0x100,

        SC_ARRANGE = SC_FIRST + 0x110,

        // Restores the window to its normal position and size.
        SC_RESTORE = SC_FIRST + 0x120,

        // Activates the Start menu.
        SC_TASKLIST = SC_FIRST + 0x130,

        // Executes the screen saver application specified 
        // in the [boot] section of the System.ini file.
        SC_SCREENSAVE = SC_FIRST + 0x140,

        // Activates the window associated with the application-specified hot key. 
        // The lParam parameter identifies the window to activate.
        SC_HOTKEY = SC_FIRST + 0x150,

        // Selects the default item; 
        // the user double-clicked the window menu.
        SC_DEFAULT = SC_FIRST + 0x160,

        // Sets the state of the display.
        // This command supports devices that have power-saving features,
        // such as a battery-powered personal computer.
        // The lParam parameter can have the following values:
        // -1 - the display is powering on
        //  1 - the display is going to low power
        //  2 - the display is being shut off
        SC_MONITORPOWER = SC_FIRST + 0x170,

        // Changes the cursor to a question mark with a pointer. 
        // If the user then clicks a control in the dialog box, 
        // the control receives a WM_HELP message.
        SC_CONTEXTHELP = SC_FIRST + 0x180,

        SC_SEPARATOR = 0xF00F
    }

    public class WindowInfo
    {
        public IntPtr hWnd { get; set; }

        public string SzWindowName { get; set; }

        public string SzClassName { get; set; }
    }
}
