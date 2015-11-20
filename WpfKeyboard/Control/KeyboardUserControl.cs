using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WindowsInput.Native;
using WpfKeyboard.Core;
using WpfKeyboard.Helper;

namespace WpfKeyboard.Control
{
    public class KeyboardUserControl : UserControl
    {
        #region Variable

        private Rect? _hookingArea;
        private TranslateTransform _translate;

        #endregion

        #region Event

        #region VirtualKeyDown

        public static readonly RoutedEvent VirtualKeyDownEvent =
            EventManager.RegisterRoutedEvent("VirtualKeyDown",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(KeyboardUserControl));

        public event RoutedEventHandler VirtualKeyDown
        {
            add { this.AddHandler(VirtualKeyDownEvent, value); }
            remove { this.RemoveHandler(VirtualKeyDownEvent, value); }
        }

        #endregion

        #endregion

        #region Dependency Property

        #region IsShow

        public static readonly DependencyProperty IsShowProperty =
            DependencyProperty.Register("IsShow",
                typeof(bool),
                typeof(KeyboardUserControl),
                new PropertyMetadata(ChangedIsShowProperty));

        public bool IsShow
        {
            get { return (bool)this.GetValue(IsShowProperty); }
            set { this.SetValue(IsShowProperty, value); }
        }

        private static void ChangedIsShowProperty(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var keyboard = obj as KeyboardUserControl;
            var isShow = (bool)e.NewValue;

            keyboard.ChangeShow(isShow);
        }

        #endregion

        #region UseGlobal

        public static readonly DependencyProperty UseGlobalProperty =
            DependencyProperty.Register("UseGlobal",
                typeof(bool),
                typeof(KeyboardUserControl),
                new PropertyMetadata(true, ChangedUseGlobalProperty));

        public bool UseGlobal
        {
            get { return (bool)this.GetValue(UseGlobalProperty); }
            set { this.SetValue(UseGlobalProperty, value); }
        }

        private static void ChangedUseGlobalProperty(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            Hook.UseGlobal = (bool)e.NewValue;
        }

        #endregion

        #endregion

        #region Constructor

        public KeyboardUserControl()
        {
            this.Loaded += KeyboarUserControl_Loaded;
        }

        private void KeyboarUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                this._translate = new TranslateTransform();
                this.RenderTransform = this._translate;

                Hook.MouseClickEvent += Hook_MouseClickEvent;

                this.Unloaded += (s, args) =>
                {
                    Hook.Stop();
                    Hook.MouseClickEvent -= Hook_MouseClickEvent;
                };

                Application.Current.MainWindow.Closed += (s, args) => Hook.Stop();

                StartHook();
            }
        }

        #endregion

        #region Protected Method

        protected void StartHook()
        {
            if (UpdateHookingArea())
            {
                Hook.UseGlobal = this.UseGlobal;
                Hook.Start(this._hookingArea.Value);
            }
        }

        protected void StopHook()
        {
            Hook.Stop();
        }

        protected bool UpdateHookingArea()
        {
            if (!(this.Content is FrameworkElement))
            {
                this._hookingArea = null;
                return false;
            }

            var content = this.Content as FrameworkElement;
            var bounds = VisualTreeHelper.GetDescendantBounds(content);

            if (bounds != Rect.Empty)
            {
                this._hookingArea = content.TransformToVisual(Application.Current.MainWindow).TransformBounds(bounds);
                return true;
            }
            else
            {
                return false;
            }
        }

        protected void RaiseVirtualKeyDownEvent(VirtualKeyCode keyCode)
        {
            RoutedEventArgs args = new RoutedEventArgs(KeyboardUserControl.VirtualKeyDownEvent, keyCode);
            this.RaiseEvent(args);
        }

        #endregion

        #region Private Method

        private void ChangeShow(bool isShow)
        {
            var offset = VisualTreeHelper.GetOffset(this);

            Debug.WriteLine(offset.ToString());

            Storyboard storyboard = new Storyboard();

            var to = isShow ? 0 : this.ActualHeight + offset.Y;

            DoubleAnimation doubleAnimation = new DoubleAnimation(to, new Duration(TimeSpan.FromSeconds(0.35)));
            doubleAnimation.EasingFunction = new QuinticEase { EasingMode = EasingMode.EaseInOut };

            storyboard.Children.Add(doubleAnimation);

            Storyboard.SetTargetProperty(storyboard, new PropertyPath("(0).(1)",
                new DependencyProperty[] { UIElement.RenderTransformProperty, TranslateTransform.YProperty }));

            this.BeginStoryboard(storyboard, HandoffBehavior.SnapshotAndReplace);
        }

        #endregion

        #region Event Handler

        private void Hook_MouseClickEvent(Win32Api.POINT point, Win32Api.MouseMessages msg)
        {
            var screenPoint = this.PointFromScreen(new Point(point.x, point.y));
            var result = TreeHelper.TryFindFromPoint<KeyButton>(this, screenPoint);

            if (result is KeyButton)
            {
                var keyButton = result as KeyButton;

                var key = (Key)keyButton.KeyCode;
                var target = Keyboard.FocusedElement;

                if (msg == Win32Api.MouseMessages.WM_LBUTTONDOWN)
                {
                    keyButton.SetValue(KeyButton.IsPressedProperty, true);

                    if (keyButton.ClickMode == ClickMode.Press)
                    {
                        keyButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    }

                }
                else if (msg == Win32Api.MouseMessages.WM_LBUTTONUP)
                {
                    keyButton.SetValue(KeyButton.IsPressedProperty, false);

                    if (keyButton.ClickMode == ClickMode.Release)
                    {
                        keyButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    }
                }
            }
        }

        #endregion
    }
}