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
        /// <summary>
        /// A property for indicating to viewmodel processes whether or not 
        /// the viewmodel should be refreshed. Only intended to be set by 
        /// derived viewmodels, but can be read publicly.
        /// </summary>
        /// <value><c>true</c> if needs refresh; otherwise, <c>false</c>.</value>
        public bool NeedsRefresh
        {
            get;
            protected set;
        }

        bool _IsBusy;
        /// <summary>
        /// Gets or sets the "IsBusy" property
        /// </summary>
        /// <value>The IsBusy property.</value>
        public const string IsBusyPropertyName = "IsBusy";

        public bool IsBusy
        {
            get { return _IsBusy; }
            set { SetProperty(ref _IsBusy, value, IsBusyPropertyName); }
        }

        INavigation _Navigation;
        /// <summary>
        /// A reference to INavigation, typically assigned from the Navigation property on a Page. 
        /// Allows use of INavigation from within viewmodels, which can be quite handy.
        /// </summary>
        /// <value>The navigation.</value>
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

        /// <summary>
        /// A reference to the page that this ViewModel belongs to. 
        /// Generally, you don't want a ViewModel to be aware of it's associated View (Page),
        /// But since the DisplayAlert() method is only available on Page, we're holding onto 
        /// a reference to Page so that we can use DisplayAlert() from within ViewModels. An 
        /// alternative pattern would be use MessagingCenter to send messages from the ViewModel 
        /// to the Page in order to have Page call DisplayAlert() as needed.
        /// </summary>
        /// <value>The page which referencces this ViewModel.</value>
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

