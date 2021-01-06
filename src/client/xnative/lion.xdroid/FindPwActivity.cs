using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Lion.XDroid;
using Lion.XDroid.Libs;
using Lion.XDroid.Type;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Lion.XDroid
{
    [Activity(Label = "JoinActivity", Theme = "@style/DefaultTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class FindPwActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_find_pw);

            this.InitSize();
            this.InitEvent();
        }

        private void InitEvent()
        {
            var btn_email = FindViewById<Button>(Resource.Id.btn_email);
            btn_email.Click += (sender, e) =>
            {
                var _view = (Button)sender;
                switch (_view.Id)
                {
                    case Resource.Id.btn_email:
                        var et_email = FindViewById<EditText>(Resource.Id.et_email);

                        var _guest_token = AppCommon.SNG.GetGuestToken(this.ApplicationContext);
                        SendMailToRecoveryId(_guest_token, et_email.Text);
                        break;
                }
            };

            var iv_back = FindViewById<ImageView>(Resource.Id.iv_back);
            iv_back.Touch += (sender, e) =>
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
        }

        private void InitSize()
        {
            var iv_back = FindViewById<ImageView>(Resource.Id.iv_back);
            var iv_back1 = FindViewById<ImageView>(Resource.Id.iv_back1);
            var iv_line1 = FindViewById<ImageView>(Resource.Id.iv_line1);
            var iv_label_email = FindViewById<ImageView>(Resource.Id.iv_label_email);
            iv_back.LayoutParameters = AppCommon.SNG.ResizeDP(this, iv_back.LayoutParameters);
            iv_back1.LayoutParameters = AppCommon.SNG.ResizeDP(this, iv_back1.LayoutParameters);
            iv_line1.LayoutParameters = AppCommon.SNG.ResizeDP(this, iv_line1.LayoutParameters);
            iv_label_email.LayoutParameters = AppCommon.SNG.ResizeDP(this, iv_label_email.LayoutParameters);

            var btn_email = FindViewById<Button>(Resource.Id.btn_email);
            btn_email.LayoutParameters = AppCommon.SNG.ResizeDP(this, btn_email.LayoutParameters);
        }

        private void SendMailToRecoveryId(string guest_token, string email)
        {
            try
            {
                this.Loading(true);

                var _parameter = new Dictionary<string, string>();
                {
                    _parameter.Add("mail_address", email);
                }

                var api = new ApiAsnycTask(this, GetString(Resource.String.api_url) + "user/SendMailToRecoveryId", guest_token);
                api.Execute(_parameter);

                api.SendFinish += (s, e) =>
                {
                    var _result = JsonConvert.DeserializeObject<ApiResult<string>>(e.Json);
                    if (_result != null)
                    {
                        if (_result.success == true)
                        {
                            var et_email = FindViewById<EditText>(Resource.Id.et_email);
                            AppDialog.SNG.Alert(this, et_email.Text + "\n임시 비밀번호가 발송되었습니다.",
                                (s1, e1) =>
                                {
                                    this.Finish();
                                });
                        }
                        else
                            AppDialog.SNG.Alert(this, _result.message);
                    }

                    this.Loading(false);
                };
            }
            catch (Java.Lang.Exception ex)
            {
                Log.Error(this.GetType().Name, ex.Message);
            }
        }

        private void Loading(bool view)
        {
            var btn_loading = FindViewById<Button>(Resource.Id.btn_loading);
            var pb_loading = FindViewById<ProgressBar>(Resource.Id.pb_loading);
            if (view)
            {
                btn_loading.Visibility = ViewStates.Visible;
                pb_loading.Visibility = ViewStates.Visible;
            }
            else
            {
                btn_loading.Visibility = ViewStates.Invisible;
                pb_loading.Visibility = ViewStates.Invisible;
            }
        }
    }
}