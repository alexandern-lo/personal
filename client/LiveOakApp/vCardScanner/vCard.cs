using System.IO;

namespace LiveOakApp.vCardScanner
{
    public class vCard
    {
        private string additionalNames;
        private string department;
        private string displayName;
        private string familyName;
        private string formattedName;
        private string givenName;
        private string mailer;
        private string namePrefix;
        private string nameSuffix;
        private string office;
        private string organization;
        private string productId;
        private string role;
        private string title;

        private vCardDeliveryAddressCollection deliveryAddresses;
        private vCardEmailAddressCollection emailAddresses;
        private vCardPhoneCollection phones;
        private vCardWebsiteCollection websites;

        public vCard()
        {
            this.additionalNames = string.Empty;
            this.department = string.Empty;
            this.displayName = string.Empty;
            this.familyName = string.Empty;
            this.formattedName = string.Empty;
            this.givenName = string.Empty;
            this.mailer = string.Empty;
            this.namePrefix = string.Empty;
            this.nameSuffix = string.Empty;
            this.office = string.Empty;
            this.organization = string.Empty;
            this.productId = string.Empty;
            this.role = string.Empty;
            this.title = string.Empty;

            this.deliveryAddresses = new vCardDeliveryAddressCollection();
            this.emailAddresses = new vCardEmailAddressCollection();
            this.phones = new vCardPhoneCollection();
            this.websites = new vCardWebsiteCollection();
        }

        public vCard(TextReader input) : this()
        {
            vCardStandardReader reader = new vCardStandardReader();
            reader.ReadInto(this, input);
        }

        public string AdditionalNames
        {
            get
            {
                return this.additionalNames ?? string.Empty;
            }
            set
            {
                this.additionalNames = value;
            }
        }

        public vCardDeliveryAddressCollection DeliveryAddresses
        {
            get
            {
                return this.deliveryAddresses;
            }
        }

        public string Department
        {
            get
            {
                return this.department ?? string.Empty;
            }
            set
            {
                this.department = value;
            }
        }

        public string DisplayName
        {
            get
            {
                return this.displayName ?? string.Empty;
            }
            set
            {
                this.displayName = value;
            }
        }

        public vCardEmailAddressCollection EmailAddresses
        {
            get
            {
                return this.emailAddresses;
            }
        }

        public string FamilyName
        {
            get
            {
                return this.familyName ?? string.Empty;
            }
            set
            {
                this.familyName = value;
            }
        }

        public string FormattedName
        {
            get
            {
                return this.formattedName ?? string.Empty;
            }
            set
            {
                this.formattedName = value;
            }
        }

        public string GivenName
        {
            get
            {
                return this.givenName ?? string.Empty;
            }
            set
            {
                this.givenName = value;
            }
        }

        public string Mailer
        {
            get
            {
                return this.mailer ?? string.Empty;
            }
            set
            {
                this.mailer = value;
            }
        }

        public string NamePrefix
        {
            get
            {
                return this.namePrefix ?? string.Empty;
            }
            set
            {
                this.namePrefix = value;
            }
        }

        public string NameSuffix
        {
            get
            {
                return this.nameSuffix ?? string.Empty;
            }
            set
            {
                this.nameSuffix = value;
            }
        }

        public string Office
        {
            get
            {
                return this.office ?? string.Empty;
            }
            set
            {
                this.office = value;
            }
        }

        public string Organization
        {
            get
            {
                return this.organization ?? string.Empty;
            }
            set
            {
                this.organization = value;
            }
        }

        public vCardPhoneCollection Phones
        {
            get
            {
                return this.phones;
            }
        }

        public string ProductId
        {
            get
            {
                return this.productId ?? string.Empty;
            }
            set
            {
                this.productId = value;
            }
        }

        public string Role
        {
            get
            {
                return this.role ?? string.Empty;
            }
            set
            {
                this.role = value;
            }
        }

        public string Title
        {
            get
            {
                return this.title ?? string.Empty;
            }
            set
            {
                this.title = value;
            }
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.formattedName))
            {
                return base.ToString();
            }
            else
            {
                return this.formattedName;
            }
        }

        public vCardWebsiteCollection Websites
        {
            get
            {
                return this.websites;
            }
        }
    }
}
