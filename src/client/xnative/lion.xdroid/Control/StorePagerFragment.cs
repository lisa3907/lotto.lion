using Android.Content.PM;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Views;
using Android.Webkit;
using Lion.XDroid;

namespace Lion.XDroid.Control
{
    public class StorePagerFragment : Android.Support.V4.App.Fragment, IPagerFragment
    {
        View rootView;

        public class StoreWebChromeClient : WebChromeClient
        {
            public override void OnGeolocationPermissionsShowPrompt(string origin, GeolocationPermissions.ICallback callback)
            {
                callback.Invoke(origin, true, false);
            }
        }

        public StorePagerFragment()
        {
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            //Log.Debug(this.GetType().Name, $"OnCreate");
            base.OnCreate(savedInstanceState);

            var _permission_check = ContextCompat.CheckSelfPermission(this.Context, Android.Manifest.Permission.AccessFineLocation);
            if (_permission_check == Permission.Denied)
                ActivityCompat.RequestPermissions(this.Activity, new string[] { Android.Manifest.Permission.AccessFineLocation }, 1);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            //Log.Debug(this.GetType().Name, $"OnCreateView ({((ViewPager)container).CurrentItem})");

            this.rootView = inflater.Inflate(Resource.Layout.fragment_store, container, false);
            {
                var _webview = this.rootView.FindViewById<WebView>(Resource.Id.wv_store);
                {
                    _webview.SetWebViewClient(new WebViewClient());
                    _webview.Settings.JavaScriptEnabled = true;
                    _webview.Settings.SetGeolocationEnabled(true);
                    _webview.SetWebChromeClient(new StoreWebChromeClient());

                    _webview.LoadUrl("https://m.map.daum.net/actions/searchView?q=로또%20판매점&tab=place&service=dksearch&viewmap=true");
                }
            }

            return this.rootView;
        }

        public void Initialize()
        {
        }
    }
}