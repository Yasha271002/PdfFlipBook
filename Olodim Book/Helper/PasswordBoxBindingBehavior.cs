using System.Windows.Controls;
using System.Windows;

namespace PdfFlipBook.Helper
{
    public static class PasswordBoxBindingBehavior
    {
        public static readonly DependencyProperty BoundPasswordProperty =
            DependencyProperty.RegisterAttached("BoundPassword", typeof(string), typeof(PasswordBoxBindingBehavior),
                new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

        public static readonly DependencyProperty BindPasswordProperty =
            DependencyProperty.RegisterAttached("BindPassword", typeof(bool), typeof(PasswordBoxBindingBehavior),
                new PropertyMetadata(false, OnBindPasswordChanged));

        private static bool _updating;

        public static void SetBindPassword(DependencyObject dp, bool value)
        {
            dp.SetValue(BindPasswordProperty, value);
        }

        public static bool GetBindPassword(DependencyObject dp)
        {
            return (bool)dp.GetValue(BindPasswordProperty);
        }

        public static string GetBoundPassword(DependencyObject dp)
        {
            return (string)dp.GetValue(BoundPasswordProperty);
        }

        public static void SetBoundPassword(DependencyObject dp, string value)
        {
            dp.SetValue(BoundPasswordProperty, value);
        }

        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is PasswordBox passwordBox) || _updating) return;

            passwordBox.PasswordChanged -= HandlePasswordChanged;
            if (passwordBox.Password != (string)e.NewValue)
            {
                passwordBox.Password = (string)e.NewValue;
            }
            passwordBox.PasswordChanged += HandlePasswordChanged;
        }

        private static void OnBindPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            if (!(dp is PasswordBox passwordBox)) return;

            bool wasBound = (bool)e.OldValue;
            bool needToBind = (bool)e.NewValue;

            if (wasBound)
            {
                passwordBox.PasswordChanged -= HandlePasswordChanged;
            }

            if (needToBind)
            {
                passwordBox.PasswordChanged += HandlePasswordChanged;
            }
        }

        private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                _updating = true;
                SetBoundPassword(passwordBox, passwordBox.Password);
                _updating = false;
            }
        }
    }
}
