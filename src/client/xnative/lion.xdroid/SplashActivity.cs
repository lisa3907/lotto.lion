using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Lion.XDroid;
using System.Threading.Tasks;

namespace Lion.XDroid
{
    [Activity(Label = "로또사자", Theme = "@style/DefaultTheme", Icon = "@drawable/ic_launcher", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        Typeface typeface2;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_intro);

            this.typeface2 = Typeface.CreateFromAsset(Assets, "fonts/NanumGothic.ttf");

            var tv_intro_title = FindViewById<TextView>(Resource.Id.tv_intro_title);
            tv_intro_title.SetTypeface(this.typeface2, TypefaceStyle.Normal);

            var tv_intro_bottom = FindViewById<TextView>(Resource.Id.tv_intro_bottom);
            tv_intro_bottom.SetTypeface(this.typeface2, TypefaceStyle.Normal);

            var tv_intro_version = FindViewById<TextView>(Resource.Id.tv_intro_version);
            tv_intro_version.SetTypeface(this.typeface2, TypefaceStyle.Normal);

            var _pkg_info = this.ApplicationContext.PackageManager.GetPackageInfo(this.ApplicationContext.PackageName, 0);
            tv_intro_version.Text = $"v{_pkg_info.VersionName}";
        }

        // Launches the startup task
        protected override void OnResume()
        {
            base.OnResume();

            // Simulates background work that happens behind the splash screen
            var startupWork = new Task(async () => 
            {
                // Performing some startup work that takes a bit of time.
                await Task.Delay(1000); // Simulate a bit of startup work.

                // Startup work is finished - starting MainActivity.
                var _intent = new Intent(Application.Context, typeof(MainActivity));
                StartActivity(_intent);
            });

            startupWork.Start();
        }

        // Prevent the back button from canceling the startup process
        public override void OnBackPressed()
        {
        }
    }
}