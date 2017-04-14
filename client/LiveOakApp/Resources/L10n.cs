using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Reflection;
using SL4N;

#if __IOS__
using Foundation;
#endif

namespace LiveOakApp.Resources
{
    public class L10n
    {
        static readonly ILogger LOG = LoggerFactory.GetLogger<L10n>();

#if __IOS__
		readonly static string _platformName = "iOS";
#elif __ANDROID__
        readonly static string _platformName = "Droid";
#endif
        readonly static ResourceManager resourceManager = new ResourceManager("LiveOakApp." + _platformName + ".Resources.AppResources", typeof(L10n).GetTypeInfo().Assembly);

        static CultureInfo _culture;
        static string _locale;

        public static List<string> SupportedLocales()
        {
            return new List<string> { "en" };
        }

        public static string Localize(string key, string comment)
        {
            if (_culture == null)
            {
                _culture = new CultureInfo(CurrentLocale());
            }

            var localizedString = resourceManager.GetString(key, _culture);
            if (localizedString == null)
            {
                LOG.Error(string.Format("Key '{0}' ({1}) not found in localizations", key, comment));
                return string.Format("%{0}%", key);
            }
            return localizedString;
        }

        public static string CurrentLocale()
        {
            if (_locale == null)
            {
                _locale = GetCurrentLocale();
            }
            return _locale;
        }

        static string GetCurrentLocale()
        {
            var supportedLocales = SupportedLocales();

            string selectedLocale = null;
#if __IOS__
            var locale = NSLocale.PreferredLanguages[0];
            foreach (var prefferedLocale in NSLocale.PreferredLanguages)
            {
                if (supportedLocales.Contains(prefferedLocale))
                {
                    selectedLocale = prefferedLocale;
                    break;
                }
            }
#elif __ANDROID__
            var locale = Java.Util.Locale.Default.Language;
            if (supportedLocales.Contains(locale))
            {
                selectedLocale = locale;
            }
#endif
            if (selectedLocale != null)
            {
                LOG.Debug(string.Format("current locale: {0}", selectedLocale));
                return selectedLocale;
            }
            LOG.Debug(string.Format("locale {0} not supported, resetting to: {1}", locale, supportedLocales[0]));
            return supportedLocales[0];
        }
    }
}
