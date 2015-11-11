using WindowsInput;
using WindowsInput.Native;

namespace WpfKeyboard.Helper
{
    internal class InputSimulatorStatic
    {
        private static InputSimulator inputSimulator;
        private static KeyboardSimulator keyboardSimulator;

        private InputSimulatorStatic()
        {
        }

        internal static InputSimulator Input
        {
            get { return inputSimulator ?? (inputSimulator = new InputSimulator()); }
        }

        internal static KeyboardSimulator Keyboard
        {
            get { return keyboardSimulator ?? (keyboardSimulator = new KeyboardSimulator(Input)); }
        }
    }
}