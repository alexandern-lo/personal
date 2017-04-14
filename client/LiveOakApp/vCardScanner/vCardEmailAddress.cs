using System.Collections.ObjectModel;

namespace LiveOakApp.vCardScanner
{
	public enum vCardEmailAddressType
	{
		Other = 0,
		Work,
        Home
	}

	public class vCardEmailAddressCollection : Collection<vCardEmailAddress>
	{
		public vCardEmailAddress GetFirstChoice(vCardEmailAddressType emailType)
		{
			vCardEmailAddress firstNonPreferred = null;
			foreach (vCardEmailAddress email in this) {
				if ((email.EmailType & emailType) == emailType) {
					if (firstNonPreferred == null) firstNonPreferred = email;
					if (email.IsPreferred) return email;
				}
			}
			return firstNonPreferred;
		}
	}

    public class vCardEmailAddress
    {
        private string address;
        private vCardEmailAddressType emailType;
        private bool isPreferred;
        public vCardEmailAddress()
        {
            this.address = string.Empty;
            this.emailType = vCardEmailAddressType.Other;
        }

        public vCardEmailAddress(string address)
        {
            this.address = address == null ? string.Empty : address;
            this.emailType = vCardEmailAddressType.Other;
        }

        public vCardEmailAddress(string address, vCardEmailAddressType emailType)
        {
            this.address = address;
            this.emailType = emailType;
        }

        public string Address
        {
            get
            {
                return this.address ?? string.Empty;
            }
            set
            {
                this.address = value;
            }
        }

        public vCardEmailAddressType EmailType
        {
            get
            {
                return this.emailType;
            }
            set
            {
                this.emailType = value;
            }
        }

        public bool IsPreferred
        {
            get
            {
                return this.isPreferred;
            }
            set
            {
                this.isPreferred = value;
            }
        }

        public override string ToString()
        {
            return this.address;
        }
    }

}