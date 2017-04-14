using System;
using System.Collections.ObjectModel;

namespace LiveOakApp.vCardScanner
{
	public enum vCardDeliveryAddressTypes
	{
		Default = 0,
		Domestic,
		International,
		Postal,
		Parcel,
		Home,
		Work
	}
	
	public class vCardDeliveryAddressCollection : Collection<vCardDeliveryAddress>
	{
	}

    public class vCardDeliveryAddress
    {

        private vCardDeliveryAddressTypes addressType;
        private string city;
        private string country;
        private string postalCode;
        private string region;
        private string street;

        public vCardDeliveryAddress()
        {
            this.city = string.Empty;
            this.country = string.Empty;
            this.postalCode = string.Empty;
            this.region = string.Empty;
            this.street = string.Empty;
        }

        public vCardDeliveryAddressTypes AddressType
        {
            get
            {
                return this.addressType;
            }
            set
            {
                this.addressType = value;
            }
        }

        public string City
        {
            get
            {
                return this.city ?? string.Empty;
            }
            set
            {
                this.city = value;
            }
        }

        public string Country
        {
            get
            {
                return this.country ?? string.Empty;
            }
            set
            {
                this.country = value;
            }
        }

        public bool IsDomestic
        {
            get
            {
                return (this.addressType & vCardDeliveryAddressTypes.Domestic) ==
                    vCardDeliveryAddressTypes.Domestic;
            }
            set
            {

                if (value)
                {
                    this.addressType |= vCardDeliveryAddressTypes.Domestic;
                }
                else
                {
                    this.addressType &= ~vCardDeliveryAddressTypes.Domestic;
                }

            }
        }

        public bool IsHome
        {
            get
            {
                return (this.addressType & vCardDeliveryAddressTypes.Home) ==
                    vCardDeliveryAddressTypes.Home;
            }
            set
            {
                if (value)
                {
                    this.addressType |= vCardDeliveryAddressTypes.Home;
                }
                else
                {
                    this.addressType &= ~vCardDeliveryAddressTypes.Home;
                }

            }
        }

        public bool IsInternational
        {
            get
            {
                return (this.addressType & vCardDeliveryAddressTypes.International) ==
                    vCardDeliveryAddressTypes.International;
            }
            set
            {
                if (value)
                {
                    this.addressType |= vCardDeliveryAddressTypes.International;
                }
                else
                {
                    this.addressType &= ~vCardDeliveryAddressTypes.International;
                }
            }
        }

        public bool IsParcel
        {
            get
            {
                return (this.addressType & vCardDeliveryAddressTypes.Parcel) ==
                    vCardDeliveryAddressTypes.Parcel;
            }
            set
            {
                if (value)
                {
                    this.addressType |= vCardDeliveryAddressTypes.Parcel;
                }
                else
                {
                    this.addressType &= ~vCardDeliveryAddressTypes.Parcel;
                }
            }
        }

        public bool IsPostal
        {
            get
            {
                return (this.addressType & vCardDeliveryAddressTypes.Postal) ==
                    vCardDeliveryAddressTypes.Postal;
            }
            set
            {
                if (value)
                {
                    this.addressType |= vCardDeliveryAddressTypes.Postal;
                }
                else
                {
                    this.addressType &= ~vCardDeliveryAddressTypes.Postal;
                }
            }
        }

        public bool IsWork
        {
            get
            {
                return (this.addressType & vCardDeliveryAddressTypes.Work) ==
                    vCardDeliveryAddressTypes.Work;
            }
            set
            {
                if (value)
                {
                    this.addressType |= vCardDeliveryAddressTypes.Work;
                }
                else
                {
                    this.addressType &= ~vCardDeliveryAddressTypes.Work;
                }
            }
        }

        public string PostalCode
        {
            get
            {
                return this.postalCode ?? string.Empty;
            }
            set
            {
                this.postalCode = value;
            }
        }

        public string Region
        {
            get
            {
                return this.region ?? string.Empty;
            }
            set
            {
                this.region = value;
            }
        }

        public string Street
        {
            get
            {
                return this.street ?? string.Empty;
            }
            set
            {
                this.street = value;
            }
        }

    }

}