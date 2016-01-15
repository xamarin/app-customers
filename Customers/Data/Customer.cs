using System;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Customers
{
    public class Customer : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public Customer()
        {
            Id = Guid.NewGuid().ToString();
            PhotoUrl = "placeholderProfileImage";
        }

        protected void OnPropertyChanged([CallerMemberName] string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public string Id { get; set; }

        string _FirstName;
        public string FirstName
        {
            get { return _FirstName; }
            set
            {
                if (_FirstName == value)
                    return;
                _FirstName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayName));
                OnPropertyChanged(nameof(DisplayLastNameFirst));
            }
        }

        string _LastName;
        public string LastName
        {
            get { return _LastName; }
            set
            {
                if (_LastName == value)
                    return;
                _LastName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayName));
                OnPropertyChanged(nameof(DisplayLastNameFirst));
            }
        }

        string _Company;
        public string Company
        {
            get { return _Company; }
            set
            {
                if (_Company == value)
                    return;
                _Company = value;
                OnPropertyChanged();
            }
        }

        string _JobTitle;
        public string JobTitle
        {
            get { return _JobTitle; }
            set
            {
                if (_JobTitle == value)
                    return;
                _JobTitle = value;
                OnPropertyChanged();
            }
        }

        string _Email;
        public string Email
        {
            get { return _Email; }
            set
            {
                if (_Email == value)
                    return;
                _Email = value;
                OnPropertyChanged();
            }
        }

        string _Phone;
        public string Phone
        {
            get { return _Phone; }
            set
            {
                if (_Phone == value)
                    return;
                _Phone = value;
                OnPropertyChanged();
            }
        }

        string _Street;
        public string Street
        {
            get { return _Street; }
            set
            {
                if (_Street == value)
                    return;
                _Street = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AddressString));
            }
        }
            
        string _Unit;
        public string Unit
        {
            get { return _Unit; }
            set
            {
                if (_Unit == value)
                    return;
                _Unit = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AddressString));
            }
        }

        string _City;
        public string City
        {
            get { return _City; }
            set
            {
                if (_City == value)
                    return;
                _City = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AddressString));
                OnPropertyChanged(nameof(CityState));
                OnPropertyChanged(nameof(CityStatePostal));
            }
        }

        string _PostalCode;
        public string PostalCode
        {
            get { return _PostalCode; }
            set
            {
                if (_PostalCode == value)
                    return;
                _PostalCode = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AddressString));
                OnPropertyChanged(nameof(StatePostal));
                OnPropertyChanged(nameof(CityStatePostal));
            }
        }


        string _State;
        public string State
        {
            get { return _State; }
            set
            {
                if (_State == value)
                    return;
                _State = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AddressString));
                OnPropertyChanged(nameof(CityState));
                OnPropertyChanged(nameof(StatePostal));
                OnPropertyChanged(nameof(CityStatePostal));
            }
        }

        string _PhotoUrl;
        public string PhotoUrl
        {
            get { return _PhotoUrl; }
            set
            {
                if (_PhotoUrl == value)
                    return;
                _PhotoUrl = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SmallPhotoUrl));
            }
        }

        public string SmallPhotoUrl { get { return PhotoUrl; }}

        [JsonIgnore]
        public string AddressString
        {
            get
            {
                return string.Format(
                    "{0}{1} {2} {3} {4}",
                    Street,
                    !string.IsNullOrWhiteSpace(Unit) ? " " + Unit + "," : !string.IsNullOrWhiteSpace(Street) ? "," : "",
                    !string.IsNullOrWhiteSpace(City) ? City + "," : "",
                    State,
                    PostalCode);
            }
        }

        [JsonIgnore]
        public string DisplayName
        {
            get { return this.ToString(); }
        }

        [JsonIgnore]
        public string DisplayLastNameFirst
        {
            get { return String.Format("{0}, {1}", LastName, FirstName); }
        }

        [JsonIgnore]
        public string CityState
        {
            get { return City + ", " + State; }
        }

        [JsonIgnore]
        public string CityStatePostal
        {
            get { return CityState + " " + PostalCode; }
        }

        [JsonIgnore]
        public string StatePostal
        {
            get { return State + " " + PostalCode; }
        }

        public override string ToString()
        {
            return FirstName + " " + LastName;
        }
    }
}

