using System.Windows;
using WpfKeyboard.Control;
using WpfKeyboard.Helper;

namespace WpfKeyboard
{
    /// <summary>
    /// NumbericKeyboard.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NumbericKeyboard : KeyboardUserControl
    {
        #region Constructor

        public NumbericKeyboard()
        {
            InitializeComponent();

            AddHandler(KeyButton.ClickEvent, (RoutedEventHandler)KeyButton_Click);
        }

        #endregion

        #region Event Handler

        private void KeyButton_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is KeyButton)
            {
                var keyButton = e.OriginalSource as KeyButton;

                InputSimulatorStatic.Keyboard.KeyPress(keyButton.KeyCode);
                this.RaiseVirtualKeyDownEvent(keyButton.KeyCode);
            }
        }

        #endregion
    }
}