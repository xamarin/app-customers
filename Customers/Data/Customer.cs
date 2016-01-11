using System;
using Newtonsoft.Json;

namespace Customers
{
    public class Customer
    {
        public Customer()
        {
            Id = FirstName = LastName = Company = Department = Email = Phone = Street = Unit = City = PostalCode = State = Country = PhotoUrl = string.Empty;
        }

        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string JobTitle { get; set; }
        public string Department { get; set;}
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Street { get; set; }
        public string Unit { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PhotoUrl { get; set; }
        public string SmallPhotoUrl { get { return PhotoUrl; }}

        [JsonIgnore]
        public string AddressString
        {
            get
            {
                return string.Format(
                    "{0}{1} {2} {3} {4}",
                    Street,
                    !string.IsNullOrWhiteSpace(Unit) ? " " + Unit + "," : string.Empty + ",",
                    !string.IsNullOrWhiteSpace(City) ? City + "," : string.Empty,
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

