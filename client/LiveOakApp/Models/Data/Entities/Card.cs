using System.Collections.Generic;
using LiveOakApp.vCardScanner;

namespace LiveOakApp.Models.Data.Entities
{
    public class Card
    {
        public string City { get; private set; }
        public string Name { get; private set; }
        public string Surname { get; private set; }
        public string Company { get; private set; }
        public string Address { get; private set; }
        public string ZipCode { get; private set; }
        public string Country { get; private set; }
        public string State { get; private set; }
        public string Title { get; private set; }
        public string CompanyURL { get; private set; }
        public List<CardEmail> Emails { get; private set; }
        public List<CardPhone> Phones { get; private set; }

        public Card(vCard card)
        {
            Emails = new List<CardEmail>();
            Phones = new List<CardPhone>();
            Name = card.GivenName;
            Surname = card.FamilyName;
            Company = card.Organization;
            Title = card.Title;
            if (card.Websites.GetFirstChoice(vCardWebsiteTypes.Work) != null) 
            {
                CompanyURL = card.Websites.GetFirstChoice(vCardWebsiteTypes.Work).Url;
            }
            if (card.DeliveryAddresses.Count > 0) 
            {
                vCardDeliveryAddress appropriateAddress = null;
                foreach (var address in card.DeliveryAddresses)
                {
                    if (address.IsWork)
                    {
                        appropriateAddress = address;
                        break;
                    }
                    if (!address.IsHome)
                    {
                        appropriateAddress = address;
                    }
                }
                if (appropriateAddress == null)
                {
                    appropriateAddress = card.DeliveryAddresses[0];
                }
                Country = appropriateAddress.Country;
                City = appropriateAddress.City;
                State = appropriateAddress.Region;
                Address = appropriateAddress.Street; // check postal code
                ZipCode = appropriateAddress.PostalCode;
            }
            foreach (var phone in card.Phones) {
                var cardPhone = new CardPhone() {
                    Phone = phone.FullNumber
                };
                if (phone.IsHome) {
                    cardPhone.Type = CardPhone.PhoneType.Home;
                } else if (phone.IsWork) {
                    cardPhone.Type = CardPhone.PhoneType.Work;
                } else if (phone.IsCellular) {
                    cardPhone.Type = CardPhone.PhoneType.Mobile;
                } else {
                    cardPhone.Type = CardPhone.PhoneType.Home;
                }
                Phones.Add(cardPhone);
            }
            foreach (var email in card.EmailAddresses) {
                var cardEmail = new CardEmail()
                {
                    Email = email.Address
                };
                cardEmail.Type = CardEmail.EmailType.Other;
                switch (email.EmailType)
                {
                    case vCardEmailAddressType.Work: cardEmail.Type = CardEmail.EmailType.Work; break;
                    case vCardEmailAddressType.Home: cardEmail.Type = CardEmail.EmailType.Home; break;
                    default: cardEmail.Type = CardEmail.EmailType.Other; break;
                }
                Emails.Add(cardEmail);
            }
        }

        public bool HasAnyData { 
            get {
                return !string.IsNullOrEmpty(City) ||
                       !string.IsNullOrEmpty(Name) ||
                       !string.IsNullOrEmpty(Surname) ||
                       !string.IsNullOrEmpty(Company) ||
                       !string.IsNullOrEmpty(Address) ||
                       !string.IsNullOrEmpty(ZipCode) ||
                       !string.IsNullOrEmpty(Country) ||
                       !string.IsNullOrEmpty(State) ||
                       !string.IsNullOrEmpty(Title) ||
                       !string.IsNullOrEmpty(CompanyURL) ||
                       Emails.Count > 0 || 
                       Phones.Count > 0;
            }
        }
    }
}
