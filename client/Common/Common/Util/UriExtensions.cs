using System;
using System.Collections.Generic;
using System.Linq;

#if __IOS__
using Foundation;
#endif

namespace StudioMobile
{
    public static class UriExtensions
    {
        public static Uri TryParseWebsiteUri(this string uriString)
        {
            return TryParseUri(uriString, new[] { Uri.UriSchemeHttp, Uri.UriSchemeHttps }.ToList());
        }

        public static Uri TryParseEmailUri(this string uriString)
        {
            return TryParseUri(uriString, new[] { Uri.UriSchemeMailto }.ToList());
        }

        static Uri TryParseUri(string uriString, List<string> permittedSchemes)
        {
            if (string.IsNullOrWhiteSpace(uriString)) return null;
            Uri uri = TryCreateAbsoluteUri(uriString, permittedSchemes);
            if (uri != null) return uri;
            var uriStringWithSchema = permittedSchemes[0] + "://" + uriString;
            return TryCreateAbsoluteUri(uriStringWithSchema, permittedSchemes);
        }

        static Uri TryCreateAbsoluteUri(string uriString, List<string> permittedSchemes)
        {
            Uri uri;
            if (Uri.TryCreate(uriString, UriKind.Absolute, out uri)
                && permittedSchemes.Contains(uri.Scheme))
            {
                return uri;
            }
            return null;
        }

#if __IOS__
        public static NSUrl ToNSUrl(this Uri uri)
        {
            return new NSUrl(uri.AbsoluteUri);
        }
#endif

#if __ANDROID__
        public static Android.Net.Uri ToAndroidUri(this Uri uri)
        {
            return Android.Net.Uri.Parse(uri.AbsoluteUri);
        }
#endif
    }
}
