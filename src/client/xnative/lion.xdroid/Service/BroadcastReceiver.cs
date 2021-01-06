using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;

namespace Lion.XDroid.Service
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { Android.Content.Intent.ActionBootCompleted })]
    [IntentFilter(new[] { "com.google.android.c2dm.intent.RECEIVE" })]
    public class BroadcastReceiver : Android.Content.BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action.Equals(Intent.ActionBootCompleted))
            {
                var _intent = new Intent(context, typeof(MobileService));
                context.StartService(_intent);
            }

            if (intent.Action.Equals("com.google.android.c2dm.intent.RECEIVE"))
            {
                var _data = intent.Extras;
                var _alarm = new Intent("android.intent.action.BADGE_COUNT_UPDATE");

                // 패키지 네임과 클래스 네임 설정
                _alarm.PutExtra("badge_count_package_name", "kr.co.odinsoftware.LION");
                _alarm.PutExtra("badge_count_class_name", "Lion.XDroid.LoginActivity");

                // 업데이트 카운트
                _alarm.PutExtra("badge_count", _data.GetString("badge"));
                context.SendBroadcast(_alarm);

                var _vibrator = context.GetSystemService(Context.VibratorService).JavaCast<Vibrator>();
                if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                    _vibrator.Vibrate(VibrationEffect.CreateOneShot(500, VibrationEffect.DefaultAmplitude));
                else
#pragma warning disable CS0618 // Type or member is obsolete
                    _vibrator.Vibrate(500);
#pragma warning restore CS0618 // Type or member is obsolete
            }
        }
    }
}