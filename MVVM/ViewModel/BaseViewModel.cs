using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
namespace MVVM.ViewModel
{
    public abstract class BaseViewModel : INotifyPropertyChanged, IDisposable
    {
        #region Constructor
        protected BaseViewModel() { }
        #endregion // Constructor

        #region Public Event
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        #endregion // Public Event

        #region Virtual
        protected virtual void OnDispose() { }
        public virtual string DisplayName { get; protected set; }
        public virtual string DisplayTitle { get; protected set; }
        protected virtual bool ThrowOnInvalidPropertyName { get; private set; }
        #endregion // Virtual

        #region INotifyPropertyChanged Members
        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.VerifyPropertyName(propertyName);
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
        protected virtual void SetProperty<T>(ref T member, T val, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(member, val)) return;
            member = val;
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;
                if (this.ThrowOnInvalidPropertyName)
                    throw new Exception(msg);
                else
                    Debug.Fail(msg);
            }
        }
        #endregion // INotifyPropertyChanged Members

        #region IDisposable Members
        public void Dispose()
        {
            this.OnDispose();
        }
#if DEBUG
        ~BaseViewModel()
        {
            string msg = string.Format("{0} ({1}) ({2}) Finalized", this.GetType().Name, this.DisplayName, this.GetHashCode());
            System.Diagnostics.Debug.WriteLine(msg);
        }
#endif
        #endregion // IDisposable Members
    }
}