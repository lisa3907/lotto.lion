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

namespace Lion.XDroid.Setting
{
    [Activity(Label = "SettingPwActivity", Theme = "@style/DefaultTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class SettingPwActivity : AppCompatActivity
    {
        public SettingPwActivity()
        {
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_setting_pw);

            this.InitSize();
            this.InitEvent();
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

            var btn_pw = FindViewById<Button>(Resource.Id.btn_pw);
            btn_pw.Click += (s, e) =>
            {
                var et_pw = FindViewById<EditText>(Resource.Id.et_pw);
                var et_checkPw = FindViewById<EditText>(Resource.Id.et_checkPw);
                if (et_pw.Text.Length < 6)
                {
                    AppDialog.SNG.Alert(this, "비밀번호는 6자리이상 가능합니다.");
                }
                else if (et_pw.Text.Equals(et_checkPw.Text) == false)
                {
                    AppDialog.SNG.Alert(this, "비밀번호가 일치하지 않습니다.");
                }
                else
                {
                    var app_cache = new AppPreferences(this.ApplicationContext);

                    var token = app_cache.UserTokenKey;
                    ChangePassword(token, et_pw.Text);
                }
            };
        }

        private void InitSize()
        {
            var iv_back = FindViewById<ImageView>(Resource.Id.iv_back);
            var iv_back1 = FindViewById<ImageView>(Resource.Id.iv_back1);
            var iv_line1 = FindViewById<ImageView>(Resource.Id.iv_line1);
            var iv_label_pw = FindViewById<ImageView>(Resource.Id.iv_label_pw);
            var iv_label_checkPw = FindViewById<ImageView>(Resource.Id.iv_label_checkPw);
            iv_back.LayoutParameters = AppCommon.SNG.ResizeDP(this, iv_back.LayoutParameters);
            iv_back1.LayoutParameters = AppCommon.SNG.ResizeDP(this, iv_back1.LayoutParameters);
            iv_line1.LayoutParameters = AppCommon.SNG.ResizeDP(this, iv_line1.LayoutParameters);
            iv_label_pw.LayoutParameters = AppCommon.SNG.ResizeDP(this, iv_label_pw.LayoutParameters);
            iv_label_checkPw.LayoutParameters = AppCommon.SNG.ResizeDP(this, iv_label_checkPw.LayoutParameters);
        }

        private void ChangePassword(string user_token, string password)
        {
            try
            {
                this.Loading(true);

                var _parameter = new Dictionary<string, string>();
                {
                    _parameter.Add("password", password);
                }

                var api = new ApiAsnycTask(this, GetString(Resource.String.api_url) + "user/ChangePassword", user_token);
                api.Execute(_parameter);

                api.SendFinish += (s, e) =>
                {
                    var _result = JsonConvert.DeserializeObject<ApiResult<string>>(e.Json);
                    if (_result != null)
                    {
                        if (_result.success == true)
                        {
                            AppDialog.SNG.Alert(this, "비밀번호가 변경되었습니다.",
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