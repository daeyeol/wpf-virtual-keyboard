using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WindowsInput.Native;
using WpfKeyboard.Control;
using WpfKeyboard.Helper;

namespace WpfKeyboard
{
    /// <summary>
    /// GeneralKeyboard.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class GeneralKeyboard : KeyboardUserControl
    {
        #region Property

        public bool IsPressShift { get; private set; }
        public bool IsPressHangul { get; private set; }
        public bool IsCapsLock { get; private set; }

        #endregion

        #region Constructor

        public GeneralKeyboard()
        {
            InitializeComponent();

            AddHandler(KeyButton.ClickEvent, (RoutedEventHandler)KeyButton_Click);
        }

        #endregion

        #region Private Method

        private void UpdateKeys()
        {
            var content = this.Content as Panel;
            ChangeKeys(content);

        }
        private void ChangeKeys(Panel panel)
        {
            foreach(UIElement child in panel.Children)
            {
                if(child is Panel)
                {
                    var content = child as Panel;
                    ChangeKeys(content);
                }
                else if(child is KeyButton)
                {
                    var keyButton = child as KeyButton;
                    keyButton.UpdateKey(this.IsPressShift, this.IsCapsLock, this.IsPressHangul);
                }
            }
        }

        #endregion

        #region Event Handler

        private void KeyButton_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is KeyButton)
            {
                var keyButton = e.OriginalSource as KeyButton;

                if(keyButton.KeyCode == VirtualKeyCode.CAPITAL)
                {
                    this.IsCapsLock = !this.IsCapsLock;
                    UpdateKeys();

                    InputSimulatorStatic.Keyboard.KeyPress(keyButton.KeyCode);
                }
                else if(keyButton.KeyCode == VirtualKeyCode.SHIFT)
                {
                    this.IsPressShift = !this.IsPressShift;
                    UpdateKeys();

                    if (this.IsPressShift)
                    {
                        InputSimulatorStatic.Keyboard.KeyDown(keyButton.KeyCode);
                    }
                }
                else if(keyButton.KeyCode == VirtualKeyCode.HANGUL)
                {
                    this.IsPressHangul = !this.IsPressHangul;
                    UpdateKeys();

                    InputSimulatorStatic.Keyboard.KeyPress(keyButton.KeyCode);
                }
                else
                {
                    if (this.IsPressShift)
                    {
                        this.IsPressShift = false;
                        UpdateKeys();

                        InputSimulatorStatic.Keyboard.KeyPress(keyButton.KeyCode);
                        InputSimulatorStatic.Keyboard.KeyUp(VirtualKeyCode.SHIFT);
                    }
                    else
                    {
                        InputSimulatorStatic.Keyboard.KeyPress(keyButton.KeyCode);
                    }
                }

                this.RaiseVirtualKeyDownEvent(keyButton.KeyCode);
            }
        }

        #endregion
    }
}