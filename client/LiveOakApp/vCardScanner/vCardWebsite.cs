using System.Collections.ObjectModel;

namespace LiveOakApp.vCardScanner
{
	public enum vCardWebsiteTypes
	{
		Default = 0,
		Personal = 1,
		Work = 2
	}

	public class vCardWebsiteCollection : Collection<vCardWebsite>
	{
		public vCardWebsite GetFirstChoice(vCardWebsiteTypes siteType)
		{
			vCardWebsite alternate = null;
			foreach (vCardWebsite webSite in this) {
				if ((webSite.WebsiteType & siteType) == siteType) return webSite;
				else {
					if ((alternate == null) && (webSite.WebsiteType == vCardWebsiteTypes.Default)) {
						alternate = webSite;
					}
				}
			}
			return alternate;
		}
	}

    public class vCardWebsite
    {

        private string url;
        private vCardWebsiteTypes websiteType;

        public vCardWebsite()
        {
            this.url = string.Empty;
        }

        public vCardWebsite(string url)
        {
            this.url = url == null ? string.Empty : url;
        }

        public vCardWebsite(string url, vCardWebsiteTypes websiteType)
        {
            this.url = url == null ? string.Empty : url;
            this.websiteType = websiteType;
        }

        public bool IsPersonalSite
        {
            get
            {
                return (this.websiteType & vCardWebsiteTypes.Personal) ==
                    vCardWebsiteTypes.Personal;
            }
            set
            {

                if (value)
                {
                    this.websiteType |= vCardWebsiteTypes.Personal;
                }
                else
                {
                    this.websiteType &= ~vCardWebsiteTypes.Personal;
                }

            }
        }

        public bool IsWorkSite
        {
            get
            {
                return (this.websiteType & vCardWebsiteTypes.Work) ==
                    vCardWebsiteTypes.Work;
            }
            set
            {

                if (value)
                {
                    this.websiteType |= vCardWebsiteTypes.Work;
                }
                else
                {
                    this.websiteType &= ~vCardWebsiteTypes.Work;
                }

            }

        }

        public string Url
        {
            get
            {
                return this.url;
            }
            set
            {
                if (value == null)
                {
                    this.url = string.Empty;
                }
                else
                {
                    this.url = value;
                }
            }
        }

        public vCardWebsiteTypes WebsiteType
        {
            get
            {
                return this.websiteType;
            }
            set
            {
                this.websiteType = value;
            }
        }

        public override string ToString()
        {
            return this.url;
        }
    }

}