using System;
using Android.Support.V4.App;
using Android.Support.V7.App;
using LiveOakApp.Resources;
using Android.Content;
namespace LiveOakApp.Droid.Controller
{
    public class ClearQualifyDialogFragment : DialogFragment
    {

        public EventHandler<DialogClickEventArgs> PositiveCallback { get; set; }
        public EventHandler<DialogClickEventArgs> NegativeCallback { get; set; }

        public override Android.App.Dialog OnCreateDialog(Android.OS.Bundle savedInstanceState)
        {
            return new AlertDialog.Builder(Activity)
                                  .SetTitle(L10n.Localize("ChangeEventWarningText", "Warning"))
                                  .SetPositiveButton(L10n.Localize("Ok", "Ok"), PositiveCallback)
                                  .SetNegativeButton(L10n.Localize("Cancel", "Cancel"), NegativeCallback)
                                  .Create();
        }
    }
}

