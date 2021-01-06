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
    public class JoinActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_join);

            this.InitSize();
            this.InitEvent();
        }

        private void InitEvent()
        {
            var btn_email = FindViewById<Button>(Resource.Id.btn_email);
            var btn_certify = FindViewById<Button>(Resource.Id.btn_certify);
            var btn_join = FindViewById<Button>(Resource.Id.btn_join);

            btn_email.Click += ListenerClick;
            btn_certify.Click += ListenerClick;
            btn_join.Click += ListenerClick;

            var iv_back = FindViewById<ImageView>(Resource.Id.iv_back);
            {
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
            }
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

                case Resource.Id.btn_join:
                    var et_id = FindViewById<EditText>(Resource.Id.et_id);
                    et_id.Text = et_id.Text.Trim();

                    var et_name = FindViewById<EditText>(Resource.Id.et_name);
                    et_name.Text = et_name.Text.Trim();

                    var et_pw = FindViewById<EditText>(Resource.Id.et_pw);
                    et_pw.Text = et_pw.Text.Trim();

                    var et_checkPw = FindViewById<EditText>(Resource.Id.et_checkPw);
                    et_checkPw.Text = et_checkPw.Text.Trim();

                    if (et_pw.Text.Length < 6)
                        AppDialog.SNG.Alert(this, "비밀번호는 6자리이상 가능합니다.");
                    else if (et_pw.Text.Equals(et_checkPw.Text) == false)
                        AppDialog.SNG.Alert(this, "비밀번호가 일치하지 않습니다.");
                    else
                        AddMemberByLoginId(_guest_token, 
                                et_email.Text, et_certify.Text, et_id.Text, et_name.Text, et_pw.Text
                            );
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
            var iv_label_id = FindViewById<ImageView>(Resource.Id.iv_label_id);
            var iv_label_pw = FindViewById<ImageView>(Resource.Id.iv_label_pw);
            var iv_label_checkPw = FindViewById<ImageView>(Resource.Id.iv_label_checkPw);
            var iv_label_name = FindViewById<ImageView>(Resource.Id.iv_label_name);
            var btn_email = FindViewById<Button>(Resource.Id.btn_email);
            var btn_certify = FindViewById<Button>(Resource.Id.btn_certify);
            var btn_join = FindViewById<Button>(Resource.Id.btn_join);
            iv_back.LayoutParameters = AppCommon.SNG.ResizeDP(this, iv_back.LayoutParameters);
            iv_back1.LayoutParameters = AppCommon.SNG.ResizeDP(this, iv_back1.LayoutParameters);
            iv_line1.LayoutParameters = AppCommon.SNG.ResizeDP(this, iv_line1.LayoutParameters);
            iv_label_email.LayoutParameters = AppCommon.SNG.ResizeDP(this, iv_label_email.LayoutParameters);
            iv_label_certify.LayoutParameters = AppCommon.SNG.ResizeDP(this, iv_label_certify.LayoutParameters);
            iv_label_id.LayoutParameters = AppCommon.SNG.ResizeDP(this, iv_label_id.LayoutParameters);
            iv_label_pw.LayoutParameters = AppCommon.SNG.ResizeDP(this, iv_label_pw.LayoutParameters);
            iv_label_checkPw.LayoutParameters = AppCommon.SNG.ResizeDP(this, iv_label_checkPw.LayoutParameters);
            iv_label_name.LayoutParameters = AppCommon.SNG.ResizeDP(this, iv_label_name.LayoutParameters);
            btn_email.LayoutParameters = AppCommon.SNG.ResizeDP(this, btn_email.LayoutParameters);
            btn_certify.LayoutParameters = AppCommon.SNG.ResizeDP(this, btn_certify.LayoutParameters);
            btn_join.LayoutParameters = AppCommon.SNG.ResizeDP(this, btn_join.LayoutParameters);
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
                            var btn_email = FindViewById<Button>(Resource.Id.btn_email);
                            var btn_certify = FindViewById<Button>(Resource.Id.btn_certify);
                            var et_email = FindViewById<EditText>(Resource.Id.et_email);
                            var et_certify = FindViewById<EditText>(Resource.Id.et_certify);
                            btn_email.Enabled = false;
                            btn_certify.Enabled = false;
                            et_email.Enabled = false;
                            et_certify.Enabled = false;

                            AppDialog.SNG.Alert(this, "인증되었습니다.");
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

        private void AddMemberByLoginId(string guest_token, string email, string check, string id, string name, string pw)
        {
            try
            {
                this.Loading(true);

                var app_cache = new AppPreferences(this.ApplicationContext);

                var parameter = new Dictionary<string, string>();
                {
                    parameter.Add("login_id", id);
                    parameter.Add("login_name", name);
                    parameter.Add("password", pw);
                    parameter.Add("mail_address", email);
                    parameter.Add("device_type", "A");
                    parameter.Add("device_id", app_cache.GetDeviceId());
                    parameter.Add("check_number", check);
                }

                var _api = new ApiAsnycTask(this, GetString(Resource.String.api_url) + "user/AddMemberByLoginId", guest_token);
                _api.Execute(parameter);

                _api.SendFinish += (s, e) =>
                {
                    var _result = JsonConvert.DeserializeObject<ApiResult<string>>(e.Json);
                    if (_result != null)
                    {
                        if (_result.success == true)
                        {
                            AppDialog.SNG.Alert(this, "회원가입을 축하드립니다.",
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