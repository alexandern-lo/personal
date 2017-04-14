using Android.Content;
using Android.Views;
using Android.Views.InputMethods;

namespace StudioMobile
{
    public static class UiUtil
    {
        public static void hideKeyboard(View view)
        {
            InputMethodManager imm = (InputMethodManager)view.Context.GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(view.WindowToken, 0);
        }
    }
}

