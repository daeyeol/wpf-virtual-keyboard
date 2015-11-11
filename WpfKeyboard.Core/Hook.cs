using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;

// http://blogs.msdn.com/b/toub/archive/2006/05/03/589423.aspx
// http://blogs.msdn.com/b/toub/archive/2006/05/03/589468.aspx
// http://www.devpia.com/MAEUL/Contents/Detail.aspx?BoardID=278&MAEULNo=20&no=12183&ref=12183	 
// http://www.devpia.com/MAEUL/Contents/Detail.aspx?BoardID=278&MAEULNo=20&no=13213&ref=13213	 

namespace WpfKeyboard.Core
{
    public class Hook
    {
        #region Variable

        private static IntPtr _handle = IntPtr.Zero;
        private static IntPtr _hModule = IntPtr.Zero;
        private static IntPtr _keyboardId = IntPtr.Zero;
        private static IntPtr _mouseId = IntPtr.Zero;

        private static Win32Api.HookProc _keyboardProc = new Win32Api.HookProc(KeyboardProc);
        private static Win32Api.HookProc _mouseProc = new Win32Api.HookProc(MouseProc);

        private static Window _window;
        private static Win32Api.MOUSEHOOKSTRUCT _mouseParam;
        private static Rect hookingArea;

        private static IntPtr _prevWindow = IntPtr.Zero;
        private static IntPtr _prevFocus = IntPtr.Zero;

        public delegate void MouseClickEventHandler(Win32Api.POINT point, Win32Api.MouseMessages msg);
        public static event MouseClickEventHandler MouseClickEvent;

        #endregion

        #region Property

        public static bool IsRun { get; private set; }
        public static bool UseGlobal { get; set; }

        #endregion

        #region Constructor

        public Hook()
        {
        }

        #endregion

        #region Public Method

        public static void Start(Rect hookingArea)
        {
            if (!IsRun)
            {
                Hook.hookingArea = hookingArea;

                uint threadId = Win32Api.GetCurrentThreadId();

                using (Process process = Process.GetCurrentProcess())
                {
                    using (ProcessModule module = process.MainModule)
                    {
                        _window = System.Windows.Application.Current.MainWindow;
                        _handle = process.MainWindowHandle;

                        _hModule = Win32Api.GetModuleHandle(module.ModuleName);

                        _keyboardId = Win32Api.SetWindowsHookEx((int)Win32Api.HookType.WH_KEYBOARD, _keyboardProc, _hModule, threadId);
                        _mouseId = Win32Api.SetWindowsHookEx((int)Win32Api.HookType.WH_MOUSE_LL, _mouseProc, _hModule, 0);

                        IsRun = true;
                    }
                }
            }
        }

        public static void Stop()
        {
            if (IsRun)
            {
                Win32Api.UnhookWindowsHookEx(_keyboardId);
                Win32Api.UnhookWindowsHookEx(_mouseId);

                IsRun = false;
            }
        }

        #endregion

        #region Private Method

        private static IntPtr KeyboardProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode == Win32Api.HC_ACTION)
            {
                uint wParamValue = (uint)wParam;
                long lParamValue = (long)lParam;

                // 229 ( 0xE5 ) : VK_PROCESSKEY ( IME PROCESS key )
                if ((wParamValue == 229 && lParamValue == -2147483647) || (wParamValue == 229 && lParamValue == -2147483648))
                {
                    if (IsHookingArea())
                    {
                        return (IntPtr)1;
                    }
                }
            }

            return Win32Api.CallNextHookEx(_keyboardId, nCode, wParam, lParam);
        }

        private static IntPtr MouseProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                _mouseParam = (Win32Api.MOUSEHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(Win32Api.MOUSEHOOKSTRUCT));
                var mouseMessage = (Win32Api.MouseMessages)wParam;

                if (UseGlobal)
                {
                    if (mouseMessage == Win32Api.MouseMessages.WM_LBUTTONDOWN || mouseMessage == Win32Api.MouseMessages.WM_LBUTTONUP)
                    {
                        var onMouseClickEvent = MouseClickEvent;
                        if (onMouseClickEvent != null)
                        {
                            onMouseClickEvent(_mouseParam.pt, mouseMessage);
                        }

                        if (mouseMessage == Win32Api.MouseMessages.WM_LBUTTONDOWN && IsHookingArea())
                        {
                            return (IntPtr)1;
                        }
                    }
                }
            }

            return Win32Api.CallNextHookEx(_mouseId, nCode, wParam, lParam);
        }

        private static bool IsHookingArea()
        {
            var point = _window.PointFromScreen(new Point((double)_mouseParam.pt.x, (double)_mouseParam.pt.y));
            var contains = hookingArea.Contains(point);

            return contains;
        }

        #endregion
    }
}