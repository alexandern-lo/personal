using System;
using System.IO;
using System.Text;

namespace LiveOakApp.vCardScanner
{
	public enum vCardEncoding
	{
		Unknown = 0,
		Escaped,
		Base64,
		QuotedPrintable
	}

    public class vCardStandardReader
    {
        private readonly string[] DeliveryAddressTypeNames = new string[] {
            "DOM",
            "INTL",
            "POSTAL",
            "PARCEL",
            "HOME",
            "WORK",
            "PREF" };
		
        private enum QuotedPrintableState
        {
            None,
            ExpectingHexChar1,
            ExpectingHexChar2,
            ExpectingLineFeed
        }

        #region [ DecodeBase64(string) ]

        public static byte[] DecodeBase64(string value)
        {
            return Convert.FromBase64String(value);
        }

        #endregion

        #region [ DecodeBase64(char[]) ]

        public static byte[] DecodeBase64(char[] value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            else
            {
                return Convert.FromBase64CharArray(value, 0, value.Length);
            }
        }

        #endregion

        #region [ DecodeEmailAddressType ]

        public static vCardEmailAddressType? DecodeEmailAddressType(string keyword)
        {
            if (string.IsNullOrEmpty(keyword)) return null;
            switch (keyword.ToUpperInvariant())
            {
                case "WORK": return vCardEmailAddressType.Work;
                case "HOME": return vCardEmailAddressType.Home;
                default: return null;
            }
        }

        #endregion

        #region [ DecodeEscaped ]

        public static string DecodeEscaped(string value)
        {

            if (string.IsNullOrEmpty(value))
                return value;

            StringBuilder builder = new StringBuilder(value.Length);

            int startIndex = 0;
            do
            {
                int nextIndex = value.IndexOf('\\', startIndex);

                if ((nextIndex == -1) || (nextIndex == value.Length - 1))
                {
                    builder.Append(
                        value,
                        startIndex,
                        value.Length - startIndex);
                    break;

                }
                else
                {
                    char code = value[nextIndex + 1];
                    builder.Append(
                        value,
                        startIndex,
                        nextIndex - startIndex);

                    switch (code)
                    {
                        case '\\':
                        case ',':
                        case ';':

                            builder.Append(code);
                            nextIndex += 2;
                            break;

                        case 'n':
                        case 'N':
                            builder.Append('\n');
                            nextIndex += 2;
                            break;

                        case 'r':
                        case 'R':
                            builder.Append('\r');
                            nextIndex += 2;
                            break;

                        default:
                            builder.Append('\\');
                            builder.Append(code);
                            nextIndex += 2;
                            break;
                    }
                }
                startIndex = nextIndex;
            } while (startIndex < value.Length);
            return builder.ToString();
        }

        #endregion

        #region [ DecodeHexadecimal ]

        public static int DecodeHexadecimal(char value)
        {
            if (char.IsDigit(value))
            {
                return Convert.ToInt32(char.GetNumericValue(value));
            }

            else if ((value >= 'A') && (value <= 'F'))
            {
                return Convert.ToInt32(value) - 55;
            }

            else if ((value >= 'a') && (value <= 'f'))
            {
                return Convert.ToInt32(value) - 87;
            }
            else 
				throw new ArgumentOutOfRangeException("value");
        }

        #endregion

        #region [ DecodeQuotedPrintable ]

        public static string DecodeQuotedPrintable(string value)
        {

            if (string.IsNullOrEmpty(value))
                return value;

            StringBuilder builder = new StringBuilder();
            char firstHexChar = '\x0';
            QuotedPrintableState state = QuotedPrintableState.None;
            foreach (char c in value)
            {
                switch (state)
                {
                    case QuotedPrintableState.None:
                        if (c == '=')
                        {
                            state = QuotedPrintableState.ExpectingHexChar1;
                        }
                        else
                        {
                            builder.Append(c);
                        }
                        break;

                    case QuotedPrintableState.ExpectingHexChar1:
                        if (IsHexDigit(c))
                        {
                            firstHexChar = c;
                            state = QuotedPrintableState.ExpectingHexChar2;
                        }

                        else if (c == '\r')
                        {
                            state = QuotedPrintableState.ExpectingLineFeed;
                        }

                        else if (c == '=')
                        {
                            builder.Append('=');
                            state = QuotedPrintableState.ExpectingHexChar1;
                        }

                        else
                        {
                            builder.Append('=');
                            builder.Append(c);
                            state = QuotedPrintableState.None;
                        }
                        break;

                    case QuotedPrintableState.ExpectingHexChar2:
                        if (IsHexDigit(c))
                        {
                            int charValue =
                                (DecodeHexadecimal(firstHexChar) << 4) +
                                DecodeHexadecimal(c);

                            builder.Append((char)charValue);
                            state = QuotedPrintableState.None;

                        }
                        else
                        {
                            builder.Append('=');
                            builder.Append(firstHexChar);
                            builder.Append(c);
                            state = QuotedPrintableState.None;
                        }
                        break;

                    case QuotedPrintableState.ExpectingLineFeed:
                        if (c == '\n')
                        {
                            state = QuotedPrintableState.None;
                        }
                        else if (c == '=')
                        {
                            state = QuotedPrintableState.ExpectingHexChar1;
                        }
                        else
                        {
                            builder.Append(c);
                            state = QuotedPrintableState.None;
                        }

                        break;
                }
            }

            switch (state)
            {
                case QuotedPrintableState.ExpectingHexChar1:
                    builder.Append('=');
                    break;

                case QuotedPrintableState.ExpectingHexChar2:
                    builder.Append('=');
                    builder.Append(firstHexChar);
                    break;

                case QuotedPrintableState.ExpectingLineFeed:
                    builder.Append('=');
                    builder.Append('\r');
                    break;
            }

            return builder.ToString();
        }

        #endregion

        #region [ IsHexDigit ]

        public static bool IsHexDigit(char value)
        {
            if (char.IsDigit(value)) return true;
            return
                ((value >= 'A') && (value <= 'F')) ||
                ((value >= 'a') && (value <= 'f'));
        }

        #endregion

        #region [ ParseEncoding ]

        public static vCardEncoding ParseEncoding(string name)
        {
            if (string.IsNullOrEmpty(name)) return vCardEncoding.Unknown;
            switch (name.ToUpperInvariant())
            {
                case "B": return vCardEncoding.Base64;
                case "BASE64": return vCardEncoding.Base64;
                case "QUOTED-PRINTABLE": return vCardEncoding.QuotedPrintable;
                default: return vCardEncoding.Unknown;
            }
        }

        #endregion

        #region [ ParsePhoneType(string) ]

        public static vCardPhoneTypes ParsePhoneType(string name)
        {
            if (string.IsNullOrEmpty(name))
                return vCardPhoneTypes.Default;

            switch (name.Trim().ToUpperInvariant())
            {
                case "BBS": return vCardPhoneTypes.BBS;
                case "CAR": return vCardPhoneTypes.Car;
                case "CELL": return vCardPhoneTypes.Cellular;
                case "FAX": return vCardPhoneTypes.Fax;
                case "HOME": return vCardPhoneTypes.Home;
                case "ISDN": return vCardPhoneTypes.ISDN;
                case "MODEM": return vCardPhoneTypes.Modem; 
                case "MSG": return vCardPhoneTypes.MessagingService;
                case "PAGER": return vCardPhoneTypes.Pager;
                case "PREF": return vCardPhoneTypes.Preferred;
                case "VIDEO": return vCardPhoneTypes.Video;
                case "VOICE": return vCardPhoneTypes.Voice;
                case "WORK": return vCardPhoneTypes.Work;
                default: return vCardPhoneTypes.Default;
            }
        }

        #endregion

        #region [ ParsePhoneType(string[]) ]

        public static vCardPhoneTypes ParsePhoneType(string[] names)
        {
            vCardPhoneTypes sum = vCardPhoneTypes.Default;

            foreach (string name in names)
            {
                sum |= ParsePhoneType(name);
            }
            return sum;
        }

        #endregion

        #region [ ParseDeliveryAddressType(string) ]

        public static vCardDeliveryAddressTypes ParseDeliveryAddressType(string value)
        {
            if (string.IsNullOrEmpty(value)) return vCardDeliveryAddressTypes.Default;
            switch (value.ToUpperInvariant())
            {
                case "DOM": return vCardDeliveryAddressTypes.Domestic;
                case "HOME": return vCardDeliveryAddressTypes.Home;
                case "INTL": return vCardDeliveryAddressTypes.International;
                case "PARCEL": return vCardDeliveryAddressTypes.Parcel;
                case "POSTAL": return vCardDeliveryAddressTypes.Postal;
                case "WORK": return vCardDeliveryAddressTypes.Work;
                default: return vCardDeliveryAddressTypes.Default;
            }
        }

        #endregion

        #region [ ParseDeliveryAddressType(string[]) ]

        public static vCardDeliveryAddressTypes ParseDeliveryAddressType(string[] typeNames)
        {
            vCardDeliveryAddressTypes allTypes = vCardDeliveryAddressTypes.Default;
            foreach (string typeName in typeNames)
            {
                allTypes |= ParseDeliveryAddressType(typeName);
            }
            return allTypes;
        }

        #endregion


        #region [ ReadInto(vCard, TextReader) ]

        public void ReadInto(vCard card, TextReader reader)
        {
            vCardProperty property;
            do
            {
                property = ReadProperty(reader);

                if (property != null)
                {
                    if (
                        (string.Compare("END", property.Name, StringComparison.OrdinalIgnoreCase) == 0) &&
                        (string.Compare("VCARD", property.ToString(), StringComparison.OrdinalIgnoreCase) == 0))
                    {
                        break;
                    }
                    else
                    {
                        ReadInto(card, property);
                    }
                }

            } while (property != null);
        }

        #endregion

        #region [ ReadInto(vCard, vCardProperty) ]

        public void ReadInto(vCard card, vCardProperty property)
        {
            if (card == null) throw new ArgumentNullException("card");
            if (property == null) throw new ArgumentNullException("property");
            if (string.IsNullOrEmpty(property.Name)) return;

            switch (property.Name.ToUpperInvariant())
            {
                case "ADR": ReadInto_ADR(card, property); break;
                case "EMAIL": ReadInto_EMAIL(card, property); break;
                case "FN": ReadInto_FN(card, property); break;
                case "MAILER": ReadInto_MAILER(card, property); break;
                case "N": ReadInto_N(card, property); break;
                case "NAME": ReadInto_NAME(card, property); break;
                case "ORG": ReadInto_ORG(card, property); break;
                case "PRODID": ReadInto_PRODID(card, property); break;
                case "ROLE": ReadInto_ROLE(card, property); break;
                case "TEL": ReadInto_TEL(card, property); break;
                case "TITLE": ReadInto_TITLE(card, property); break;
                case "URL": ReadInto_URL(card, property); break;
				default: break;
            }
        }

        #endregion

        #region [ ReadInto_ADR() ]

        private void ReadInto_ADR(vCard card, vCardProperty property)
        {
            string[] addressParts =
                property.Value.ToString().Split(new char[] { ';' });

            vCardDeliveryAddress deliveryAddress = new vCardDeliveryAddress();

            if (addressParts.Length >= 7)
                deliveryAddress.Country = addressParts[6].Trim();

            if (addressParts.Length >= 6)
                deliveryAddress.PostalCode = addressParts[5].Trim();

            if (addressParts.Length >= 5)
                deliveryAddress.Region = addressParts[4].Trim();

            if (addressParts.Length >= 4)
                deliveryAddress.City = addressParts[3].Trim();

            if (addressParts.Length >= 3)
                deliveryAddress.Street = addressParts[2].Trim();

            if (
                (string.IsNullOrEmpty(deliveryAddress.City)) &&
                (string.IsNullOrEmpty(deliveryAddress.Country)) &&
                (string.IsNullOrEmpty(deliveryAddress.PostalCode)) &&
                (string.IsNullOrEmpty(deliveryAddress.Region)) &&
                (string.IsNullOrEmpty(deliveryAddress.Street)))
            {
                return;
            }

            deliveryAddress.AddressType =
                ParseDeliveryAddressType(property.Subproperties.GetNames(DeliveryAddressTypeNames));

            foreach (vCardSubproperty sub in property.Subproperties)
            {

                if (
                    (!string.IsNullOrEmpty(sub.Value)) &&
                    (string.Compare("TYPE", sub.Name, StringComparison.OrdinalIgnoreCase) == 0))
                {

                    deliveryAddress.AddressType |=
                        ParseDeliveryAddressType(sub.Value.Split(new char[] { ',' }));
                }
            }
            card.DeliveryAddresses.Add(deliveryAddress);
        }

        #endregion

        #region [ ReadInto_EMAIL ]

        private void ReadInto_EMAIL(vCard card, vCardProperty property)
        {
            vCardEmailAddress email = new vCardEmailAddress();
            email.Address = property.Value.ToString();

            foreach (vCardSubproperty subproperty in property.Subproperties)
            {
                switch (subproperty.Name.ToUpperInvariant())
                {
                    case "PREF": email.IsPreferred = true; break;
                    case "TYPE":
                        string[] typeValues = subproperty.Value.Split(new char[] { ',' });
                        foreach (string typeValue in typeValues)
                        {
                            if (string.Compare("PREF", typeValue, StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                email.IsPreferred = true;
                            }
                            else
                            {
                                vCardEmailAddressType? typeType = DecodeEmailAddressType(typeValue);
                                if (typeType.HasValue) email.EmailType = typeType.Value;
                            }
                        }
                        break;

                    default:
                        vCardEmailAddressType? emailType = DecodeEmailAddressType(subproperty.Name);
                        if (emailType.HasValue) email.EmailType = emailType.Value;
                        break;
                }
            }
            card.EmailAddresses.Add(email);
        }

        #endregion

        #region [ ReadInto_FN ]

        private void ReadInto_FN(vCard card, vCardProperty property)
        {
            card.FormattedName = property.Value.ToString();
        }

        #endregion

        #region [ ReadInto_MAILER ]

        private void ReadInto_MAILER(vCard card, vCardProperty property)
        {
            card.Mailer = property.Value.ToString();
        }

        #endregion

        #region [ ReadInto_N ]

        private void ReadInto_N(vCard card, vCardProperty property)
        {
            string[] names = property.ToString().Split(';');

            card.FamilyName = names[0];
            if (names.Length == 1)
                return;
			
            card.GivenName = names[1];
            if (names.Length == 2)
                return;
			
            card.AdditionalNames = names[2];
            if (names.Length == 3)
                return;
			
            card.NamePrefix = names[3];
            if (names.Length == 4)
                return;
			
            card.NameSuffix = names[4];
        }

        #endregion

        #region [ ReadInto_NAME ]

        private void ReadInto_NAME(vCard card, vCardProperty property)
        {
            card.DisplayName = property.ToString().Trim();
        }

        #endregion

        #region [ ReadInto_ORG ]
		
        private void ReadInto_ORG(vCard card, vCardProperty property)
        {
            card.Organization = property.Value.ToString();
        }

        #endregion

        #region [ ReadInto_PRODID ]

        private void ReadInto_PRODID(vCard card, vCardProperty property)
        {
            card.ProductId = property.ToString();
        }

        #endregion

        #region [ ReadInto_ROLE ]

        private void ReadInto_ROLE(vCard card, vCardProperty property)
        {
            card.Role = property.Value.ToString();
        }

        #endregion

        #region [ ReadInto_TEL ]

        private void ReadInto_TEL(vCard card, vCardProperty property)
        {
            vCardPhone phone = new vCardPhone();

            phone.FullNumber = property.ToString();
            if (string.IsNullOrEmpty(phone.FullNumber))
                return;

            foreach (vCardSubproperty sub in property.Subproperties)
            {
                if (
                    (string.Compare(sub.Name, "TYPE", StringComparison.OrdinalIgnoreCase) == 0) &&
                    (!string.IsNullOrEmpty(sub.Value)))
                {
                    phone.PhoneType |=
                        ParsePhoneType(sub.Value.Split(new char[] { ',' }));
                }
                else
                {
                    phone.PhoneType |= ParsePhoneType(sub.Name);
                }
            }
            card.Phones.Add(phone);
        }

        #endregion

        #region [ ReadInto_TITLE ]

        private void ReadInto_TITLE(vCard card, vCardProperty property)
        {
            card.Title = property.ToString();
        }

        #endregion

        #region [ ReadInto_URL ]

        private void ReadInto_URL(vCard card, vCardProperty property)
        {
            vCardWebsite webSite = new vCardWebsite();

            webSite.Url = property.ToString();

            if (property.Subproperties.Contains("HOME"))
                webSite.IsPersonalSite = true;

            if (property.Subproperties.Contains("WORK"))
                webSite.IsWorkSite = true;

            card.Websites.Add(webSite);
        }

        #endregion

        #region [ ReadProperty(string) ]

        public vCardProperty ReadProperty(string text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");

            using (StringReader reader = new StringReader(text))
            {
                return ReadProperty(reader);
            }
        }

        #endregion

        #region [ ReadProperty(TextReader) ]

        public vCardProperty ReadProperty(TextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            do
            {
                string firstLine = reader.ReadLine();
                if (firstLine == null)
                    return null;
				
                firstLine = firstLine.Trim();
                if (firstLine.Length == 0)
                {
                    continue;
                }

                int colonIndex = firstLine.IndexOf(':');
                if (colonIndex == -1)
                {
                    continue;
                }

                string namePart = firstLine.Substring(0, colonIndex).Trim();
                if (string.IsNullOrEmpty(namePart))
                {
                    continue;
                }

                string[] nameParts = namePart.Split(';');
                for (int i = 0; i < nameParts.Length; i++)
                    nameParts[i] = nameParts[i].Trim();

                if (nameParts[0].Length == 0)
                {
                    continue;
                }

                vCardProperty property = new vCardProperty();
                property.Name = nameParts[0];

                for (int index = 1; index < nameParts.Length; index++)
                {
                    string[] subNameValue =
                        nameParts[index].Split(new char[] { '=' }, 2);

                    if (subNameValue.Length == 1)
                    {
                        property.Subproperties.Add(
                            nameParts[index].Trim());
                    }
                    else
                    {
                        property.Subproperties.Add(
                            subNameValue[0].Trim(),
                            subNameValue[1].Trim());
                    }

                }

                string encodingName =
                    property.Subproperties.GetValue("ENCODING",
                        new string[] { "B", "BASE64", "QUOTED-PRINTABLE" });

                vCardEncoding encoding =
                    ParseEncoding(encodingName);

                string rawValue = firstLine.Substring(colonIndex + 1);

                do
                {
                    int peekChar = reader.Peek();

                    if ((peekChar == 32) || (peekChar == 9))
                    {
                        string foldedLine = reader.ReadLine();
                        rawValue += foldedLine.Substring(1);
                    }
                    else
                    {
                        break;
                    }

                } while (true);

                if (encoding == vCardEncoding.QuotedPrintable && rawValue.Length > 0)
                {
                    while (rawValue[rawValue.Length - 1] == '=')
                    {
                        rawValue += "\r\n" + reader.ReadLine();
                    }
                }

                switch (encoding)
                {
                    case vCardEncoding.Base64:
                        property.Value = DecodeBase64(rawValue);
                        break;

                    case vCardEncoding.Escaped:
                        property.Value = DecodeEscaped(rawValue);
                        break;

                    case vCardEncoding.QuotedPrintable:
                        property.Value = DecodeQuotedPrintable(rawValue);
                        break;

                    default:
                        property.Value = DecodeEscaped(rawValue);
                        break;
                }
                return property;
            } while (true);
        }
        #endregion
    }
}