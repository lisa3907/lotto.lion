using System;
using Android.Content;
using Android.Support.V7.App;

namespace Lion.XDroid.Libs
{
    public class AppDialog
    {
        private static AppDialog sigleton = null;

        public static AppDialog SNG
        {
            get
            {
                if (sigleton == null)
                    sigleton = new AppDialog();

                return sigleton;
            }
        }

        public void Alert(Context context, string text)
        {
            var builder = new AlertDialog.Builder(context, Android.Resource.Style.ThemeHoloLightDialogNoActionBar);
            builder.SetTitle("알림")
                    .SetMessage(text)
                    .SetCancelable(false)
                    .SetPositiveButton("확인", 
                        (s, e) =>
                        {
                            ((AlertDialog)s).Cancel();
                        });

            var _dialog = builder.Create();
            _dialog.Show();
        }

        public void Alert(Context context, string text, EventHandler<DialogEventArgs> listener)
        {
            var builder = new AlertDialog.Builder(context, Android.Resource.Style.ThemeHoloLightDialogNoActionBar);
            builder.SetTitle("알림")
                    .SetMessage(text)
                    .SetCancelable(false)
                    .SetPositiveButton("확인",
                        (s, e) =>
                        {
                            listener?.Invoke(this, new DialogEventArgs(false));
                            ((AlertDialog)s).Cancel();
                        });

            var _dialog = builder.Create();
            _dialog.Show();
        }

        public void CancelAlert(Context context, string text, EventHandler<DialogEventArgs> listener)
        {
            var builder = new AlertDialog.Builder(context, Android.Resource.Style.ThemeHoloLightDialogNoActionBar);
            builder.SetTitle("알림")
                    .SetMessage(text)
                    .SetCancelable(false)
                    .SetPositiveButton("확인", (s, e) =>
                        {
                            listener?.Invoke(this, new DialogEventArgs(true));
                            ((AlertDialog)s).Cancel();
                        })
                    .SetNegativeButton("취소",  (s, e) =>
                        {
                        
                        });

            var _dialog = builder.Create();
            _dialog.Show();
        }
    }
}