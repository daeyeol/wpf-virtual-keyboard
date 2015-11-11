using System.Windows;

namespace WpfKeyboard.Test
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _type = 0;

        public MainWindow()
        {
            InitializeComponent();

            this.tbInput.AcceptsReturn = true;
            this.tbInput.Focus();

            this.btnType.Click += BtnType_Click;
            this.btnShow.Click += BtnShow_Click;
        }
        private void BtnShow_Click(object sender, RoutedEventArgs e)
        {
            var show = false;

            if(this._type == 0)
            {
                this.generalKeyboard.IsShow= !this.generalKeyboard.IsShow;
                show = this.generalKeyboard.IsShow;
            }
            else
            {
                this.numbericKeyboard.IsShow = !this.numbericKeyboard.IsShow;
                show = this.numbericKeyboard.IsShow;
            }

            this.btnShow.Content = show ? "Hide" : "Show";
            this.tbInput.Focus();
        }

        private void BtnType_Click(object sender, RoutedEventArgs e)
        {
            bool show = false;

            if(this._type == 0)
            {
                show = this.numbericKeyboard.IsShow;

                this.generalKeyboard.Visibility = Visibility.Hidden;
                this.numbericKeyboard.Visibility = Visibility.Visible;
                this._type = 1;           
            }
            else
            {
                show = this.generalKeyboard.IsShow;

                this.numbericKeyboard.Visibility = Visibility.Hidden;
                this.generalKeyboard.Visibility = Visibility.Visible;
                this._type = 0;
            }

            this.btnType.Content = this._type == 0 ? "Numberic" : "General";
            this.btnShow.Content = show ? "Hide" : "Show";
            this.tbInput.Focus();
        }
    }
}