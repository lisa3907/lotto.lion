using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase.Iid;
using Lion.XDroid.Libs;
using Lion.XDroid.Type;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Lion.XDroid
{
    [Activity(Label = "LoginActivity", Theme = "@style/DefaultTheme", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTask)]
    public class LoginActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_login);

            this.InitData();
            this.InitEvent();
            this.InitSize();
        }

        private void InitData()
        {
            var et_id = FindViewById<EditText>(Resource.Id.et_id);
            var cb_id = FindViewById<CheckBox>(Resource.Id.cb_id);

            var app_cache = new AppPreferences(this.ApplicationContext);
            cb_id.Checked = app_cache.SaveLoginIdCheck;
            if (cb_id.Checked)
                et_id.SetText(app_cache.UserLoginId);

            var et_pw = FindViewById<EditText>(Resource.Id.et_pw);
            var cb_pw = FindViewById<CheckBox>(Resource.Id.cb_pw);

            cb_pw.Checked = app_cache.SavePasswordCheck;
            if (cb_pw.Checked)
                et_pw.SetText(app_cache.UserPassword);
        }

        private void InitEvent()
        {
            var btn_login = FindViewById<Button>(Resource.Id.btn_login);
            var tv_join = FindViewById<TextView>(Resource.Id.tv_join);
            var tv_findPw = FindViewById<TextView>(Resource.Id.tv_findPw);
            var cb_id = FindViewById<CheckBox>(Resource.Id.cb_id);
            var cb_pw = FindViewById<CheckBox>(Resource.Id.cb_pw);

            btn_login.Click += ListenerClick;
            tv_join.Click += ListenerClick;
            tv_findPw.Click += ListenerClick;

            tv_join.Touch += ListenerTouch;
            tv_findPw.Touch += ListenerTouch;

            cb_id.CheckedChange += ListenerCheckedChange;
            cb_pw.CheckedChange += ListenerCheckedChange;
        }

        private void InitSize()
        {
            var iv_line1 = FindViewById<ImageView>(Resource.Id.iv_line1);
            var iv_line2 = FindViewById<ImageView>(Resource.Id.iv_line2);
            iv_line1.LayoutParameters = AppCommon.SNG.ResizeDP(this, iv_line1.LayoutParameters);
            iv_line2.LayoutParameters = AppCommon.SNG.ResizeDP(this, iv_line2.LayoutParameters);
        }

        private void ListenerCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            var app_cache = new AppPreferences(this.ApplicationContext);

            var _view = (CompoundButton)sender;
            switch (_view.Id)
            {
                case Resource.Id.cb_id:
                    app_cache.SaveLoginIdCheck = e.IsChecked;
                    break;
                case Resource.Id.cb_pw:
                    app_cache.SavePasswordCheck = e.IsChecked;
                    break;
            }
        }

        private void ListenerTouch(object sender, View.TouchEventArgs e)
        {
            var view = (TextView)sender;
            if (e.Event.Action == MotionEventActions.Down)
            {
                view.SetTextColor(Color.ParseColor("#A0000000"));
            }
            else if (e.Event.Action == MotionEventActions.Up)
            {
                view.SetTextColor(Color.ParseColor("#000000"));
            }

            e.Handled = false;
        }

        private void ListenerClick(object sender, System.EventArgs e)
        {
            var app_cache = new AppPreferences(this.ApplicationContext);

            Intent intent = null;

            var _view = (View)sender;
            switch (_view.Id)
            {
                case Resource.Id.btn_login:
                    var et_id = FindViewById<EditText>(Resource.Id.et_id);
                    et_id.Text = et_id.Text.Trim();

                    app_cache.UserLoginId = et_id.Text;

                    var et_pw = FindViewById<EditText>(Resource.Id.et_pw);
                    et_pw.Text = et_pw.Text.Trim();

                    var cb_pw = FindViewById<CheckBox>(Resource.Id.cb_pw);
                    if (cb_pw.Checked)
                        app_cache.UserPassword = et_pw.Text;

                    var _guest_token = AppCommon.SNG.GetGuestToken(this.ApplicationContext);
                    GetTokenByLoginId(_guest_token, et_id.Text, et_pw.Text);
                    break;

                case Resource.Id.tv_join:
                    intent = new Intent(this, typeof(JoinActivity));
                    StartActivity(intent);
                    break;

                case Resource.Id.tv_findPw:
                    intent = new Intent(this, typeof(FindPwActivity));
                    StartActivity(intent);
                    break;
            }
        }

        private void GetTokenByLoginId(string guest_token, string login_id, string password)
        {
            try
            {
                this.Loading(true);

                var _params = new Dictionary<string, string>();
                {
                    _params.Add("login_id", login_id);
                    _params.Add("password", password);
                    _params.Add("device_type", "A");
                    _params.Add("device_id", FirebaseInstanceId.Instance.Token);
                }

                var _api = new ApiAsnycTask(this, GetString(Resource.String.api_url) + "user/GetTokenByLoginId", guest_token);
                _api.Execute(_params);

                _api.SendFinish += (s, e) =>
                {
                    var _result = JsonConvert.DeserializeObject<ApiResult<string>>(e.Json);
                    if (_result != null)
                    {
                        if (_result.success == true)
                        {
                            var app_cache = new AppPreferences(this.ApplicationContext);
                            app_cache.UserTokenKey = _result.result;

                            this.Finish();
                        }
                        else
                            AppDialog.SNG.Alert(this, _result.message);
                    }

                    Loading(false);
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