using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
namespace MVVM.Service.Password
{
    public static class PasswordBoxProperties
    {
        public static readonly DependencyProperty EncryptedPasswordProperty = DependencyProperty.RegisterAttached("EncryptedPassword", typeof(SecureString), typeof(PasswordBoxProperties));
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.RegisterAttached("Password", typeof(string), typeof(PasswordBoxProperties), new FrameworkPropertyMetadata(string.Empty, OnPasswordPropertyChanged));
        public static readonly DependencyProperty AttachProperty = DependencyProperty.RegisterAttached("Attach", typeof(bool), typeof(PasswordBoxProperties), new PropertyMetadata(false, Attach));
        private static readonly DependencyProperty IsUpdatingProperty = DependencyProperty.RegisterAttached("IsUpdating", typeof(bool), typeof(PasswordBoxProperties));

        // Secure string encryption
        public static SecureString GetEncryptedPassword(DependencyObject obj)
        {
            return (SecureString)obj.GetValue(EncryptedPasswordProperty);
        }
        public static void SetEncryptedPassword(DependencyObject obj, SecureString value)
        {
            obj.SetValue(EncryptedPasswordProperty, value);
        }
        public static byte[] ConvertSecurePassword(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            SetEncryptedPassword(passwordBox, passwordBox.SecurePassword);
            return CalculateHash(ConvertSecureStringToByteArray(passwordBox.SecurePassword));
        }
        // hashing

        public static byte[] ConvertSecureStringToByteArray(SecureString value)
        {
            byte[] returnVal = new byte[value.Length];
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                for (int i = 0; i < value.Length; i++)
                {
                    short unicodeChar = Marshal.ReadInt16(valuePtr, i * 2);
                    returnVal[i] = Convert.ToByte(unicodeChar);
                }
                return returnVal;
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }
        public static byte[] CalculateHash(byte[] inputBytes)
        {
            SHA256Managed algorithm = new SHA256Managed();
            algorithm.ComputeHash(inputBytes);
            return algorithm.Hash;
        }
        public static bool SequenceEquals(byte[] originalByteArray, byte[] newByteArray)
        {
            if (originalByteArray == null || newByteArray == null)
                throw new ArgumentNullException(originalByteArray == null ? "originalByteArray" : "newByteArray", "The byte arrays supplied may not be null.");
            if (originalByteArray.Length != newByteArray.Length)
                return false;
            for (int i = 0; i < originalByteArray.Length; i++)
            {
                if (originalByteArray[i] != newByteArray[i])
                    return false;
            }
            return true;
        }
        // PasswordBox Properties 
        public static void SetAttach(DependencyObject dp, bool value)
        {
            dp.SetValue(AttachProperty, value);
        }
        public static bool GetAttach(DependencyObject dp)
        {
            return (bool)dp.GetValue(AttachProperty);
        }
        public static string GetPassword(DependencyObject dp)
        {
            return (string)dp.GetValue(PasswordProperty);
        }
        public static void SetPassword(DependencyObject dp, string value)
        {
            dp.SetValue(PasswordProperty, value);
        }
        private static bool GetIsUpdating(DependencyObject dp)
        {
            return (bool)dp.GetValue(IsUpdatingProperty);
        }
        private static void SetIsUpdating(DependencyObject dp, bool value)
        {
            dp.SetValue(IsUpdatingProperty, value);
        }
        private static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            SetEncryptedPassword(passwordBox, passwordBox.SecurePassword);
            passwordBox.PasswordChanged -= PasswordChanged;
            if (!(bool)GetIsUpdating(passwordBox))
                passwordBox.Password = (string)e.NewValue;
            passwordBox.PasswordChanged += PasswordChanged;
        }
        private static void Attach(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            if (passwordBox == null)
                return;
            if ((bool)e.OldValue)
                passwordBox.PasswordChanged -= PasswordChanged;
            if ((bool)e.NewValue)
                passwordBox.PasswordChanged += PasswordChanged;
        }
        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            SetIsUpdating(passwordBox, true);
            SetPassword(passwordBox, passwordBox.Password);
            SetIsUpdating(passwordBox, false);
        }
    }
}