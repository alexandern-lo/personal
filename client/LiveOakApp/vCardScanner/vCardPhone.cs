using System.Collections.ObjectModel;

namespace LiveOakApp.vCardScanner
{
	public enum vCardPhoneTypes
	{
		Default = 0,
		BBS = 1,
		Car = 2,
		Cellular = 4,
		CellularVoice = Cellular + Voice,
		Fax = 8,
		Home = 16,
		HomeVoice = Home + Voice,
		ISDN = 32,
		MessagingService = 64,
		Modem = 128,
		Pager = 256,
		Preferred = 512,
		Video = 1024,
		Voice = 2048,
		Work = 4096,
		WorkFax = Work + Fax,
		WorkVoice = Work + Voice
	}

	public class vCardPhoneCollection : Collection<vCardPhone>
	{
		public vCardPhone GetFirstChoice(vCardPhoneTypes phoneType)
		{
			vCardPhone firstNonPreferred = null;
			foreach (vCardPhone phone in this) {
				if ((phone.PhoneType & phoneType) == phoneType) {
					if (firstNonPreferred == null) firstNonPreferred = phone;
					if (phone.IsPreferred) return phone;
				}
			}
			return firstNonPreferred;
		}
	}

    public class vCardPhone
    {

        private string fullNumber;
        private vCardPhoneTypes phoneType;

        public vCardPhone()
        {
        }

        public vCardPhone(string fullNumber)
        {
            this.fullNumber = fullNumber;
        }

        public vCardPhone(string fullNumber, vCardPhoneTypes phoneType)
        {
            this.fullNumber = fullNumber;
            this.phoneType = phoneType;
        }

        public string FullNumber
        {
            get
            {
                return this.fullNumber ?? string.Empty;
            }
            set
            {
                this.fullNumber = value;
            }
        }

        public bool IsBBS
        {
            get
            {
                return (this.phoneType & vCardPhoneTypes.BBS) == vCardPhoneTypes.BBS;
            }
            set
            {
                if (value)
                {
                    this.phoneType = this.phoneType | vCardPhoneTypes.BBS;
                }
                else
                {
                    this.phoneType = this.phoneType & ~vCardPhoneTypes.BBS;
                }
            }
        }

        public bool IsCar
        {
            get
            {
                return (this.phoneType & vCardPhoneTypes.Car) == vCardPhoneTypes.Car;
            }
            set
            {
                if (value)
                {
                    this.phoneType = this.phoneType | vCardPhoneTypes.Car;
                }
                else
                {
                    this.phoneType = this.phoneType & ~vCardPhoneTypes.Car;
                }
            }
        }

        public bool IsCellular
        {
            get
            {
                return (this.phoneType & vCardPhoneTypes.Cellular) == vCardPhoneTypes.Cellular;
            }
            set
            {
                if (value)
                {
                    this.phoneType = this.phoneType | vCardPhoneTypes.Cellular;
                }
                else
                {
                    this.phoneType = this.phoneType & ~vCardPhoneTypes.Cellular;
                }
            }
        }

        public bool IsFax
        {
            get
            {
                return (this.phoneType & vCardPhoneTypes.Fax) == vCardPhoneTypes.Fax;
            }
            set
            {
                if (value)
                {
                    this.phoneType = this.phoneType | vCardPhoneTypes.Fax;
                }
                else
                {
                    this.phoneType = this.phoneType & ~vCardPhoneTypes.Fax;
                }
            }
        }

        public bool IsHome
        {
            get
            {
                return (this.phoneType & vCardPhoneTypes.Home) == vCardPhoneTypes.Home;
            }
            set
            {
                if (value)
                {
                    this.phoneType = this.phoneType | vCardPhoneTypes.Home;
                }
                else
                {
                    this.phoneType = this.phoneType & ~vCardPhoneTypes.Home;
                }
            }
        }

        public bool IsISDN
        {
            get
            {
                return (this.phoneType & vCardPhoneTypes.ISDN) == vCardPhoneTypes.ISDN;
            }
            set
            {
                if (value)
                {
                    this.phoneType = this.phoneType | vCardPhoneTypes.ISDN;
                }
                else
                {
                    this.phoneType = this.phoneType & ~vCardPhoneTypes.ISDN;
                }
            }
        }

        public bool IsMessagingService
        {
            get
            {
                return (this.phoneType & vCardPhoneTypes.MessagingService) ==
                    vCardPhoneTypes.MessagingService;
            }
            set
            {
                if (value)
                {
                    this.phoneType = this.phoneType | vCardPhoneTypes.MessagingService;
                }
                else
                {
                    this.phoneType = this.phoneType & ~vCardPhoneTypes.MessagingService;
                }
            }
        }

        public bool IsModem
        {
            get
            {
                return (this.phoneType & vCardPhoneTypes.Modem) == vCardPhoneTypes.Modem;
            }
            set
            {
                if (value)
                {
                    this.phoneType = this.phoneType | vCardPhoneTypes.Modem;
                }
                else
                {
                    this.phoneType = this.phoneType & ~vCardPhoneTypes.Modem;
                }
            }
        }

        public bool IsPager
        {
            get
            {
                return (this.phoneType & vCardPhoneTypes.Pager) == vCardPhoneTypes.Pager;
            }
            set
            {
                if (value)
                {
                    this.phoneType = this.phoneType | vCardPhoneTypes.Pager;
                }
                else
                {
                    this.phoneType = this.phoneType & ~vCardPhoneTypes.Pager;
                }
            }
        }

        public bool IsPreferred
        {
            get
            {
                return (this.phoneType & vCardPhoneTypes.Preferred) == vCardPhoneTypes.Preferred;
            }
            set
            {
                if (value)
                {
                    this.phoneType = this.phoneType | vCardPhoneTypes.Preferred;
                }
                else
                {
                    this.phoneType = this.phoneType & ~vCardPhoneTypes.Preferred;
                }
            }
        }

        public bool IsVideo
        {
            get
            {
                return (this.phoneType & vCardPhoneTypes.Video) == vCardPhoneTypes.Video;
            }
            set
            {
                if (value)
                {
                    this.phoneType = this.phoneType | vCardPhoneTypes.Video;
                }
                else
                {
                    this.phoneType = this.phoneType & ~vCardPhoneTypes.Video;
                }
            }
        }

        public bool IsVoice
        {
            get
            {
                return (this.phoneType & vCardPhoneTypes.Voice) == vCardPhoneTypes.Voice;
            }
            set
            {
                if (value)
                {
                    this.phoneType = this.phoneType | vCardPhoneTypes.Voice;
                }
                else
                {
                    this.phoneType = this.phoneType & ~vCardPhoneTypes.Voice;
                }
            }
        }

        public bool IsWork
        {
            get
            {
                return (this.phoneType & vCardPhoneTypes.Work) == vCardPhoneTypes.Work;
            }
            set
            {
                if (value)
                {
                    this.phoneType = this.phoneType | vCardPhoneTypes.Work;
                }
                else
                {
                    this.phoneType = this.phoneType & ~vCardPhoneTypes.Work;
                }
            }
        }

        public vCardPhoneTypes PhoneType
        {
            get
            {
                return this.phoneType;
            }
            set
            {
                this.phoneType = value;
            }
        }

    }

}