using System;

namespace LiveOakApp.vCardScanner
{
    public class vCardProperty
    {

        private string group;
        private string language;
        private string name;
        private vCardSubpropertyCollection subproperties;
        private object value;

        public vCardProperty()
        {
            this.subproperties = new vCardSubpropertyCollection();
        }

        public vCardProperty(string name)
        {
            this.name = name;
            this.subproperties = new vCardSubpropertyCollection();
        }

        public vCardProperty(string name, string value)
        {
            this.name = name;
            this.subproperties = new vCardSubpropertyCollection();
            this.value = value;
        }

        public vCardProperty(string name, string value, string group)
        {
            this.group = group;
            this.name = name;
            this.subproperties = new vCardSubpropertyCollection();
            this.value = value;
        }

        public vCardProperty(string name, byte[] value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;
            this.subproperties = new vCardSubpropertyCollection();
            this.value = value;
        }

        public vCardProperty(string name, DateTime value)
        {

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;
            this.subproperties = new vCardSubpropertyCollection();
            this.value = value;
        }

        public string Group
        {
            get
            {
                return this.group ?? string.Empty;
            }
            set
            {
                this.group = value;
            }
        }

        public string Language
        {
            get
            {
                return this.language ?? string.Empty;
            }
            set
            {
                this.language = value;
            }
        }

        public string Name
        {
            get
            {
                return this.name ?? string.Empty;
            }
            set
            {
                this.name = value;
            }
        }

        public vCardSubpropertyCollection Subproperties
        {
            get
            {
                return this.subproperties;
            }
        }

        public override string ToString()
        {
            if (value == null)
            {
                return string.Empty;
            }
            else
            {
                return value.ToString();
            }

        }

        public object Value
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