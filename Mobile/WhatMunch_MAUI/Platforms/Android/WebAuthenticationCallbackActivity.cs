using Android.App;
using Android.Content.PM;
using Android.Content;

namespace WhatMunch_MAUI.Platforms.Android
{
    [Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
    [IntentFilter([Intent.ActionView],
              Categories = new[] {  Intent.CategoryDefault, Intent.CategoryBrowsable },
              DataScheme = CALLBACK_SCHEME)]
    public class WebAuthenticationCallbackActivity : WebAuthenticatorCallbackActivity
    {
        const string CALLBACK_SCHEME = "whatmunch";
    }
}
