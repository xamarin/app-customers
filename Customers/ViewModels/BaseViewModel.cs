using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;

namespace Customers
{
    /// <summary>
    /// A base viewmodel that provides some nice-to-have properties and behaviors.
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public bool NeedsRefresh
        {
            get;
            protected set;
        }

        bool _IsBusy;
        /// <summary>
        /// Gets or sets the "IsBusy" property
        /// </summary>
        /// <value>The isbusy property.</value>
        public const string IsBusyPropertyName = "IsBusy";

        public bool IsBusy
        {
            get { return _IsBusy; }
            set { SetProperty(ref _IsBusy, value, IsBusyPropertyName); }
        }

        INavigation _Navigation;

        public INavigation Navigation
        {
            get { return _Navigation; }
            set
            {
                _Navigation = value;
                OnPropertyChanged("Navigation");
            }
        }

        Page _Page;

        public Page Page
        {
            get { return _Page; }
            set
            {
                _Page = value;
                OnPropertyChanged("Page");
            }
        }

        protected void SetProperty<U>(
            ref U backingStore, U value,
            string propertyName,
            Action onChanged = null,
            Action<U> onChanging = null)
        {
            if (EqualityComparer<U>.Default.Equals(backingStore, value))
                return;

            if (onChanging != null)
                onChanging(value);

            OnPropertyChanging(propertyName);

            backingStore = value;

            if (onChanged != null)
                onChanged();

            OnPropertyChanged(propertyName);
        }

        #region INotifyPropertyChanging implementation

        public event PropertyChangingEventHandler PropertyChanging;

        #endregion

        public void OnPropertyChanging(string propertyName)
        {
            if (PropertyChanging == null)
                return;

            PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
        }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

