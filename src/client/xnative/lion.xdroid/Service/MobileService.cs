using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Annotation;

namespace Lion.XDroid.Service
{
    [Service]
    [IntentFilter(new[] { "kr.co.odinsoftware.LionService" })]
    public class MobileService : Android.App.Service
    {
        public override void OnCreate()
        {
            base.OnCreate();
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            return StartCommandResult.Sticky;
        }

        [Nullable()]
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
    }
}