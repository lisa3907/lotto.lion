using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Lion.XDroid;
using Lion.XDroid.Libs;

namespace Lion.XDroid.Setting
{
    [Activity(Label = "SettingNameActivity", Theme = "@style/DefaultTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class SettingNameActivity : AppCompatActivity
    {
        public SettingNameActivity()
        {
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_setting_name);

            this.InitSize();
            this.InitEvent();
            this.InitData();
        }

        private void InitEvent()
        {
            var iv_back = FindViewById<ImageView>(Resource.Id.iv_back);
            iv_back.Touch += (s, e) =>
            {
                var iv_back1 = FindViewById<ImageView>(Resource.Id.iv_back1);
                if (e.Event.Action == MotionEventActions.Down)
                {
                    iv_back1.SetColorFilter(0xAA111111);
                }
                else if (e.Event.Action == MotionEventActions.Up)
                {
                    iv_back1.SetColorFilter(0x00000000);
                }

                e.Handled = false;
            };

            iv_back.Click += (s, e) =>
            {
                this.Finish();
            };

            var btn_change_name = FindViewById<Button>(Resource.Id.btn_change_name);
            btn_change_name.Click += (s, e) =>
            {
                var et_new_name = FindViewById<EditText>(Resource.Id.et_new_name);
                if (et_new_name.Text.Length <= 0)
                {
                    AppDialog.SNG.Alert(this, "이름을 입력 하세요.");
                }
                else
                {
                    var tv_old_name = FindViewById<TextView>(Resource.Id.tv_old_name);

                    this.Intent.PutExtra("old_name", tv_old_name.Text);
                    this.Intent.PutExtra("new_name", et_new_name.Text);
                    this.SetResult(Result.Ok, this.Intent);
                    this.Finish();
                }
            };
        }

        private void InitSize()
        {
            var iv_back = FindViewById<ImageView>(Resource.Id.iv_back);
            var iv_back1 = FindViewById<ImageView>(Resource.Id.iv_back1);
            var iv_line1 = FindViewById<ImageView>(Resource.Id.iv_line1);
            var iv_old_name = FindViewById<ImageView>(Resource.Id.iv_old_name);
            var iv_new_name = FindViewById<ImageView>(Resource.Id.iv_new_name);
            iv_back.LayoutParameters = AppCommon.SNG.ResizeDP(this, iv_back.LayoutParameters);
            iv_back1.LayoutParameters = AppCommon.SNG.ResizeDP(this, iv_back1.LayoutParameters);
            iv_line1.LayoutParameters = AppCommon.SNG.ResizeDP(this, iv_line1.LayoutParameters);
            iv_old_name.LayoutParameters = AppCommon.SNG.ResizeDP(this, iv_old_name.LayoutParameters);
            iv_new_name.LayoutParameters = AppCommon.SNG.ResizeDP(this, iv_new_name.LayoutParameters);
        }

        private void InitData()
        {
            var _name = this.Intent.GetStringExtra("old_name");

            var tv_old_name = FindViewById<TextView>(Resource.Id.tv_old_name);
            var et_new_name = FindViewById<EditText>(Resource.Id.et_new_name);

            tv_old_name.Text = _name;
            et_new_name.Text = _name;
        }
    }
}