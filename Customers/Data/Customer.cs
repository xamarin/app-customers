using System;
using Newtonsoft.Json;
using MvvmHelpers;

namespace Customers
{
    public class Customer : ObservableObject
    {
        public Customer()
        {
            Id = Guid.NewGuid().ToString();
            PhotoUrl = "placeholderProfileImage";
        }

        public string Id { get; set; }

        string _FirstName;
        public string FirstName
        {
            get { return _FirstName; }
            set
            {
                SetProperty(ref _FirstName, value);
                OnPropertyChanged(nameof(DisplayName)); // because DisplayName is dependent on FirstName, we need to manually call OnPropertyChanged() on DisplayName
                OnPropertyChanged(nameof(DisplayLastNameFirst)); // because DisplayLastNameFirst is dependent on FirstName, we need to manually call OnPropertyChanged() on DisplayLastNameFirst
            }
        }

        string _LastName;
        public string LastName
        {
            get { return _LastName; }
            set
            {
                SetProperty(ref _LastName, value);
                OnPropertyChanged(nameof(DisplayName)); // because DisplayName is dependent on LastName, we need to manually call OnPropertyChanged() on DisplayName
                OnPropertyChanged(nameof(DisplayLastNameFirst)); // because DisplayLastNameFirst is dependent on LastName, we need to manually call OnPropertyChanged() on DisplayLastNameFirst
            }
        }

        string _Company;
        public string Company
        {
            get { return _Company; }
            set { SetProperty(ref _Company, value); }
        }

        string _JobTitle;
        public string JobTitle
        {
            get { return _JobTitle; }
            set { SetProperty(ref _JobTitle, value); }
        }

        string _Email;
        public string Email
        {
            get { return _Email; }
            set { SetProperty(ref _Email, value); }
        }

        string _Phone;
        public string Phone
        {
            get { return _Phone; }
            set { SetProperty(ref _Phone, value); }
        }

        string _Street;
        public string Street
        {
            get { return _Street; }
            set
            {
                SetProperty(ref _Street, value);
                OnPropertyChanged(nameof(AddressString)); // because AddressString is dependent on Street, we need to manually call OnPropertyChanged() on AddressString
            }
        }

        string _City;
        public string City
        {
            get { return _City; }
            set
            {
                SetProperty(ref _City, value);
                OnPropertyChanged(nameof(AddressString)); // because AddressString is dependent on City, we need to manually call OnPropertyChanged() on AddressString
            }
        }

        string _PostalCode;
        public string PostalCode
        {
            get { return _PostalCode; }
            set
            {
                SetProperty(ref _PostalCode, value);
                OnPropertyChanged(nameof(AddressString)); // because AddressString is dependent on PostalCode, we need to manually call OnPropertyChanged() on AddressString
                OnPropertyChanged(nameof(StatePostal)); // because StatePostal is dependent on PostalCode, we need to manually call OnPropertyChanged() on StatePostal
            }
        }


        string _State;
        public string State
        {
            get { return _State; }
            set
            {
                SetProperty(ref _State, value);
                OnPropertyChanged(nameof(AddressString)); // because AddressString is dependent on State, we need to manually call OnPropertyChanged() on AddressString
                OnPropertyChanged(nameof(StatePostal)); // because StatePostal is dependent on State, we need to manually call OnPropertyChanged() on StatePostal
            }
        }

        string _PhotoUrl;
        public string PhotoUrl
        {
            get { return _PhotoUrl; }
            set
            {
                SetProperty(ref _PhotoUrl, value);
                OnPropertyChanged(nameof(SmallPhotoUrl)); // because SmallPhotoUrl is dependent on PhotoUrl, we need to manually call OnPropertyChanged() on SmallPhotoUrl
            }
        }

        public string SmallPhotoUrl { get { return PhotoUrl; }}

        [JsonIgnore]
        public string AddressString
        {
            get
            {
                return string.Format(
                    "{0} {1} {2} {3}",
                    Street,
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

