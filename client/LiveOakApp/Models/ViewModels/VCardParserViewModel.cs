using System;
using System.IO;
using LiveOakApp.Models.Data.Entities;
using LiveOakApp.Resources;
using LiveOakApp.vCardScanner;
using StudioMobile;

namespace LiveOakApp.Models.ViewModels
{
    public class VCardParserViewModel
    {
        public string InvalidQRCodeErrorMessage = L10n.Localize("InvalidQRCodeErrorMessage", "This QR code doesn't contain vCard");
        public string QRCodeContainsUriErrorMessage = L10n.Localize("QRCodeContainsUriErrorMessage", "This QR code contains an URL");
        public bool IsUri { get { return ScanningResult?.Text?.TryParseWebsiteUri() != null; } }
        public ZXing.Result ScanningResult { get; private set; }

        public VCardParserViewModel(ZXing.Result result)
        {
            ScanningResult = result;
        }

        public Card Parse()
        {
            if (ScanningResult == null)
            {
                return null;
            }
            var resultText = ScanningResult.Text;

            TextReader textReader = new StringReader(resultText);
            var card = new Card(new vCard(textReader));
            if (card.HasAnyData)
            {
                return card;
            }

            if (IsUri)
            {
                throw new Exception(QRCodeContainsUriErrorMessage);
            }
            else {
                throw new Exception(InvalidQRCodeErrorMessage);
            }
        }

        public Card Parse(string vCardString)
        {
            TextReader textReader = new StringReader(vCardString);
            var card = new Card(new vCard(textReader));
            if (card.HasAnyData) return card;
            else throw new Exception(InvalidQRCodeErrorMessage);
        }
    }
}
