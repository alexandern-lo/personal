using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LiveOakApp.vCardScanner
{
    public class vCardSubpropertyCollection : Collection<vCardSubproperty>
    {
        public void Add(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            else
            {
                Add(new vCardSubproperty(name));
            }
        }

        public void Add(string name, string value)
        {
            Add(new vCardSubproperty(name, value));
        }

        public void AddOrUpdate(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            int index = IndexOf(name);

            if (index == -1)
            {
                Add(name, value);
            }
            else
            {
                this[index].Value = value;
            }
        }

        public bool Contains(string name)
        {
            foreach (vCardSubproperty sub in this)
            {
                if (string.Compare(name, sub.Name, StringComparison.OrdinalIgnoreCase) == 0)
                    return true;
            }

            return false;
        }

        public string[] GetNames()
        {
            var names = new List<string>(this.Count);
            foreach (vCardSubproperty sub in this)
            {
                names.Add(sub.Name);
            }

			return names.ToArray();
        }

        public string[] GetNames(string[] filteredNames)
        {

            if (filteredNames == null)
                throw new ArgumentNullException("filteredNames");

            string[] processedNames =
                (string[])filteredNames.Clone();

            for (int index = 0; index < processedNames.Length; index++)
            {
                if (!string.IsNullOrEmpty(processedNames[index]))
                {
                    processedNames[index] =
                        processedNames[index].Trim().ToUpperInvariant();
                }
            }

            var matchingNames = new List<string>();

            foreach (vCardSubproperty sub in this)
            {
                string subName =
                    sub.Name == null ? null : sub.Name.ToUpperInvariant();

                int matchIndex =
                    Array.IndexOf<string>(processedNames, subName);

                if (matchIndex != -1)
                    matchingNames.Add(processedNames[matchIndex]);
            }
			return matchingNames.ToArray();
        }

        public string GetValue(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            int index = IndexOf(name);
            if (index == -1)
            {
                return null;
            }
            else
            {
                return this[index].Value;
            }
        }

        public string GetValue(string name, string[] namelessValues)
        {

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            int index = IndexOf(name);
            if (index != -1)
            {
                return this[index].Value;
            }

            if ((namelessValues == null) || (namelessValues.Length == 0))
                return null;

            int nameIndex = IndexOfAny(namelessValues);
            if (nameIndex == -1)
            {
                return null;
            }
            else
            {
                return this[nameIndex].Name;
            }
        }

        public int IndexOf(string name)
        {
            for (int index = 0; index < this.Count; index++)
            {
                if (string.Compare(name, this[index].Name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return index;
                }
            }
            return -1;
        }

        public int IndexOfAny(string[] names)
        {

            if (names == null)
                throw new ArgumentNullException("names");

            for (int index = 0; index < this.Count; index++)
            {
                foreach (string name in names)
                {
                    if (string.Compare(this[index].Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return index;
                    }
                }
            }
            return -1;
        }
    }
}