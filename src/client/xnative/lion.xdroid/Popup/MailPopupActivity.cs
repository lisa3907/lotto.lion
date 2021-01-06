using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Lion.XDroid;
using Lion.XDroid.Libs;
using System;

namespace Lion.XDroid.Popup
{
    [Activity(Label = "MailPopupActivity", Theme = "@android:style/Theme.Translucent.NoTitleBar", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MailPopupActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_mail_popup);

            this.InitData();
            this.InitEvent();
            this.InitSize();
        }

        private void InitSize()
        {
            var btn_ok = FindViewById<Button>(Resource.Id.btn_ok);
            var btn_cancel = FindViewById<Button>(Resource.Id.btn_cancel);
            btn_ok.LayoutParameters = AppCommon.SNG.ResizeDP(this, btn_ok.LayoutParameters);
            btn_cancel.LayoutParameters = AppCommon.SNG.ResizeDP(this, btn_cancel.LayoutParameters);
        }

        private void InitEvent()
        {
            var btn_ok = FindViewById<Button>(Resource.Id.btn_ok);
            btn_ok.Touch += Listner_Touch;
            btn_ok.Click += Listner_Click;

            var btn_cancel = FindViewById<Button>(Resource.Id.btn_cancel);
            btn_cancel.Touch += Listner_Touch;
            btn_cancel.Click += Listner_Click;
        }

        private void Listner_Click(object sender, EventArgs e)
        {
            var _view = (Button)sender;
            switch (_view.Id)
            {
                case Resource.Id.btn_ok:
                    this.SetResult(Result.Ok, this.Intent);
                    this.Finish();
                    break;
                case Resource.Id.btn_cancel:
                    this.SetResult(Result.Canceled, this.Intent);
                    this.Finish();
                    break;
            }
        }

        private void Listner_Touch(object sender, View.TouchEventArgs e)
        {
            var _view = (Button)sender;
            if (e.Event.Action == MotionEventActions.Down)
            {
                _view.Background.SetColorFilter(0x10000000);
                _view.Invalidate();
            }
            else if (e.Event.Action == MotionEventActions.Up)
            {
                _view.Background.SetColorFilter(0x00000000);
                _view.Invalidate();
            }
            else if (e.Event.Action == MotionEventActions.Cancel)
            {
                _view.Background.SetColorFilter(0x00000000);
                _view.Invalidate();
            }

            e.Handled = false;
        }

        private void InitData()
        {
        }
    }
}