using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Lion.XDroid;
using Firebase.Messaging;
using Lion.XDroid.Libs;

namespace Lion.XDroid.Service
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class PushReceiverService : FirebaseMessagingService
    {
        public override void OnMessageReceived(RemoteMessage remoteMessage)
        {
            var data = remoteMessage.Data;

            var _builder = (Notification.Builder)null;
            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                _builder = new Notification.Builder(this, "lion_default_channel");
            else
#pragma warning disable CS0618 // Type or member is obsolete
                _builder = new Notification.Builder(this);
#pragma warning restore CS0618 // Type or member is obsolete


            var notifyBuilder = _builder
                                    .SetContentTitle(data["title"])
                                    .SetContentText(data["message"])
                                    .SetSmallIcon(Resource.Drawable.alarm)
                                    .SetWhen(Java.Lang.JavaSystem.CurrentTimeMillis());

            var _notification_manager = this.ApplicationContext.GetSystemService(Context.NotificationService).JavaCast<NotificationManager>();
            _notification_manager.Notify(0, notifyBuilder.Build());

            AppCommon.SNG.ResetBadge(this);
        }
    }
}