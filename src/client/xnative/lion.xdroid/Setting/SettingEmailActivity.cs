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
    [Activity(Label = "SettingEmailActivity", Theme = "@style/DefaultTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class SettingEmailActivity : AppCompatActivity
    {
        public SettingEmailActivity()
        {
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_setting_email);

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

            var btn_email = FindViewById<Button>(Resource.Id.btn_email);
            var btn_certify = FindViewById<Button>(Resource.Id.btn_certify);
            btn_email.Click += ListenerClick;
            btn_certify.Click += ListenerClick;
        }

        private void ListenerClick(object sender, System.EventArgs e)
        {
            var _guest_token = AppCommon.SNG.GetGuestToken(this.ApplicationContext);

            var et_email = FindViewById<EditText>(Resource.Id.et_email);
            var et_certify = FindViewById<EditText>(Resource.Id.et_certify);

            var _view = (Button)sender;
            switch (_view.Id)
            {
                case Resource.Id.btn_email:
                    SendMailToCheckMailAddress(_guest_token, et_email.Text);
                    break;
                case Resource.Id.btn_certify:
                    CheckMailAddress(_guest_token, et_email.Text, et_certify.Text);
                    break;
            }
        }

        private void InitSize()
        {
            var iv_back = FindViewById<ImageView>(Resource.Id.iv_back);
            var iv_back1 = FindViewById<ImageView>(Resource.Id.iv_back1);
            var iv_line1 = FindViewById<ImageView>(Resource.Id.iv_line1);
            var iv_label_email = FindViewById<ImageView>(Resource.Id.iv_label_email);
            var iv_label_certify = FindViewById<ImageView>(Resource.Id.iv_label_certify);

            iv_back.LayoutParameters = AppCommon.SNG.ResizeDP(this, iv_back.LayoutParameters);
            iv_back1.LayoutParameters = AppCommon.SNG.ResizeDP(this, iv_back1.LayoutParameters);
            iv_line1.LayoutParameters = AppCommon.SNG.ResizeDP(this, iv_line1.LayoutParameters);
            iv_label_email.LayoutParameters = AppCommon.SNG.ResizeDP(this, iv_label_email.LayoutParameters);
            iv_label_certify.LayoutParameters = AppCommon.SNG.ResizeDP(this, iv_label_certify.LayoutParameters);
        }

        private void SendMailToCheckMailAddress(string guest_token, string email)
        {
            try
            {
                this.Loading(true);
                    
                var _parameter = new Dictionary<string, string>();
                {
                    _parameter.Add("mail_address", email);
                }

                var api = new ApiAsnycTask(this, GetString(Resource.String.api_url) + "user/SendMailToCheckMailAddress", guest_token);
                api.Execute(_parameter);

                api.SendFinish += (s, e) =>
                {
                    var _result = JsonConvert.DeserializeObject<ApiResult<string>>(e.Json);
                    if (_result != null)
                    {
                        if (_result.success == true)
                        {
                            var et_email = FindViewById<EditText>(Resource.Id.et_email);
                            AppDialog.SNG.Alert(this, et_email.Text + "\n인증코드가 발송되었습니다.");
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

        private void CheckMailAddress(string guest_token, string email, string check)
        {
            try
            {
                this.Loading(true);

                var _parameter = new Dictionary<string, string>();
                {
                    _parameter.Add("mail_address", email);
                    _parameter.Add("check_number", check);
                }

                var api = new ApiAsnycTask(this, GetString(Resource.String.api_url) + "user/CheckMailAddress", guest_token);
                api.Execute(_parameter);

                api.SendFinish += (s, e) =>
                {
                    var _result = JsonConvert.DeserializeObject<ApiResult<string>>(e.Json);
                    if (_result != null)
                    {
                        if (_result.success == true)
                        {
                            var et_email = FindViewById<EditText>(Resource.Id.et_email);
                            var et_certify = FindViewById<EditText>(Resource.Id.et_certify);

                            var app_cache = new AppPreferences(this.ApplicationContext);
                            var _token = app_cache.UserTokenKey;
                            UpdateMailAddress(_token, et_email.Text, et_certify.Text);
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

        private void UpdateMailAddress(string user_token, string email, string check)
        {
            try
            {
                this.Loading(true);

                var _parameter = new Dictionary<string, string>();
                {
                    _parameter.Add("mail_address", email);
                    _parameter.Add("check_number", check);
                }

                var api = new ApiAsnycTask(this, GetString(Resource.String.api_url) + "user/UpdateMailAddress", user_token);
                api.Execute(_parameter);

                api.SendFinish += (s, e) =>
                {
                    var _result = JsonConvert.DeserializeObject<ApiResult<string>>(e.Json);
                    if (_result != null)
                    {
                        if (_result.success == true)
                        {
                            var et_email = FindViewById<EditText>(Resource.Id.et_email);
                            AppDialog.SNG.Alert(this, "메일이 변경되었습니다.",
                                (s1, e2) =>
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