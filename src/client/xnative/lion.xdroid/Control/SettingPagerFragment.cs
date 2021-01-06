using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Lion.XDroid;
using Lion.XDroid.Libs;
using Lion.XDroid.Popup;
using Lion.XDroid.Setting;
using Lion.XDroid.Type;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lion.XDroid.Control
{
    public class SettingPagerFragment : Android.Support.V4.App.Fragment, IPagerFragment
    {
        private const int NAME_CHANGE_POPUP = 1;
        private const int MAX_NUMBER_POPUP = 2;
        private const int FIX_NUMBER_POPUP = 3;

        public event EventHandler<RefreshEventArgs> Refresh;

        View rootView;

        List<UserInfo> user_infos;

        public SettingPagerFragment()
        {
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            //Log.Debug(this.GetType().Name, $"OnCreate");
            base.OnCreate(savedInstanceState);

            user_infos = new List<UserInfo>();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            //Log.Debug(this.GetType().Name, $"OnCreateView ({((ViewPager)container).CurrentItem})");

            this.rootView = inflater.Inflate(Resource.Layout.fragment_setting, container, false);

            this.InitSize(this.rootView);
            this.InitEvent(this.rootView);
            this.InitData(this.rootView);

            return this.rootView;
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            var app_cache = new AppPreferences(this.Context);
            var _token = app_cache.UserTokenKey;

            var _login_id = app_cache.UserLoginId;
            var _user_info = user_infos
                                .Where(u => u.login_id == _login_id)
                                .SingleOrDefault();

            switch (requestCode)
            {
                case NAME_CHANGE_POPUP:
                    if (resultCode == (int)Result.Ok && data != null)
                    {
                        var _old_name = data.GetStringExtra("old_name");
                        var _new_name = data.GetStringExtra("new_name");
                        if (_old_name != _new_name)
                        {
                            _user_info.loginName = _new_name;
                            this.UpdateUserInfor(_token, _user_info);
                        }
                    }
                    break;

                case MAX_NUMBER_POPUP:
                    if (resultCode == (int)Result.Ok && data != null)
                    {
                        _user_info.maxSelectNumber = (short)data.GetIntExtra("number", 0);
                        this.UpdateUserInfor(_token, _user_info);
                    }
                    break;

                case FIX_NUMBER_POPUP:
                    if (resultCode == (int)Result.Ok && data != null)
                    {
                        var _number = data.GetIntExtra("number", 0);

                        var _exist_check = false;
                        if ((_user_info.digit1 != 0 && _user_info.digit1 == _number)
                            || (_user_info.digit2 != 0 && _user_info.digit2 == _number)
                            || (_user_info.digit3 != 0 && _user_info.digit3 == _number))
                            _exist_check = true;

                        if (_exist_check == false)
                        {
                            if (_number == 0)
                            {
                                _user_info.digit3 = 0;
                                _user_info.digit2 = 0;
                                _user_info.digit1 = 0;
                            }
                            else
                            {
                                if (_user_info.digit1 == 0)
                                    _user_info.digit1 = (short)_number;
                                else if (_user_info.digit2 == 0)
                                    _user_info.digit2 = (short)_number;
                                else
                                    _user_info.digit3 = (short)_number;
                            }

                            this.UpdateUserInfor(_token, _user_info);
                        }
                        else
                            AppDialog.SNG.Alert(this.Context, "동일 번호를 선택 하셨습니다.");
                    }
                    break;
            }
        }

        public void Initialize()
        {
            this.InitData(this.rootView);
        }

        private void InitSize(View view)
        {
            var iv_label_name = view.FindViewById<ImageView>(Resource.Id.iv_label_name);
            var iv_label_email = view.FindViewById<ImageView>(Resource.Id.iv_label_email);
            var iv_label_max = view.FindViewById<ImageView>(Resource.Id.iv_label_max);
            var iv_label_fix = view.FindViewById<ImageView>(Resource.Id.iv_label_fix);
            var iv_label_pw = view.FindViewById<ImageView>(Resource.Id.iv_label_pw);
            var iv_label_push = view.FindViewById<ImageView>(Resource.Id.iv_label_push);

            iv_label_name.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, iv_label_name.LayoutParameters);
            iv_label_email.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, iv_label_email.LayoutParameters);
            iv_label_max.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, iv_label_max.LayoutParameters);
            iv_label_fix.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, iv_label_fix.LayoutParameters);
            iv_label_pw.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, iv_label_pw.LayoutParameters);
            iv_label_push.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, iv_label_push.LayoutParameters);

            var btn_logout = view.FindViewById<Button>(Resource.Id.btn_logout);
            var btn_leave = view.FindViewById<Button>(Resource.Id.btn_leave);

            btn_logout.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, btn_logout.LayoutParameters);
            btn_leave.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, btn_leave.LayoutParameters);
        }

        private void InitEvent(View view)
        {
            var iv_label_name = view.FindViewById<ImageView>(Resource.Id.iv_label_name);
            var iv_label_email = view.FindViewById<ImageView>(Resource.Id.iv_label_email);
            var iv_label_max = view.FindViewById<ImageView>(Resource.Id.iv_label_max);
            var iv_label_fix = view.FindViewById<ImageView>(Resource.Id.iv_label_fix);
            var iv_label_pw = view.FindViewById<ImageView>(Resource.Id.iv_label_pw);
            var iv_label_push = view.FindViewById<ImageView>(Resource.Id.iv_label_push);
            var btn_logout = view.FindViewById<Button>(Resource.Id.btn_logout);
            var btn_leave = view.FindViewById<Button>(Resource.Id.btn_leave);

            iv_label_name.Touch += ListenerTouchIV;
            iv_label_email.Touch += ListenerTouchIV;
            iv_label_max.Touch += ListenerTouchIV;
            iv_label_fix.Touch += ListenerTouchIV;
            iv_label_pw.Touch += ListenerTouchIV;
            iv_label_push.Touch += ListenerTouchIV;

            iv_label_name.Click += ListenerClickIV;
            iv_label_email.Click += ListenerClickIV;
            iv_label_max.Click += ListenerClickIV;
            iv_label_fix.Click += ListenerClickIV;
            iv_label_pw.Click += ListenerClickIV;
            iv_label_push.Click += ListenerClickIV;

            btn_logout.Touch += ListenerTouchBT;
            btn_leave.Touch += ListenerTouchBT;
            btn_logout.Click += ListenerClickBT;
            btn_leave.Click += ListenerClickBT;
        }

        private void InitData(View view)
        {
            var app_cache = new AppPreferences(this.Context);
            var _token = app_cache.UserTokenKey;
            var _login_id = app_cache.UserLoginId;

            var _user_info = user_infos
                                .Where(u => u.login_id == _login_id)
                                .SingleOrDefault();

            if (_user_info == null)
            {
                this.GetCount(_token, _user_info);
                this.GetUserInfo(_token, _login_id);
            }
            else
            {
                var _interval = Convert.ToInt32(Resource.String.push_read_interval_minute);
                if (_user_info.last_push_count_read.AddMinutes(_interval) < DateTime.Now)
                    this.GetCount(_token, _user_info);

                DisplayView(_user_info);
            }
        }

        private void ListenerClickBT(object sender, System.EventArgs e)
        {
            var app_cache = new AppPreferences(this.Context);

            var view = (Button)sender;
            switch (view.Id)
            {
                case Resource.Id.btn_logout:
                    app_cache.UserTokenKey = "";
                    Refresh?.Invoke(this, new RefreshEventArgs(true));
                    break;

                case Resource.Id.btn_leave:
                    AppDialog.SNG.CancelAlert(this.Context, "모든 정보가 삭제됩니다.\n정말로 탈퇴하시겠습니까?",
                        (s1, e1) =>
                        {
                            var _token = app_cache.UserTokenKey;
                            LeaveMember(_token);
                        });
                    break;
            }
        }

        private void ListenerTouchBT(object sender, View.TouchEventArgs e)
        {
            var view = (Button)sender;

            if (e.Event.Action == MotionEventActions.Down)
            {
                view.Background.SetColorFilter(0x50000000);
                view.Invalidate();
            }
            else if (e.Event.Action == MotionEventActions.Up)
            {
                view.Background.SetColorFilter(0x00000000);
                view.Invalidate();
            }
            else if (e.Event.Action == MotionEventActions.Cancel)
            {
                view.Background.SetColorFilter(0x00000000);
                view.Invalidate();
            }

            e.Handled = false;
        }

        private void ListenerClickIV(object sender, System.EventArgs e)
        {
            var _intent = (Intent)null;

            var view = (ImageView)sender;
            switch (view.Id)
            {
                case Resource.Id.iv_label_name:
                    var tv_name = this.rootView.FindViewById<TextView>(Resource.Id.tv_name);

                    _intent = new Intent(this.Context, typeof(SettingNameActivity));
                    _intent.PutExtra("old_name", tv_name.Text);
                    StartActivityForResult(_intent, NAME_CHANGE_POPUP);
                    break;

                case Resource.Id.iv_label_email:
                    _intent = new Intent(this.Context, typeof(SettingEmailActivity));
                    StartActivity(_intent);
                    break;

                case Resource.Id.iv_label_max:
                    _intent = new Intent(this.Context, typeof(NumberPopupActivity));
                    _intent.PutExtra("min", 5);
                    _intent.PutExtra("max", 100);
                    _intent.PutExtra("reverse", false);
                    StartActivityForResult(_intent, MAX_NUMBER_POPUP);
                    break;

                case Resource.Id.iv_label_fix:
                    _intent = new Intent(this.Context, typeof(NumberPopupActivity));
                    _intent.PutExtra("min", 0);
                    _intent.PutExtra("max", 45);
                    _intent.PutExtra("reverse", false);
                    StartActivityForResult(_intent, FIX_NUMBER_POPUP);
                    break;

                case Resource.Id.iv_label_pw:
                    _intent = new Intent(this.Context, typeof(SettingPwActivity));
                    StartActivity(_intent);
                    break;

                case Resource.Id.iv_label_push:
                    PutCount(0);

                    _intent = new Intent(this.Context, typeof(SettingPushActivity));
                    StartActivity(_intent);
                    break;
            }
        }

        private void ListenerTouchIV(object sender, View.TouchEventArgs e)
        {
            var view = (ImageView)sender;

            if (e.Event.Action == MotionEventActions.Down)
            {
                view.Background.SetColorFilter(0x30000000);
            }
            else if (e.Event.Action == MotionEventActions.Up)
            {
                view.Background.SetColorFilter(0x00000000);
            }
            else if (e.Event.Action == MotionEventActions.Cancel)
            {
                view.Background.SetColorFilter(0x00000000);
                view.Invalidate();
            }

            e.Handled = false;
        }

        private void GetUserInfo(string user_token, string login_id)
        {
            try
            {
                this.Loading(true);

                var api = new ApiAsnycTask(this.Context, GetString(Resource.String.api_url) + "user/GetUserInfor", user_token);
                api.Execute();

                api.SendFinish += (s, e) =>
                {
                    var _result = JsonConvert.DeserializeObject<ApiResult<UserInfo>>(e.Json);
                    if (_result != null)
                    {
                        if (_result.success == true)
                        {
                            _result.result.login_id = login_id;

                            user_infos.RemoveAll(x => x.login_id == login_id);
                            user_infos.Add(_result.result);

                            DisplayView(_result.result);
                        }
                        else
                            AppDialog.SNG.Alert(this.Context, _result.message);
                    }

                    this.Loading(false);
                };
            }
            catch (Java.Lang.Exception ex)
            {
                Log.Error(this.GetType().Name, ex.Message);
            }
        }

        private void UpdateUserInfor(string user_token, UserInfo user_info)
        {
            try
            {
                this.Loading(true);

                var _parameter = new Dictionary<string, string>();
                {
                    _parameter.Add("login_name", user_info.loginName);
                    _parameter.Add("max_select_number", user_info.maxSelectNumber.ToString());
                    _parameter.Add("digit1", user_info.digit1.ToString());
                    _parameter.Add("digit2", user_info.digit2.ToString());
                    _parameter.Add("digit3", user_info.digit3.ToString());
                }

                var api = new ApiAsnycTask(this.Context, GetString(Resource.String.api_url) + "user/UpdateUserInfor", user_token);
                api.Execute(_parameter);

                api.SendFinish += (s, e) =>
                {
                    var _result = JsonConvert.DeserializeObject<ApiResult<string>>(e.Json);
                    if (_result != null)
                    {
                        if (_result.success == true)
                        {
                            AppDialog.SNG.Alert(this.Context, "변경되었습니다.",
                            (s2, e2) =>
                            {
                                Initialize();
                            });
                        }
                        else
                            AppDialog.SNG.Alert(this.Context, _result.message);
                    }

                    this.Loading(false);
                };
            }
            catch (Java.Lang.Exception ex)
            {
                Log.Error(this.GetType().Name, ex.Message);
            }
        }

        private void PutCount(int push_count)
        {
            var app_cache = new AppPreferences(this.Context);
            var _login_id = app_cache.UserLoginId;
            var _user_info = user_infos
                                .Where(u => u.login_id == _login_id)
                                .SingleOrDefault();

            if (_user_info != null)
            {
                _user_info.last_push_count_read = DateTime.Now;
                _user_info.push_count = push_count;
            }
        }

        private void GetCount(string user_token, UserInfo user_info)
        {
            try
            {
                var api = new ApiAsnycTask(this.Context, GetString(Resource.String.api_url) + "notify/GetCount", user_token);
                api.Execute();

                api.SendFinish += (s, e) =>
                {
                    var _result = JsonConvert.DeserializeObject<ApiResult<string>>(e.Json);
                    if (_result != null)
                    {
                        if (_result.success == true)
                        {
                            //var tv_push = this.rootView.FindViewById<TextView>(Resource.Id.tv_push);
                            //tv_push.SetText(_result.result);

                            PutCount(Convert.ToInt32(_result.result));
                        }
                        else
                            AppDialog.SNG.Alert(this.Context, _result.message);
                    }
                };
            }
            catch (Java.Lang.Exception ex)
            {
                Log.Error(this.GetType().Name, ex.Message);
            }
        }

        private void LeaveMember(string user_token)
        {
            try
            {
                this.Loading(true);

                var api = new ApiAsnycTask(this.Context, GetString(Resource.String.api_url) + "user/LeaveMember", user_token);
                api.Execute();

                api.SendFinish += (s, e) =>
                {
                    var _result = JsonConvert.DeserializeObject<ApiResult<string>>(e.Json);
                    if (_result != null)
                    {
                        if (_result.success == true)
                        {
                            var app_cache = new AppPreferences(this.Context);
                            app_cache.UserTokenKey = "";

                            Refresh?.Invoke(this, new RefreshEventArgs(true));
                        }
                        else
                            AppDialog.SNG.Alert(this.Context, _result.message);
                    }

                    this.Loading(false);
                };
            }
            catch (Java.Lang.Exception ex)
            {
                Log.Error(this.GetType().Name, ex.Message);
            }
        }

        private void DisplayView(UserInfo user_info)
        {
            var tv_name = this.rootView.FindViewById<TextView>(Resource.Id.tv_name);
            tv_name.SetText(user_info.loginName);

            var tv_email = this.rootView.FindViewById<TextView>(Resource.Id.tv_email);
            tv_email.SetText(user_info.emailAddress);

            var tv_max = this.rootView.FindViewById<TextView>(Resource.Id.tv_max);
            tv_max.SetText(user_info.maxSelectNumber.ToString());

            var _digit_list = new int[]
                            {
                                user_info.digit1, user_info.digit2, user_info.digit3
                            };

            var _fix_number = "";
            foreach (var _digit in _digit_list)
            {
                if (_digit > 0)
                {
                    if (_fix_number.Length > 0)
                        _fix_number += ", ";

                    _fix_number += _digit.ToString();
                }
            }

            var tv_fix = this.rootView.FindViewById<TextView>(Resource.Id.tv_fix);
            tv_fix.SetText(_fix_number);

            var tv_push = this.rootView.FindViewById<TextView>(Resource.Id.tv_push);
            tv_push.SetText(user_info.push_count.ToString());
        }

        private void Loading(bool view)
        {
            var btn_loading = this.rootView.FindViewById<Button>(Resource.Id.btn_loading);
            var pb_loading = this.rootView.FindViewById<ProgressBar>(Resource.Id.pb_loading);

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