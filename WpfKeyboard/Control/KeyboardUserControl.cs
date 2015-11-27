using System;
using System.ComponentModel;
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
        private bool _keyPressed;

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
            add { AddHandler(VirtualKeyDownEvent, value); }
            remove { RemoveHandler(VirtualKeyDownEvent, value); }
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
            get { return (bool)GetValue(IsShowProperty); }
            set { SetValue(IsShowProperty, value); }
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
            get { return (bool)GetValue(UseGlobalProperty); }
            set { SetValue(UseGlobalProperty, value); }
        }

        private static void ChangedUseGlobalProperty(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            Hook.UseGlobal = (bool)e.NewValue;
        }

        #endregion

        #endregion

        #region Property

        #region Property

        public bool IsPressedShift { get; private set; }
        public bool IsPressedHangul { get; private set; }
        public bool IsPressedCapsLock { get; private set; }

        #endregion

        #endregion

        #region Constructor

        public KeyboardUserControl()
        {
            IsVisibleChanged += (s, args) => UpdateKeyState((bool)args.NewValue);
            Loaded += KeyboarUserControl_Loaded;
        }

        private void KeyboarUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _translate = new TranslateTransform();
                RenderTransform = _translate;

                Hook.MouseClickEvent += Hook_MouseClickEvent;
                Hook.KeyClickEvent += Hook_KeyClickEvent;

                Unloaded += (s, args) =>
                {
                    Hook.Stop();
                    Hook.MouseClickEvent -= Hook_MouseClickEvent;
                    Hook.KeyClickEvent -= Hook_KeyClickEvent;
                };

                Application.Current.MainWindow.Closed += (s, args) => Hook.Stop();

                AddHandler(KeyButton.ClickEvent, (RoutedEventHandler)KeyButton_Click);
                StartHook();
            }
        }

        #endregion

        #region Protected Method

        protected void StartHook()
        {
            if (UpdateHookingArea())
            {
                Hook.UseGlobal = UseGlobal;
                Hook.Start(_hookingArea.Value);
            }
        }

        protected void StopHook()
        {
            Hook.Stop();
        }

        protected bool UpdateHookingArea()
        {
            if (!(Content is FrameworkElement))
            {
                _hookingArea = null;
                return false;
            }

            var content = this.Content as FrameworkElement;
            var bounds = VisualTreeHelper.GetDescendantBounds(content);

            if (bounds != Rect.Empty)
            {
                _hookingArea = content.TransformToVisual(Application.Current.MainWindow).TransformBounds(bounds);
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
            RaiseEvent(args);
        }

        protected void UpdateKeys()
        {
            var content = Content as Panel;
            ChangeKeys(content);

        }
        protected void ChangeKeys(Panel panel)
        {
            foreach (UIElement child in panel.Children)
            {
                if (child is Panel)
                {
                    var content = child as Panel;
                    ChangeKeys(content);
                }
                else if (child is KeyButton)
                {
                    var keyButton = child as KeyButton;
                    keyButton.UpdateKey(IsPressedShift, IsPressedCapsLock, IsPressedHangul);
                }
            }
        }

        #endregion

        #region Private Method

        private void UpdateKeyState(bool visible)
        {
            if (visible)
            {
                IsPressedHangul = InputSimulatorStatic.Input.InputDeviceState.IsTogglingKeyInEffect(VirtualKeyCode.HANGUL);
                IsPressedCapsLock = InputSimulatorStatic.Input.InputDeviceState.IsTogglingKeyInEffect(VirtualKeyCode.CAPITAL);

                UpdateKeys();
            }
        }

        private void ChangeShow(bool isShow)
        {
            var offset = VisualTreeHelper.GetOffset(this);

            Storyboard storyboard = new Storyboard();

            var to = isShow ? 0 : ActualHeight + offset.Y;

            DoubleAnimation doubleAnimation = new DoubleAnimation(to, new Duration(TimeSpan.FromSeconds(0.35)));
            doubleAnimation.EasingFunction = new QuinticEase { EasingMode = EasingMode.EaseInOut };

            storyboard.Children.Add(doubleAnimation);

            Storyboard.SetTargetProperty(storyboard, new PropertyPath("(0).(1)",
                new DependencyProperty[] { UIElement.RenderTransformProperty, TranslateTransform.YProperty }));

            BeginStoryboard(storyboard, HandoffBehavior.SnapshotAndReplace);
        }

        #endregion

        #region Event Handler

        private void Hook_MouseClickEvent(Win32Api.POINT point, Win32Api.MouseMessages msg)
        {
            var screenPoint = PointFromScreen(new Point(point.x, point.y));
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

        private void Hook_KeyClickEvent(uint keyCode)
        {
            if (_keyPressed) return;

            var virtualKeyCode = (VirtualKeyCode)keyCode;

            switch (virtualKeyCode)
            {
                case VirtualKeyCode.HANGUL:
                    IsPressedHangul = !InputSimulatorStatic.Input.InputDeviceState.IsTogglingKeyInEffect(VirtualKeyCode.HANGUL);
                    break;

                case VirtualKeyCode.CAPITAL:
                    IsPressedCapsLock = !InputSimulatorStatic.Input.InputDeviceState.IsTogglingKeyInEffect(VirtualKeyCode.CAPITAL);
                    break;

                case VirtualKeyCode.LSHIFT:
                case VirtualKeyCode.RSHIFT:
                    IsPressedShift = !InputSimulatorStatic.Input.InputDeviceState.IsTogglingKeyInEffect(VirtualKeyCode.SHIFT);
                    break;
            }

            if (IsShow || Visibility == Visibility.Visible)
            {
                UpdateKeys();
            }
        }

        private void KeyButton_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is KeyButton)
            {
                _keyPressed = true;

                var keyButton = e.OriginalSource as KeyButton;

                if (keyButton.KeyCode == VirtualKeyCode.CAPITAL)
                {
                    IsPressedCapsLock = !IsPressedCapsLock;
                    UpdateKeys();

                    InputSimulatorStatic.Keyboard.KeyPress(keyButton.KeyCode);
                }
                else if (keyButton.KeyCode == VirtualKeyCode.SHIFT)
                {
                    IsPressedShift = !IsPressedShift;
                    UpdateKeys();

                    if (IsPressedShift)
                    {
                        InputSimulatorStatic.Keyboard.KeyDown(keyButton.KeyCode);
                    }
                }
                else if (keyButton.KeyCode == VirtualKeyCode.HANGUL)
                {
                    IsPressedHangul = !IsPressedHangul;
                    UpdateKeys();

                    InputSimulatorStatic.Keyboard.KeyPress(keyButton.KeyCode);
                }
                else
                {
                    if (keyButton.KeyCode == VirtualKeyCode.RETURN)
                    {
                        if (Keyboard.FocusedElement is TextBox)
                        {
                            var textBox = Keyboard.FocusedElement as TextBox;
                            var binding = textBox.GetBindingExpression(TextBox.TextProperty);

                            binding.UpdateSource();
                        }
                    }

                    if (IsPressedShift)
                    {
                        IsPressedShift = false;
                        UpdateKeys();

                        InputSimulatorStatic.Keyboard.KeyPress(keyButton.KeyCode);
                        InputSimulatorStatic.Keyboard.KeyUp(VirtualKeyCode.SHIFT);
                    }
                    else
                    {
                        InputSimulatorStatic.Keyboard.KeyPress(keyButton.KeyCode);
                    }
                }

                RaiseVirtualKeyDownEvent(keyButton.KeyCode);

                _keyPressed = false;
            }
        }

        #endregion
    }
}