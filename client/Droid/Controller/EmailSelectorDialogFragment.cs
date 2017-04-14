
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using LiveOakApp.Resources;

namespace LiveOakApp.Droid.Controller
{
    public class EmailSelectorDialogFragment : DialogFragment
    {
        private static string EMAILS_KEY = "EMAILS_KEY";

        public static EmailSelectorDialogFragment CreateWithEmails(string[] emails, Action<string> onEmailChoosenAction) 
        {
            var fragment = new EmailSelectorDialogFragment();
            var args = new Bundle();
            args.PutStringArray(EMAILS_KEY, emails);
            fragment.Arguments = args;
            fragment.OnEmailChoosenAction = onEmailChoosenAction;
            return fragment;
        }

        private Action<string> OnEmailChoosenAction;
        private string[] emails;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            emails = Arguments.GetStringArray(EMAILS_KEY);
        }

        public override Android.App.Dialog OnCreateDialog(Android.OS.Bundle savedInstanceState)
        {
            return new AlertDialog.Builder(Activity)
                                  .SetTitle(L10n.Localize("EmailAlertAction", "Direct email"))
                                  .SetItems(emails, (sender, e) => 
            {
                OnEmailChoosenAction(emails[e.Which]);
                Dismiss();
            })
                                  .Create();
        }
    }
}
