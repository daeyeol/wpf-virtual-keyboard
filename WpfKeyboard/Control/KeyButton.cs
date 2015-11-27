using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WindowsInput.Native;
using WpfKeyboard.Model;

namespace WpfKeyboard.Control
{
    public class KeyButton : Button
    {
        #region Variable

        private static Dictionary<VirtualKeyCode, KeyData> _keyDictionary;

        #endregion

        #region Dependency Property

        #region IsPressed

        public static readonly new DependencyProperty IsPressedProperty =
            DependencyProperty.Register("IsPressed",
                typeof(bool),
                typeof(KeyButton));

        public new bool IsPressed
        {
            get { return (bool)GetValue(IsPressedProperty); }
            set { SetValue(IsPressedProperty, value); }
        }

        #endregion

        #endregion

        #region Property

        #region KeyCode

        public VirtualKeyCode KeyCode { get; set; }

        #endregion

        #endregion

        #region Constructor

        static KeyButton()
        {
            MappingKeys();
        }

        public KeyButton()
        {
            Focusable = false;
            IsTabStop = false;
            ClickMode = ClickMode.Press;
        }

        #endregion

        #region Public Method

        public void UpdateKey(bool shift, bool capsLock, bool hangul)
        {
            if (!_keyDictionary.ContainsKey(KeyCode))
            {
                return;
            }

            var data = _keyDictionary[KeyCode];
            var key = data.DefaultKey;

            if (KeyCode >= VirtualKeyCode.VK_A && KeyCode <= VirtualKeyCode.VK_Z)
            {
                if (hangul)
                {
                    if (capsLock && !shift &&
                        (KeyCode == VirtualKeyCode.VK_Q || KeyCode == VirtualKeyCode.VK_W || KeyCode == VirtualKeyCode.VK_E ||
                        KeyCode == VirtualKeyCode.VK_R || KeyCode == VirtualKeyCode.VK_T || KeyCode == VirtualKeyCode.VK_O ||
                        KeyCode == VirtualKeyCode.VK_P))
                    {
                        key = data.KorKey;
                    }
                    else
                    {
                        key = IsUpper(shift, capsLock, hangul) && !string.IsNullOrWhiteSpace(data.KorShiftKey) ? data.KorShiftKey : data.KorKey;
                    }
                }
                else
                {
                    key = IsUpper(shift, capsLock, hangul) ? key.ToUpper() : key;
                }
            }
            else
            {
                key = IsUpper(shift, capsLock, hangul) && !string.IsNullOrWhiteSpace(data.ShiftKey) ? data.ShiftKey : key;
            }

            if (KeyCode == VirtualKeyCode.SHIFT)
            {
                IsPressed = shift;
            }

            if (KeyCode == VirtualKeyCode.CAPITAL)
            {
                IsPressed = capsLock;
            }

            Content = key;
        }

        #endregion

        #region Private Method

        private bool IsUpper(bool shift, bool capsLock, bool hangul)
        {
            return (shift && !capsLock) || (capsLock && !shift && !hangul) || (capsLock && !shift);
        }

        private static void MappingKeys()
        {
            _keyDictionary = new Dictionary<VirtualKeyCode, KeyData>();

            _keyDictionary.Add(VirtualKeyCode.VK_1, new KeyData { DefaultKey = "1", ShiftKey = "!" });
            _keyDictionary.Add(VirtualKeyCode.VK_2, new KeyData { DefaultKey = "2", ShiftKey = "@" });
            _keyDictionary.Add(VirtualKeyCode.VK_3, new KeyData { DefaultKey = "3", ShiftKey = "#" });
            _keyDictionary.Add(VirtualKeyCode.VK_4, new KeyData { DefaultKey = "4", ShiftKey = "$" });
            _keyDictionary.Add(VirtualKeyCode.VK_5, new KeyData { DefaultKey = "5", ShiftKey = "%" });
            _keyDictionary.Add(VirtualKeyCode.VK_6, new KeyData { DefaultKey = "6", ShiftKey = "^" });
            _keyDictionary.Add(VirtualKeyCode.VK_7, new KeyData { DefaultKey = "7", ShiftKey = "&" });
            _keyDictionary.Add(VirtualKeyCode.VK_8, new KeyData { DefaultKey = "8", ShiftKey = "*" });
            _keyDictionary.Add(VirtualKeyCode.VK_9, new KeyData { DefaultKey = "9", ShiftKey = "(" });
            _keyDictionary.Add(VirtualKeyCode.VK_0, new KeyData { DefaultKey = "0", ShiftKey = ")" });

            _keyDictionary.Add(VirtualKeyCode.VK_A, new KeyData { DefaultKey = "a", KorKey = "ㅁ" });
            _keyDictionary.Add(VirtualKeyCode.VK_B, new KeyData { DefaultKey = "b", KorKey = "ㅠ" });
            _keyDictionary.Add(VirtualKeyCode.VK_C, new KeyData { DefaultKey = "c", KorKey = "ㅊ" });
            _keyDictionary.Add(VirtualKeyCode.VK_D, new KeyData { DefaultKey = "d", KorKey = "ㅇ" });
            _keyDictionary.Add(VirtualKeyCode.VK_E, new KeyData { DefaultKey = "e", KorKey = "ㄷ" });
            _keyDictionary.Add(VirtualKeyCode.VK_F, new KeyData { DefaultKey = "f", KorKey = "ㄹ" });
            _keyDictionary.Add(VirtualKeyCode.VK_G, new KeyData { DefaultKey = "g", KorKey = "ㅎ" });
            _keyDictionary.Add(VirtualKeyCode.VK_H, new KeyData { DefaultKey = "h", KorKey = "ㅗ" });
            _keyDictionary.Add(VirtualKeyCode.VK_I, new KeyData { DefaultKey = "i", KorKey = "ㅑ" });
            _keyDictionary.Add(VirtualKeyCode.VK_J, new KeyData { DefaultKey = "j", KorKey = "ㅓ" });
            _keyDictionary.Add(VirtualKeyCode.VK_K, new KeyData { DefaultKey = "k", KorKey = "ㅏ" });
            _keyDictionary.Add(VirtualKeyCode.VK_L, new KeyData { DefaultKey = "l", KorKey = "ㅣ" });
            _keyDictionary.Add(VirtualKeyCode.VK_M, new KeyData { DefaultKey = "m", KorKey = "ㅡ" });
            _keyDictionary.Add(VirtualKeyCode.VK_N, new KeyData { DefaultKey = "n", KorKey = "ㅜ" });
            _keyDictionary.Add(VirtualKeyCode.VK_O, new KeyData { DefaultKey = "o", KorKey = "ㅐ", KorShiftKey = "ㅒ" });
            _keyDictionary.Add(VirtualKeyCode.VK_P, new KeyData { DefaultKey = "p", KorKey = "ㅔ", KorShiftKey = "ㅖ" });
            _keyDictionary.Add(VirtualKeyCode.VK_Q, new KeyData { DefaultKey = "q", KorKey = "ㅂ", KorShiftKey = "ㅃ" });
            _keyDictionary.Add(VirtualKeyCode.VK_R, new KeyData { DefaultKey = "r", KorKey = "ㄱ", KorShiftKey = "ㄲ" });
            _keyDictionary.Add(VirtualKeyCode.VK_S, new KeyData { DefaultKey = "s", KorKey = "ㄴ" });
            _keyDictionary.Add(VirtualKeyCode.VK_T, new KeyData { DefaultKey = "t", KorKey = "ㅅ", KorShiftKey = "ㅆ" });
            _keyDictionary.Add(VirtualKeyCode.VK_U, new KeyData { DefaultKey = "u", KorKey = "ㅕ" });
            _keyDictionary.Add(VirtualKeyCode.VK_V, new KeyData { DefaultKey = "v", KorKey = "ㅍ" });
            _keyDictionary.Add(VirtualKeyCode.VK_W, new KeyData { DefaultKey = "w", KorKey = "ㅈ", KorShiftKey = "ㅉ" });
            _keyDictionary.Add(VirtualKeyCode.VK_X, new KeyData { DefaultKey = "x", KorKey = "ㅌ", KorShiftKey = "ㄸ" });
            _keyDictionary.Add(VirtualKeyCode.VK_Y, new KeyData { DefaultKey = "y", KorKey = "ㅛ" });
            _keyDictionary.Add(VirtualKeyCode.VK_Z, new KeyData { DefaultKey = "z", KorKey = "ㅋ" });

            _keyDictionary.Add(VirtualKeyCode.OEM_3, new KeyData { DefaultKey = "`", ShiftKey = "~" });
            _keyDictionary.Add(VirtualKeyCode.OEM_MINUS, new KeyData { DefaultKey = "-", ShiftKey = "_" });
            _keyDictionary.Add(VirtualKeyCode.OEM_PLUS, new KeyData { DefaultKey = "=", ShiftKey = "+" });
            _keyDictionary.Add(VirtualKeyCode.BACK, new KeyData { DefaultKey = "Backspace" });
            _keyDictionary.Add(VirtualKeyCode.TAB, new KeyData { DefaultKey = "Tab" });
            _keyDictionary.Add(VirtualKeyCode.OEM_4, new KeyData { DefaultKey = "[", ShiftKey = "{" });
            _keyDictionary.Add(VirtualKeyCode.OEM_6, new KeyData { DefaultKey = "]", ShiftKey = "}" });
            _keyDictionary.Add(VirtualKeyCode.OEM_5, new KeyData { DefaultKey = "￦", ShiftKey = "|" });
            _keyDictionary.Add(VirtualKeyCode.CAPITAL, new KeyData { DefaultKey = "Caps Lock" });
            _keyDictionary.Add(VirtualKeyCode.OEM_1, new KeyData { DefaultKey = ";", ShiftKey = ":" });
            _keyDictionary.Add(VirtualKeyCode.OEM_7, new KeyData { DefaultKey = "'", ShiftKey = "″" });
            _keyDictionary.Add(VirtualKeyCode.RETURN, new KeyData { DefaultKey = "Enter" });
            _keyDictionary.Add(VirtualKeyCode.SHIFT, new KeyData { DefaultKey = "Shift" });
            _keyDictionary.Add(VirtualKeyCode.OEM_COMMA, new KeyData { DefaultKey = ",", ShiftKey = "<" });
            _keyDictionary.Add(VirtualKeyCode.OEM_PERIOD, new KeyData { DefaultKey = ".", ShiftKey = ">" });
            _keyDictionary.Add(VirtualKeyCode.OEM_2, new KeyData { DefaultKey = "/", ShiftKey = "?" });
            _keyDictionary.Add(VirtualKeyCode.HANGUL, new KeyData { DefaultKey = "한/영" });
            _keyDictionary.Add(VirtualKeyCode.SPACE, new KeyData { DefaultKey = "Space" });

            _keyDictionary.Add(VirtualKeyCode.NUMPAD0, new KeyData { DefaultKey = "0" });
            _keyDictionary.Add(VirtualKeyCode.NUMPAD1, new KeyData { DefaultKey = "1" });
            _keyDictionary.Add(VirtualKeyCode.NUMPAD2, new KeyData { DefaultKey = "2" });
            _keyDictionary.Add(VirtualKeyCode.NUMPAD3, new KeyData { DefaultKey = "3" });
            _keyDictionary.Add(VirtualKeyCode.NUMPAD4, new KeyData { DefaultKey = "4" });
            _keyDictionary.Add(VirtualKeyCode.NUMPAD5, new KeyData { DefaultKey = "5" });
            _keyDictionary.Add(VirtualKeyCode.NUMPAD6, new KeyData { DefaultKey = "6" });
            _keyDictionary.Add(VirtualKeyCode.NUMPAD7, new KeyData { DefaultKey = "7" });
            _keyDictionary.Add(VirtualKeyCode.NUMPAD8, new KeyData { DefaultKey = "8" });
            _keyDictionary.Add(VirtualKeyCode.NUMPAD9, new KeyData { DefaultKey = "9" });
        }

        #endregion
    }
}