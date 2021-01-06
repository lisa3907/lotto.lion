using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Lion.XDroid;
using Java.Util;
using Lion.XDroid.Control;
using Lion.XDroid.Libs;
using Lion.XDroid.Type;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Lion.XDroid.Setting
{
    [Activity(Label = "SettingPushActivity", Theme = "@style/DefaultTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class SettingPushActivity : AppCompatActivity
    {
        ListViewPushAdapter adapter;

        public SettingPushActivity()
        {
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_setting_push);

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
        }

        private void InitData()
        {
            var app_cache = new AppPreferences(this.ApplicationContext);
            var token = app_cache.UserTokenKey;
            {
                this.GetMessages(token);
                this.PushClear(token);
            }

            this.adapter = new ListViewPushAdapter(this, Resource.Layout.listview_push_custom, new List<PushMessage>());
            {
                var lv_push = FindViewById<ListView>(Resource.Id.lv_push);
                lv_push.Adapter = this.adapter;
            }
        }

        private void InitSize()
        {
            var iv_back = FindViewById<ImageView>(Resource.Id.iv_back);
            var iv_back1 = FindViewById<ImageView>(Resource.Id.iv_back1);
            var iv_line1 = FindViewById<ImageView>(Resource.Id.iv_line1);
            iv_back.LayoutParameters = AppCommon.SNG.ResizeDP(this, iv_back.LayoutParameters);
            iv_back1.LayoutParameters = AppCommon.SNG.ResizeDP(this, iv_back1.LayoutParameters);
            iv_line1.LayoutParameters = AppCommon.SNG.ResizeDP(this, iv_line1.LayoutParameters);
        }

        private void GetMessages(string user_token)
        {
            try
            {
                this.Loading(true);

                var _parameter = new Dictionary<string, string>();
                {
                    _parameter.Add("notify_time", AppCommon.SNG.GetDate(Calendar.Instance));
                }

                var api = new ApiAsnycTask(this, GetString(Resource.String.api_url) + "notify/GetMessages", user_token);
                api.Execute(_parameter);

                api.SendFinish += (s, e) =>
                {
                    var _result = JsonConvert.DeserializeObject<ApiResult<List<PushMessage>>>(e.Json);
                    if (_result != null)
                    {
                        if (_result.success == true)
                        {
                            this.adapter.Clear();
                            this.adapter.AddAll(_result.result);
                            this.adapter.NotifyDataSetChanged();

                            var tv_noting = FindViewById<TextView>(Resource.Id.tv_noting);
                            if (_result.result.Count == 0)
                                tv_noting.Visibility = ViewStates.Visible;
                            else
                                tv_noting.Visibility = ViewStates.Invisible;
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

        private void PushClear(string user_token)
        {
            try
            {
                var _parameter = new Dictionary<string, string>();

                var api = new ApiAsnycTask(this, GetString(Resource.String.api_url) + "notify/Clear", user_token);
                api.Execute(_parameter);

                api.SendFinish += (s, e) =>
                {

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