using System.Windows;

namespace WpfKeyboard.Test
{
    public partial class MainWindow : Window
    {
        private int _type = 0;

        public MainWindow()
        {
            InitializeComponent();

            tbInput.AcceptsReturn = true;
            tbInput.Focus();

            generalKeyboard.IsEnableHook = true;
            numbericKeyboard.IsEnableHook = true;

            btnType.Click += BtnType_Click;
            btnShow.Click += BtnShow_Click;
        }
        private void BtnShow_Click(object sender, RoutedEventArgs e)
        {
            var show = false;

            if(_type == 0)
            {
                generalKeyboard.IsShow= !generalKeyboard.IsShow;
                show = generalKeyboard.IsShow;
            }
            else
            {
                numbericKeyboard.IsShow = !numbericKeyboard.IsShow;
                show = numbericKeyboard.IsShow;
            }

            btnShow.Content = show ? "Hide" : "Show";
            tbInput.Focus();
        }

        private void BtnType_Click(object sender, RoutedEventArgs e)
        {
            bool show = false;

            if(_type == 0)
            {
                show = numbericKeyboard.IsShow;

                generalKeyboard.Visibility = Visibility.Hidden;
                numbericKeyboard.Visibility = Visibility.Visible;
                _type = 1;           
            }
            else
            {
                show = generalKeyboard.IsShow;

                numbericKeyboard.Visibility = Visibility.Hidden;
                generalKeyboard.Visibility = Visibility.Visible;
                _type = 0;
            }

            btnType.Content = _type == 0 ? "Numberic" : "General";
            btnShow.Content = show ? "Hide" : "Show";
            tbInput.Focus();
        }
    }
}