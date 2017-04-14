using System;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Primitives;

namespace Recurly.AspNetCore.List
{
    public class CouponList : RecurlyList<Coupon>
    {
        internal CouponList()
        {
        }

        internal CouponList(string baseUrl) : base(Client.HttpRequestMethod.Get, baseUrl)
        {
        }

        public override RecurlyList<Coupon> Start
        {
            get { return HasStartPage() ? new CouponList(StartUrl) : RecurlyList.Empty<Coupon>(); }
        }

        public override RecurlyList<Coupon> Next
        {
            get { return HasNextPage() ? new CouponList(NextUrl) : RecurlyList.Empty<Coupon>(); }
        }

        public override RecurlyList<Coupon> Prev
        {
            get { return HasPrevPage() ? new CouponList(PrevUrl) : RecurlyList.Empty<Coupon>(); }
        }

        internal override void ReadXml(XmlReader reader)
        {
            while (reader.Read())
            {
                if ((reader.Name == "coupons" || reader.Name == "unique_coupon_codes") && reader.NodeType == XmlNodeType.EndElement)
                    break;

                if (reader.NodeType == XmlNodeType.Element && reader.Name == "coupon")
                {
                    Add(new Coupon(reader));
                }
            }
        }

        public async Task ReadFromLocation(HttpWebResponse response)
        {
            var url = new Uri(response.Headers["Location"]);
            var qsDictionary =  Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(url.Query);

            StringValues perPageEntries;

            var perPageEntriesResult = qsDictionary.TryGetValue("per_page", out perPageEntries);
            if (perPageEntriesResult && perPageEntries.Count > 0)
                PerPage = int.Parse(perPageEntries[0]);

            var cursor = "";

            StringValues cursorEntries;
            var cursorResult = qsDictionary.TryGetValue("cursor", out cursorEntries);
            if (cursorResult && cursorEntries.Count > 0)
                cursor = cursorEntries[0];

            BaseUrl = url.Scheme + "://" + url.Host + ":" + url.Port + url.AbsolutePath + "?cursor=" + cursor;

            await GetItems();
        }
    }
}