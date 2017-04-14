using System;

namespace LiveOakApp.vCardScanner
{
    public class vCardSubproperty
    {
        private string name;
        private string value;
        public vCardSubproperty(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            else
            {
                this.name = name;
            }
        }

        public vCardSubproperty(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            else
            {
                this.name = name;
            }
            this.value = value;
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public string Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }

    }

}